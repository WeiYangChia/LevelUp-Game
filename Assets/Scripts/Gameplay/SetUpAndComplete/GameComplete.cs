﻿using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using Newtonsoft.Json;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

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

    // Information regarding all players
    private List<PlayerController> players = new List<PlayerController>();
    private List<Record> records = new List<Record>();

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
        localID = Login.localid;

        // End Gameplay
        initializePlayers();
        stopMoving();

        QM = gameObject.GetComponent<QuestionManager>();
        QM.setEnded();

        // Create Records
        createRecords();

        //display User Avatar:
        AvatarController AC = new AvatarController();
        AC.displayAvatar(avatar, LobbySetUp.LS.playerList[PhotonNetwork.LocalPlayer.NickName]);
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
            for (int i = 1; i < 5; i++)
            {
                GameObject curPlayer = GameObject.FindWithTag("Player" + i.ToString());

                if (curPlayer != null)
                {
                    players.Add(curPlayer.GetComponent<PlayerController>());
                }
                else
                {
                    break;
                }
            }
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
            Record cur = new Record(dateTime,
                QM.Difficulty, QM.Category,
                rankedPlayers[i].GetComponent<PlayerController>().playerName,
                rankedPlayers[i].GetComponent<PlayerController>().getPoints(),
                rank);

            records.Add(cur);

            // If the current player is the local player, upload record (with player responses) to database.
            // Essentially, each player only uploads their own record, to avoid duplicates.
            if (Login.currentUser.username == rankedPlayers[i].GetComponent<PlayerController>().playerName)
            {
                cur.attachResponses(QM.getResponses());

                uploadRecord(cur, rank);
            }

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
        // Upload record
        string urlRecord = "https://quizguyz.firebaseio.com/Users/" + localID+"/Records.json";
        RestClient.Post(url: urlRecord, JsonConvert.SerializeObject(record));

        // Calculate achievement points to be awarded based on rank
        int pointsAwarded = 0;
        rankDisplay.text = (rank).ToString();
        switch (rank)
        {
            case 1:
                pointsAwarded = 10;
                break;
            case 2:
                pointsAwarded = 7;
                break;
            case 3:
                pointsAwarded = 4;
                break;
            case 4:
                pointsAwarded = 1;
                break;
        }
        pointsAwarded *= QM.Difficulty;

        updateAchievementPoints(pointsAwarded);

    }

    /// <summary>
    /// This function is called to update the local player's achievement points based on rank, in the database
    /// It also allows the UI to display the achievement points progress of the player
    /// </summary>
    /// <param name="achievementPoints"></param>

    void updateAchievementPoints(int achievementPoints)
    {
        Achievement playerinfo = new Achievement();
        string playerurl = "https://quizguyz.firebaseio.com/Users/" + localID;
        RestClient.Get(url: playerurl + ".json").Then(onResolved: response =>
        {
            pointInc.text = "+" + achievementPoints.ToString();

            //Get
            playerinfo = JsonConvert.DeserializeObject<Achievement>(response.Text);
            prevPoints.text = playerinfo.achievementPoints.ToString();
            prevBar.GetComponent<Image>().fillAmount = ((float)playerinfo.achievementPoints / 750);

            //Update
            playerinfo.achievementPoints = playerinfo.achievementPoints + achievementPoints;
            afterPoints.text = playerinfo.achievementPoints.ToString();
            afterBar.GetComponent<Image>().fillAmount = ((float)playerinfo.achievementPoints / 750);

            //Upload
            RestClient.Put(url: playerurl + "/achievementPoints.json", JsonConvert.SerializeObject(playerinfo.achievementPoints));

            if (playerinfo.achievementPoints >= 250)
            {
                Bronze.SetActive(true);
            }
            if (playerinfo.achievementPoints >= 500)
            {
                Silver.SetActive(true);
            }
            if (playerinfo.achievementPoints >= 750)
            {
                Gold.SetActive(true);
            }
        });
        
    }

    /// <summary>
    /// This function is called to display all game results on the UI at the end of the game, but only after all records are processed.
    /// </summary>

    void displayResults()
    {
        highscoreDisplayUpdated = true;
        ResultsPage.GetComponent<HighscoreTable>().enabled = true;
        ResultsPage.GetComponent<HighscoreTable>().endGameUpdateTable(records);
        ResultsPage.SetActive(true);
    }
}

