using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;


/// <summary>
/// This script is used to determine the behavior of the Question UI for each player.
/// After obtaining a random question from the Question Manager, it displays information in the respective UI elements.
/// It records the response of the player, and determines if it is correct or wrong, for further processing.
/// </summary>

public class DoQuestion : MonoBehaviour
{
    // UI Elements:
    public TextMeshProUGUI description;
    Button[] buttons = new Button[4];

    public RawImage QImage;
    public Button b1;
    public Button b2;
    public Button b3;
    public Button b4;

    public Button submit;
    public TextMeshProUGUI submitText;
    public TextMeshProUGUI counterUI;

    public Image background;
    Color originalColor;

    private Button prev;

    // Question Parameters:
    private Hashtable question;

    
    public bool pointsAwardable;
    public bool inQuestion = false;
    public bool answered = false; 
    public bool correct = false;

    public float bonusTimeLimit;
    public bool bonusAwardable; 

    private int normalPoints = 10;
    private int bonusPoints = 5;
    
    private int maxTime;
    private float curTime;

    // Player related information:
    public GameObject player;
    
    // Question Manager:
    public QuestionManager QM;
    public mouseHeatMap HMap;


    /// <summary>
    /// Start is callled at the very beginning, to intialize all required parameters and setup the first quesiton.
    /// </summary>
    private void Start()
    {
        // Color Setup
        originalColor = background.color;
        HMap = gameObject.GetComponent<mouseHeatMap>();
    }

    /// <summary>
    /// Update is called every frame to determine if the game has ended.
    /// If so, questions are disabled.
    /// </summary>

    private void Update()
    {
        if (QM.isEnded())
        {
            deactivateUI();
            this.enabled = false;
        }
    }

    /// <summary>
    /// OnEnable is called each time a player activates a question.
    /// It sets up a new question and resets the background color of the UI
    /// </summary>

    private void OnEnable()
    {
        // Setup UI
        setupNewQuestion();
        background.color = originalColor;
    }

    /// <summary>
    /// setupNewQuestion is the core function, which obtains a new random question from the Question Manager, and processes the information accordingly
    /// It updates the UI elements in order to display the question and options, as well as record the answers.
    /// </summary>

