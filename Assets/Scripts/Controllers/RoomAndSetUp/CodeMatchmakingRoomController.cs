using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// This script processes all logic required for players in the room and is assigned to the CodeMatchmakingRoomController game object.
/// it controls the logic of players joining or leaving the room, as well as the logic of the host starting the game or cancelling the room
/// </summary>
public class CodeMatchmakingRoomController : MonoBehaviour
{
    // Player count display
    [SerializeField]
    private TextMeshProUGUI playerCount;

    // Multiplayer scene index
    [SerializeField]
    private int multiplayerSceneIndex;

    // Panels
    [SerializeField]
    private GameObject LobbyPanel;
    [SerializeField]
    private GameObject RoomPanel;

    // Buttons
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private Button leaveButton;
    [SerializeField]
    private Button mapButton;

    /// <summary>
    /// Update is called every frame
    /// </summary>
    public void Update()
    {
        // Determines if the host can start the game
        startButton.interactable = readyToStart();
    }

    /// <summary>
    /// This is called to ensure that all the players in the Room have chosen an avatar, so that the host is able to start.
    /// </summary>
    private bool readyToStart()
    {
        if (LobbySetUp.LS.playerData == -1){
            return false;
        }

        if (MapController.mapIndex == -1){
            return false;
        }

        return true;
    }

    /// <summary>
    /// This function is triggered when the host clicks "Start". It checks that all the players in the room
    /// have all chosen an avatar, before it loads the game.
    /// </summary>
    public void StartGameOnClick()
    {
        if (!readyToStart()){
            return;
        }
        
        SceneManager.LoadScene("MainGame");
    }

    /// <summary>
    /// Cancels the room when clicked, only available for the host. This removes all players from the room.
    /// </summary>
    public void CancelRoomOnClick()
    {
        SceneManager.LoadScene("CodeMatchmakingMenuDemo");
    }
}
