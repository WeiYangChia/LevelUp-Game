using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The QuestionManager script is used to fetch the question bank from the database based on the category and difficulty selected.
/// Its functions are called by each player to obtain a randomized question they have not attempted before.
/// It also stores records of each player's responses.
/// </summary>

public class QuestionManager : MonoBehaviour
{
    // Singleton
    public static QuestionManager QM;
    public DoQuestion DQ;
    public Loading_Screen LS;
    public Slider dL;

    // Question parameters
    public int Category;
    public int catLevel;
    public int questionNumber;
    public Dictionary<string, object> question;

    // Game status
    bool ended;
    bool newQ = true;
    bool imageLoaded = false;

    // Responses and Questions:
    public Dictionary<string, int> responses = new Dictionary<string, int>();
    public List<string> questions = new List<string>();
    public List<string> choices = new List<string>();
    public List<MatrixReasoningQ> MRQ = new List<MatrixReasoningQ>();
    public Dictionary<string, object> MRQRecord = new Dictionary<string, object>();
    public Dictionary<string, object> chosen = new Dictionary<string, object>();
    int imageNum = 0;
    public List<string> difLevels = new List<string>();
    private string[] catList = { "TR" ,"RO","TX","FL"};

    //Phton View
    private PhotonView PV;
    

    /// <summary>
    /// The start function is called before the first frame,
    /// and it initializes the question bank by fetching it from the appropriate URL.
    /// </summary>
    /// 


    void Start()
    {
        LS = LS.GetComponent<Loading_Screen>();
        LS.percentageValues(16);
        print(DQ);
        // Initialize settings:
        // Category = MapController.Category;
        // Difficulty = MapController.Difficulty;
        try
        {
            Category = LobbySetUp.LS.category;
            catLevel = LobbySetUp.LS.catLevel;
        }
        catch (Exception e)
        {
            Category = 1;
            catLevel = 1;
        }
        print("Category is " + catList[Category]);
        ended = false;

        RestClient.Get(url: "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Question/Matrix Reasoning/"+ catList[Category]+"/LO1"+".json").Then(onResolved: response =>
        {
            print("https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Question/Matrix Reasoning/" + catList[Category] + "/LO1" + ".json");
            print(response.Text);
            questions = JsonConvert.DeserializeObject<List<string>>(response.Text);
            print("JSON Loaded");
            LS.moveSlider();
            randomQuestions();
        });
    }


    void Update()
    {
        if (newQ && (MRQ.Count > 19))
        {
            newQ = false;
            print("get Question");
            question = getRandomQuestion();
            
        }
        if (imageNum == 5)
        {
            imageLoaded = true;
            
        }
        else
        {
            imageLoaded = false;
        }
    }

    public void randomQuestions()
    {
        
        int choice;
        questionNumber = 0;
        while (choices.Count< 25)
        {
            choice = UnityEngine.Random.Range(0, questions.Count - 1);
            if (!choices.Contains(questions[choice]))
            {
                print(questions[choice]);
                RestClient.Get(url: "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/QuestionList/Matrix Reasoning/" + catList[Category] + "/LO1/" + questions[choice]+".json").Then(onResolved: response =>
                {
                    print("Question Hooked");
                    MatrixReasoningQ temp = JsonConvert.DeserializeObject<MatrixReasoningQ>(response.Text);
                    MRQ.Add(temp);
                    LS.moveSlider();
                });
                choices.Add(questions[choice]);
            }
        }

    }


    /// <summary>
    /// This function is called by each player to obtain a random question from the question bank that he has not attempted before
    /// </summary>
    /// <returns></returns>
    /// 
    public Dictionary<string, object> getGenQuestion()
    {
        
        return question;
    }

