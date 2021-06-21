using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using Newtonsoft.Json;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;
using System.Linq;

/// <summary>
/// This script is used to determine the required processing when the game ends.
/// It consolidates all player points and responses;
/// It creates records for each player and ranks them, to be displayed by the game results table
/// It also uploads all records and updates player points at the end of the game
/// </summary>
public class GameComplete : MonoBehaviour
{
    // boolean to track whether player information is initialized
    bool initialized = false;

    // tracks local player
    public static string localID;
    public static string localName;
    // Information regarding all players
    private List<PlayerController> players = new List<PlayerController>();
    private List<Record> records = new List<Record>();
    public Dictionary<string, int> Highscores = new Dictionary<string,int>();
    Record cur;

    // Question Manager for responses
    private QuestionManager QM;

    // Parameters related to game results UI
    public GameObject ResultsPage;
    public bool rankProcessed = false;
    bool highscoreDisplayUpdated = false;


    //UI elements:
    public Image avatar;
    public TextMeshProUGUI rankDisplay;
    public TextMeshProUGUI prevPoints;
    public TextMeshProUGUI pointInc;
    public TextMeshProUGUI afterPoints;

    public GameObject Bronze;
    public GameObject Silver;
    public GameObject Gold;

    public GameObject prevBar;
    public GameObject afterBar;


    /// <summary>
    /// Start function is called when the game ends, to immobilize all players and process their records.
    /// </summary>
    void Start()
    {
        try
        {
            localID = Login.localid;
            localName = Login.username;
        }
        catch (Exception e)
        {
            localID = "s0TESMzP9iaS4o0n4r30BIQJC3u2";
            localName = "uiyot";
        }


        // End Gameplay
        initializePlayers();
        stopMoving();

        QM = gameObject.GetComponent<QuestionManager>();
        QM.setEnded();

        // Create Records
        createRecords();

        //display User Avatar:
        //AvatarController AC = new AvatarController();
        //AC.displayAvatar(avatar, LobbySetUp.LS.playerList[PhotonNetwork.LocalPlayer.NickName]);
        StartCoroutine(Delay(5));
    }

    /// <summary>
    /// Update is called once per frame, and updates the UI (one time only) once the required records are processed
    /// </summary>

    private void Update()
    {
        if (rankProcessed && !highscoreDisplayUpdated)
        {
            // Display Ranking UI
            displayResults();
        }
    }


    /// <summary>
    /// This function is called to obtain information regarding all players in the game
    /// </summary>

    void initializePlayers()
    {
        if (!initialized)
        {
            players.Add(GameObject.FindWithTag("Player").GetComponent<PlayerController>());
            initialized = true;
        }
    }

    /// <summary>
    /// This function is called to immobilize all players at the end of the game
    /// </summary>

    public void stopMoving()
    {
            initializePlayers();
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<PlayerController>().moveable = false;
          
            }
    }

    /// <summary>
    /// This function is called to process the attributes and repsonses of each player, and create the necessary records
    /// It also ranks the records (including cases where there are ties), for further processing and displaying
    /// </summary>

    void createRecords()
    {
        // Sort players by descending points
        List<PlayerController> rankedPlayers = players.OrderByDescending(o => o.points).ToList();

        // Obtain system time for records
        string dateTime = System.DateTime.Now.ToString("MM\\/dd\\/yyyy h\\:mm tt");

        // Calculate rankings of players (to account for ties)
        bool tie = false;
        int prevPoints = -1;
        int offset = 0;
        int rank;

        for (int i = 0; i < rankedPlayers.Count; i++)
        {
            if (i > 0 && rankedPlayers[i].GetComponent<PlayerController>().getPoints() == prevPoints)
            {
                tie = true;
                offset++;
            }

            else
            {
                tie = false;
                offset = 0;
            }

            if (tie)
            {
                rank = i - offset + 1;
            }
            else
            {
                rank = i + 1;
            }

            // Create record for players storing all game related information
            cur = new Record(dateTime,
                QM.Difficulty, QM.Category,
                rankedPlayers[i].GetComponent<PlayerController>().playerName,
                rankedPlayers[i].GetComponent<PlayerController>().getPoints(),
                rank);

            records.Add(cur);
            print(cur);
            Dictionary<string, object> mrqStuff = QM.getMRQ();

            cur.attachResponses(mrqStuff);
            print(JsonConvert.SerializeObject(mrqStuff));
            uploadRecord(cur, rank);
            // If the current player is the local player, upload record (with player responses) to database.
            // Essentially, each player only uploads their own record, to avoid duplicates.
            //if (Login.currentUser.username == rankedPlayers[i].GetComponent<PlayerController>().playerName)
            //{
            //    cur.attachResponses(QM.getResponses());

            //    uploadRecord(cur, rank);
            //}
            uploadScore(cur.Points);
            prevPoints = rankedPlayers[i].GetComponent<PlayerController>().getPoints();
            tie = false;
        }

        rankProcessed = true;
    }

    /// <summary>
    /// This function is called to upload a given record (belonigng to local player) to the database
    /// </summary>
    /// <param name="record"></param>
    /// <param name="rank"></param>

    void uploadRecord(Record record, int rank)
    {

        if (localID == null) { localID = "s0TESMzP9iaS4o0n4r30BIQJC3u2"; };
        print("Uploading record");
        // Upload record
        string urlRecord = "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Users/" + localID+"/Records.json";
        print(JsonConvert.SerializeObject(record));
        RestClient.Post(url: urlRecord, JsonConvert.SerializeObject(record)).Then(onResolved: response => {
            print("Success");
        }).Catch(error => {
            print("Failed");
        });

        // Calculate achievement points to be awarded based on rank
        int pointsAwarded = 0;
        //rankDisplay.text = (rank).ToString();
        
        pointsAwarded *= QM.Difficulty;

        //updateAchievementPoints(pointsAwarded);

    }

    void uploadScore(int amt)
    {
        string urlRecord = "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Users/" + localID + "/Total_Points.json";

        RestClient.Get(url: urlRecord).Then(onResolved: response =>
        {
            int totalPoints = JsonConvert.DeserializeObject<int>(response.Text);
            totalPoints += amt;
            RestClient.Put(url: urlRecord, JsonConvert.SerializeObject(totalPoints)).Then(onResolved: response => {
                print("Success");
            }).Catch(error => {
                print("Failed");
            });
        });
    }

    /// <summary>
    /// This function is called to display all game results on the UI at the end of the game, but only after all records are processed.
    /// </summary>

    void displayResults()
    {
        string urlRecord = "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Highscore.json";
        highscoreDisplayUpdated = true;

        if (localName == null) { localName = "uiyot"; };

        RestClient.Get(url: urlRecord).Then(onResolved: response =>
        {
            print(response.Text);
            Dictionary<string, int> Highscores = JsonConvert.DeserializeObject<Dictionary<string, int>>(response.Text);
            if (cur.Points > Highscores[localName]) { Highscores[localName] = cur.Points; };
            Dictionary<string, int> toSend = Highscores.OrderByDescending(pair => pair.Value).Take(10)
               .ToDictionary(pair => pair.Key, pair => pair.Value);
            RestClient.Put(url: urlRecord, JsonConvert.SerializeObject(toSend));
            ResultsPage.GetComponent<HighscoreTable>().enabled = true;
            ResultsPage.GetComponent<HighscoreTable>().endGameUpdateTable(Highscores);
            ResultsPage.SetActive(true);
            
        }).Catch(error =>
        {
            print("Failed");
        });

    }

    IEnumerator Delay(int time)
    {
        yield return new WaitForSeconds(time);
    }

}

