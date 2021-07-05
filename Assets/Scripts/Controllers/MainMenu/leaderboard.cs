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

    public int ShowCat = 0;
    public string[] catList = { "TR", "RT", "TX", "FL" };
    int count;
    public List<GameObject> test;

    private void Start()
    {
        HighscoreUpdates();

    }
    public void HighscoreUpdates()
    {
        string urlRecord = "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Highscore/Matrix Reasoning/" + catList[ShowCat] + "/LO1.json";

        // API to retrieve the players information
        RestClient.Get(url: urlRecord).Then(onResolved: response =>
        {
            Dictionary<string, Dictionary<string, int>> Highscores = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(response.Text);
            int count = 0;
            foreach (KeyValuePair<string, Dictionary<string, int>> kv in Highscores) // loop through both
            {
                foreach (KeyValuePair<string, int> kv2 in kv.Value)
                {
                    RankPlayer[count].text = kv2.Key.ToString();
                    PlayerScore[count].text = kv2.Value.ToString();
                    count += 1;
                }
                if (count == 7) { break; };
            }
        });

    }
    public void buttonClick()
    {
        foreach (GameObject temp in test)
        {
            temp.SetActive(false);
        }
        test[ShowCat].SetActive(true);
    }
}