    public Dictionary<string, object> getRandomQuestion()
    {
        List<string> tempOpt = new List<string>();
        List<string> finalLoc = new List<string>();
        List<string> finalOpt = new List<string>();
        chosen = new Dictionary<string, object>();


        // Unlikely scenario: Player has answered all questions in the question bank
        //if (responses.Count == questions.Count)
        //{
        //    return null;
        //}

        // Randomize and return appropriate question
        
        int temp = questionNumber;
        MatrixReasoningQ tempMRQ = MRQ[temp];
        print(tempMRQ.ID);

        tempOpt.Add(tempMRQ.Correct);
        print("Difficulty level is at");
        print(Mathf.Floor(dL.value/3));
        switch (Mathf.Floor(dL.value/3))
        {
            case 0:
                tempOpt.Add(tempMRQ.diff3.Distractor1);
                tempOpt.Add(tempMRQ.diff3.Distractor2);
                tempOpt.Add(tempMRQ.diff3.Distractor3);
                break;
            case 1:
                tempOpt.Add(tempMRQ.diff2.Distractor1);
                tempOpt.Add(tempMRQ.diff2.Distractor2);
                tempOpt.Add(tempMRQ.diff2.Distractor3);
                break;
            case 2:
                tempOpt.Add(tempMRQ.diff1.Distractor1);
                tempOpt.Add(tempMRQ.diff1.Distractor2);
                tempOpt.Add(tempMRQ.diff1.Distractor3);
                break;
        }
        

        
        chosen.Add("ID", tempMRQ.ID);
        chosen.Add("bonusTimeLimit"  ,getBonusTimeLimit());
        chosen.Add("maxTime", getMaxTime());

        StartCoroutine(DownloadQImage((string)tempMRQ.Question, chosen));

        int tempNum;
        List<int> numbers = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            numbers.Add(i);
        }
        for (int i = 0; i < 4; i++)
        {
            tempNum = UnityEngine.Random.Range(0, numbers.Count);
            print(numbers[tempNum]);
            if (numbers[tempNum] == 0)
            {
                chosen.Add("Correct", i);
            }
            StartCoroutine(DownloadOptionImage(tempOpt[numbers[tempNum]], i, chosen));
            finalOpt.Add(tempOpt[numbers[tempNum]]);
            numbers.Remove(numbers[tempNum]);
        }
        questionNumber++;
        if (questionNumber >= 24)
        {
            randomQuestions();
        }
        chosen.Add("OptionPlacement", finalOpt);
        
        print(JsonConvert.SerializeObject(chosen));
        return chosen;
    }

    IEnumerator DownloadOptionImage(string OptionImage, int i, Dictionary<string, object> tempData)
    {
        var fileUrl = "https://firebasestorage.googleapis.com/v0/b/test-ebe23.appspot.com/o/Matrix_Reasoning%2F" + OptionImage + ".png?alt=media";
        print(OptionImage);

        using (var www = UnityWebRequestTexture.GetTexture(fileUrl))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                print(fileUrl);
            }
            else
            {
                if (www.isDone)
                {
                    var temp = DownloadHandlerTexture.GetContent(www);
                    tempData.Add(i.ToString(), temp);
                    print("Image loaded for" + i.ToString());
                    imageNum += 1;
                }
            }
        }
    }

    IEnumerator DownloadQImage(string QImageName, Dictionary<string, object> tempData)
    {
        var fileUrl = "https://firebasestorage.googleapis.com/v0/b/test-ebe23.appspot.com/o/Matrix_Reasoning%2F" + QImageName + ".png?alt=media";

        using (var www = UnityWebRequestTexture.GetTexture(fileUrl))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var temp = DownloadHandlerTexture.GetContent(www);
                    tempData["question"] = temp;
                    print("question loaded");
                    imageNum += 1;
                }
            }
        }
    }
    


    /// <summary>
    /// This function is called by each player upon completing a question response, for logging purposes.
    /// </summary>
    /// <param name="questionNum"></param>
    /// <param name="resp"></param>

    public void recordResponse(Dictionary<string, object> questionInfo, int resp, Dictionary<int, Dictionary<string, float>> mouseheatmap, bool answer, float curTime=0f)
    {

        if (answer)
        {
            dL.value += 1;
        }
        else
        {
            dL.value -= 1;
        }

        newQ = true;
        //responses.Add((string)questionInfo["ID"], resp);
        Dictionary<string, object> qToSend = new Dictionary<string, object>();
        qToSend.Add("ID", (string)questionInfo["ID"]);
        qToSend.Add("OptionPlacement", (List<string>)questionInfo["OptionPlacement"]);
        qToSend.Add("mouseMovement", (Dictionary<int, Dictionary<string, float>>)mouseheatmap);
        qToSend.Add("answer", (bool)answer);
        qToSend.Add("TimeTaken", (float)curTime);
        qToSend.Add("response", resp);

        print("Question Number:" + questionNumber);

        MRQRecord.Add(questionNumber.ToString(), qToSend);
        //print(JsonConvert.SerializeObject(MRQRecord));
        imageNum = 0;
    }

    /// <summary>
    /// This function is called to obtain all the player's responses, for uploading into the database when the game ends
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, int> getResponses()
    {
        return responses;
    }

    public Dictionary<string, object> getMRQ()
    {
        return MRQRecord;
    }

    /// <summary>
    /// This function is called to return the time limit for each question based on its difficulty.
    /// </summary>
    /// <returns></returns>

    public float getBonusTimeLimit()
    {
        return 30.0f;
    }

    public int getMaxTime(){
        return 120;
    }

    /// <summary>
    /// This function is called when the game has ended, to stop any questions from being given and answered.
    /// </summary>
    public void setEnded()
    {
        ended = true;
    }

    /// <summary>
    /// This function is called to check if the game has ended.
    /// </summary>
    /// <returns></returns>

    public bool isEnded()
    {
        return ended;
    }

    public bool isImage()
    {
        return imageLoaded;
    }

    public int getDifVal()
    {
        return (int)dL.value;
    }
}
