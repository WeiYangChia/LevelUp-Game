using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;
using Proyecto26;
using Newtonsoft.Json;

/// <summary>
/// This class handles the leaderboard UI and logic displayed in the main menu 
/// </summary>
public class leaderboard : MonoBehaviour
{
    // Names of the top 8 players 
    public List<TextMeshProUGUI> RankPlayer;
    public List<TextMeshProUGUI> PlayerScore;


    // List of all the players information in the database
    public List<Achievement> playerinfo = new List<Achievement>();




    private void Start()
    {
        string QuestionUrl = "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Highscore.json";
        int count = 0;

        // API to retrieve the players information
        RestClient.Get(url: QuestionUrl).Then(onResolved: response =>
        {
            // Retrieving all the players information
            Dictionary<string, int> entryDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(response.Text);

            foreach (KeyValuePair<string, int> kv in entryDict)
            {
                if (count < 8)
                {
                    RankPlayer[count].text = kv.Key;
                    PlayerScore[count].text = kv.Value.ToString();
                    count += 1;
                }
                
            }

        });

    }

}
