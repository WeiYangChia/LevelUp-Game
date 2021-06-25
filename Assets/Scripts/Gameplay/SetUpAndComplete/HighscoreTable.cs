using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the logic behind the highscore table UI at the end of every game.
/// </summary>
public class HighscoreTable : MonoBehaviour
{
    // Parameters for the entries in the highscore table
    public Transform entryContainer;
    public Transform entryTemplate;
    float templateHeight = 30f;

    /// <summary>
    /// Updates the table at the end of the game.
    /// </summary>
    /// <param name="records">The records.</param>
    public void endGameUpdateTable(Dictionary<string, Dictionary<string, int>> records)
    {
        int count = 1;
        foreach (KeyValuePair<string, Dictionary<string, int>> kv in records) // loop through both
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector3(-53, 10 - (templateHeight * (count-1)), 0);
            entryTransform.gameObject.SetActive(true);

            entryTransform.Find("Rank").GetComponent<TextMeshProUGUI>().text = count.ToString();
            foreach (KeyValuePair<string, int> kv2 in kv.Value)
            {
                entryTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = kv2.Key.ToString();
                entryTransform.Find("Points").GetComponent<TextMeshProUGUI>().text = kv2.Value.ToString();

            }
            if (count == 8) { break; };
        }

    }

    /// <summary>
    /// Called when the player clicks 'Done' on the highscore table UI.
    /// </summary>
    public void OnClickEnd()
    {
        SceneManager.LoadScene("Main Menu");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }
}
