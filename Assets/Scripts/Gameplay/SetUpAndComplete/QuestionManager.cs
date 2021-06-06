using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;
using System.IO;
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
    public List<string> questions = new List<string>();

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

        string path = "Assets/Resources/Question_Source";
        var info = new DirectoryInfo(path);

        foreach (var file in info.GetDirectories())
        {
            questions.Add(file.Name);
        }
            
    }

    /// <summary>
    /// This function is called by each player to obtain a random question from the question bank that he has not attempted before
    /// </summary>
    /// <returns></returns>

    public Hashtable getRandomQuestion()
    {
        // Unlikely scenario: Player has answered all questions in the question bank
        if (responses.Count == questions.Count)
        {
            return null;
        }

        // Randomize and return appropriate question
        string tempQid = "";
        int temp = -1;

        while (tempQid == "" || responses.ContainsKey(tempQid)) {
            temp = UnityEngine.Random.Range(0, questions.Count);
            tempQid = questions[temp];
            print(questions[temp]);
        }

        Hashtable chosen = new Hashtable();
        chosen.Add("ID",questions[temp]);
        chosen.Add("bonusTimeLimit"  ,getBonusTimeLimit());
        chosen.Add("maxTime", getMaxTime());
        return chosen;
    }


    /// <summary>
    /// This function is called by each player upon completing a question response, for logging purposes.
    /// </summary>
    /// <param name="questionNum"></param>
    /// <param name="resp"></param>

    public void recordResponse(string questionNum, int resp)
    {
        responses.Add(questionNum, resp);
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

}
