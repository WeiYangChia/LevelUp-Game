using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;

/// <summary>
/// The QuestionManager script is used to fetch the question bank from the database based on the category and difficulty selected.
/// Its functions are called by each player to obtain a randomized question they have not attempted before.
/// It also stores records of each player's responses.
/// </summary>

public class QuestionManager : MonoBehaviour
{
    // Singleton
    public static QuestionManager QM;

    // Question parameters
    public int Category;
    public int Difficulty;

    // Game status
    bool ended;

    // Responses and Questions:
    public Dictionary<string, int> responses = new Dictionary<string, int>();
    public List<Question> questions = new List<Question>();

    //Phton View
    private PhotonView PV;

    /// <summary>
    /// The start function is called before the first frame,
    /// and it initializes the question bank by fetching it from the appropriate URL.
    /// </summary>
    void Start()
    {
        // Initialize settings:
        // Category = MapController.Category;
        // Difficulty = MapController.Difficulty;
        Category = 0;
        Difficulty = 1;
        ended = false;


        // Craft URL and fetch from DataBase
        string QuestionUrl = "https://quizguyz.firebaseio.com/Questions/";

        switch (Category)
        {
            case 0:
                QuestionUrl += "Math/";
                break;
            case 1:
                QuestionUrl += "Science/";
                break;
            case 2:
                QuestionUrl += "Geography/";
                break;
            case 3:
                QuestionUrl += "General/";
                break;
        }

        QuestionUrl += (Difficulty).ToString();
        QuestionUrl += ".json";
        RestClient.Get(url: QuestionUrl).Then(onResolved: response =>
        {
            questions = JsonConvert.DeserializeObject<List<Question>>(response.Text);
        });
    }

    /// <summary>
    /// This function is called by each player to obtain a random question from the question bank that he has not attempted before
    /// </summary>
    /// <returns></returns>

    public Question getRandomQuestion()
    {
        // Unlikely scenario: Player has answered all questions in the question bank
        if (responses.Count == questions.Count)
        {
            return null;
        }

        // Randomize and return appropriate question
        int tempQid = -1;
        int temp = -1;

        while (tempQid == -1 || responses.ContainsKey(tempQid.ToString())) {
            temp = UnityEngine.Random.Range(0, questions.Count);
            tempQid = questions[temp].ID;
        }

        Question chosen = questions[temp];
        chosen.timeLimit = getTimeLimit();
        return chosen;
    }


    /// <summary>
    /// This function is called by each player upon completing a question response, for logging purposes.
    /// </summary>
    /// <param name="questionNum"></param>
    /// <param name="resp"></param>

    public void recordResponse(int questionNum, int resp)
    {
        responses.Add(questionNum.ToString(), resp);
    }

    /// <summary>
    /// This function is called to obtain all the player's responses, for uploading into the database when the game ends
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, int> getResponses()
    {
        return responses;
    }

    /// <summary>
    /// This function is called to return the time limit for each question based on its difficulty.
    /// </summary>
    /// <returns></returns>

    public float getTimeLimit()
    {
        switch (Difficulty)
        {
            case 1:
                return 5.0f;
            case 2:
                return 7.0f;
            case 3:
                return 9.0f;
        }
        return 0.0f;
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

}