    private void setupNewQuestion()
    {
        // get new Question
        question = getQuestion();
        bonusTimeLimit = (float)question["bonusTimeLimit"];
        maxTime = (int)question["maxTime"];

        // set description and other question parameters
        description.SetText("Choose the best option of the 4!");
        QImage.texture = (Texture2D)question["Question"];
        int answer = (int)question["Correct"];
        print(answer);
        int correctOption = answer;

        // set up Buttons with options
        buttons[0] = b1;
        buttons[1] = b2;
        buttons[2] = b3;
        buttons[3] = b4;   

        // set listeners of the buttons, to determine which corresponds to the wrong and right answers

        int i = 0;
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(delegate { TaskOnClick(button); });
            button.transform.GetChild(1).gameObject.GetComponent<RawImage>().texture = (Texture2D)question[i];
            i++;
        }
        submit.onClick.AddListener(SubmitClick);
        // start question countdown
        inQuestion = true;
        print(inQuestion);
        StartCoroutine("startCounter");
    }

    /// <summary>
    /// This function is called to access the QM and get a random question from the database
    /// </summary>
    /// <returns></returns>

    private Hashtable getQuestion()
    {
        Hashtable tempData = new Hashtable();
        tempData = QM.getRandomQuestion();
        List<Texture2D> distractors = new List<Texture2D>();
        List<Texture2D> choice4 = new List<Texture2D>();
        string filename = "Assets/Resources/Question_Source\\Tr221\\Tr221Q.png";
        var rawData= System.IO.File.ReadAllBytes(filename);
        Texture2D tex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)

        string path = "Assets/Resources/Question_Source\\"+(string)tempData["ID"];

        foreach (string file in System.IO.Directory.GetFiles(path))
        {
            if (!file.Contains(".meta"))
            {
                if (file.Replace(path,"").Contains("C")){
                    filename = file;
                    rawData = System.IO.File.ReadAllBytes(filename);
                    tex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)
                    tex.LoadImage(rawData);
                    choice4.Add(tex);
                    print(filename+ " Is correct");
                }
                else if (file.Replace(path,"").Contains("Q"))
                {
                    filename = file;
                    rawData = System.IO.File.ReadAllBytes(filename);
                    tex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)
                    tex.LoadImage(rawData);
                    tempData.Add("Question", tex);
                    print(filename+ " Is Question");
                }
                else
                {
                    filename = file;
                    rawData = System.IO.File.ReadAllBytes(filename);
                    tex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)
                    tex.LoadImage(rawData);
                    distractors.Add(tex);
                    print(filename+ " Is Distractor");
                }
            }
        }
        int temp;
        Texture2D tempTex;
        for (int i = 0; i < 3; i++)
        {
            temp = UnityEngine.Random.Range(0, distractors.Count);
            tempTex = distractors[temp];
            distractors.Remove(tempTex);
            choice4.Add(tempTex);
        }
        List<int> numbers = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            numbers.Add(i);
        }
        for (int i = 0; i < 4; i++)
        {
            temp = UnityEngine.Random.Range(0, numbers.Count);
            print(numbers[temp]);
            if (numbers[temp] == 0)
            {
                tempData.Add("Correct", i);
                print(i.ToString() + "is correct");
            }
            tempData.Add(i, choice4[numbers[temp]]);
            numbers.Remove(temp);
        }

        if (QM != null)
        {
            return tempData;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// This function is called to perform the question countdown,
    /// It also updates the UI elements to indicate the time remaining for the question.
    /// </summary>
    /// <returns></returns>
    IEnumerator startCounter()
    {
        bonusAwardable = true;

        submitText.SetText("Submit (+5)");
        submit.GetComponent<Image>().color = new Color (255,216,0,255);

        //curTime in seconds

        curTime = 0;
        counterUI.text = "Time Elapsed: 0";

        while (curTime < (maxTime + 1)&& inQuestion) {
            yield return new WaitForSeconds(1);

            curTime += 1;

            counterUI.text = timeToString(curTime);

            if (curTime > bonusTimeLimit){
                // deactivate bonus points
                bonusAwardable = false;

                submitText.SetText("Submit");
                submit.GetComponent<Image>().color = Color.green;
            }

            //If the countdown runs out of time, then the question is unanswered and it is process accordingly
            
            if (curTime == maxTime){
                if (!answered && !correct)
                {
                    Unanswered();
                }
            }

            
        }
    }

    string timeToString(float time)
    {
        if (time > 60)
        {
            string min = Math.Floor(time / 60).ToString("0");
            string sec = (time % 60).ToString("00");

            return ("Time Elapsed: " + min + ":" + sec);
        }
        else
        {
            return ("Time Elapsed: " + time.ToString("0"));
        }
    }


    /// <summary>
    /// This function is called when a question is finished, to reset all listeners on the buttons to prepare for the next question.
    /// </summary>

    private void resetButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }
    }
    /// <summary>
    /// This function is called when the correct button is clicked.
    /// It records the player's response and awards points accordingly
    /// It also updates the answered and correct parameters, which determines the behavior of the block (remain floating until countdown expires)
    /// Finally, it deactivates the question UI
    /// </summary>
    /// <param name="index"></param>
    void TaskOnClick(Button buttonSel)
    {
        if (buttons.Contains(buttonSel))
        {
            foreach (Button button in buttons)
            {
                if (button == buttonSel)
                {
                    button.gameObject.SetActive(true);
                    button.Select();
                    prev = button;
                }
            }
        }
    }

    void SubmitClick()
    {
        prev.gameObject.SetActive(true);
        prev.Select();
        int count = Array.IndexOf(buttons,prev);
        bool answer = (count == (int)question["Correct"]);

        Hashtable toSend = new Hashtable();

        foreach (Button button in buttons)
        {
        }

        QM.recordResponse((string)question["ID"], count);
        resetButtons();

        if (pointsAwardable)
        {
            int points = normalPoints* Convert.ToInt32(answer);

            if (bonusAwardable)
            {
                if (answer)
                {
                    points += bonusPoints;
                    print("Corect");
                }else
                {
                    points -= bonusPoints;
                    print("Wrong");
                }
                
            }
            player.GetComponent<PlayerController>().ChangePoints(points);
        }
        List<List<int>> test = new List<List<int>>();
        test=HMap.getMaps();

        answered = true;
        correct = answer;
        inQuestion = false;
        if (answer)
        {
            StartCoroutine("Disappear");
        }
        else
        {
            StartCoroutine("FailBGChange");
        }
        
    }


    /// <summary>
    /// This function is called when the countdown expires without a player response
    /// It records the lack of response (-1)
    /// It also updates the answered and correct booleans which affects the block behavior (drop immediately)
    /// Finally, it allows the question UI to transition to the respawn UI.
    /// </summary>

    void Unanswered()
    {
        QM.recordResponse((string)question["ID"], -1);
        resetButtons();

        answered = false;
        correct = false;
        inQuestion = false;

        StartCoroutine("FailBGChange");

        print("Unanswered");

    }

    /// <summary>
    /// This function is called to allow the question UI to transition to the respawn UI, when the question is answered wrongly or unanswered.
    /// </summary>
    /// <returns></returns>

    IEnumerator FailBGChange()
    {
        Color color = background.color;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
        {
            Color newColor = new Color(Mathf.Lerp(color.r, 0.965f, t), Mathf.Lerp(color.g, 0.275f,t), Mathf.Lerp(color.b, 0.196f,t), Mathf.Lerp(color.a, 0.396f, t));

            background.color = newColor;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// This funciton is called to deactivate the question UI when the correct answer is given
    /// </summary>
    /// <returns></returns>

    IEnumerator Disappear()
    {

        Color color = background.color;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.75f)
        {
            Color newColor = new Color(Mathf.Lerp(color.r, 0.0f, t), Mathf.Lerp(color.g, 0.0f, t), Mathf.Lerp(color.b, 0.0f, t), Mathf.Lerp(color.a, 0.0f, t));

            background.color = newColor;

            yield return null;
        }
        
        gameObject.SetActive(false);
    }

    /// <summary>
    /// This function is called to fully deactivate question UI when the game has ended.
    /// </summary>
    void deactivateUI()
    {
        gameObject.SetActive(false);   
    }
}
