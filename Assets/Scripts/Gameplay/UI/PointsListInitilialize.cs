using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles the points UI during the gameplay.
/// </summary>
public class PointsListInitilialize : MonoBehaviour
{
    // UI of the points dispay
    public GameObject[] borders;
    public GameObject[] playerBoxes;
    public Material blue;
    public Material pink;
    public Material green;
    public Material yellow;
    public Material purple;
    public Material brown;

    public Dictionary<string, int> playerList;

    // Start is called before the first frame update
    void Start()
    {
        // playerList = LobbySetUp.LS.playerList;

        // int i = 0;
        // foreach (KeyValuePair<string, int> player in playerList)
        // {

        //     setUpBoxColor(i, player.Value);

        //     if (player.Key == Login.currentUser.username)
        //     {
        //         setUpHighlight(i);
        //     }

        //     i++;
        // }
    }

    /// <summary>
    /// Sets the color of up box based on the colour of the player.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="choice">The choice.</param>
    // void setUpBoxColor(int index, int choice)
    // {
    //     playerBoxes[index].SetActive(true);
    //     int color = choice % 10;
    //     Color chosen = Color.white;

    //     // Sets up the colour of the points box based on the user's colour.
    //     switch (color)
    //     {
    //         case 1:
    //             chosen = blue.color;
    //             break;

    //         case 2:
    //             chosen = pink.color;
    //             break;

    //         case 3:
    //             chosen = green.color;
    //             break;

    //         case 4:
    //             chosen = yellow.color;
    //             break;

    //         case 5:
    //             chosen = purple.color;
    //             break;

    //         case 6:
    //             chosen = brown.color;
    //             break;
    //     }

    //     playerBoxes[index].GetComponent<Image>().color = chosen;
    // }

    /// <summary>
    /// Highlights the box of each user based on their colour.
    /// </summary>
    /// <param name="index">The index.</param>
    // void setUpHighlight(int index)
    // {
    //     borders[index].SetActive(true);
    // }
}
