using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// This class handles the logic of the main menu UI.
/// </summary>
public class MainMenu : MonoBehaviour
{
    // Parameters of the different panels in the main menu page 
    public GameObject MenuPanel;
    public GameObject ProfilePanel;
    public GameObject LeaderboardPanel;
    public GameObject AchievementsPanel;
    public GameObject InstructionPanel;

    /// <summary>
    /// Loads the scene if the player chooses the play button.
    /// </summary>
    public void playGame()
    {
        SceneManager.LoadScene("CodeMatchMakingMenuDemo");
    }
    /// <summary>
    /// Opens the profile when the player clicks the profile button.
    /// </summary>
    public void OpenProfileOnClick()
    {
        MenuPanel.SetActive(false);
        ProfilePanel.SetActive(true);
    }
    /// <summary>
    /// Closes the profile when the player clicks the back button.
    /// </summary>
    public void CloseProfileOnClick()
    {
        ProfilePanel.SetActive(false);
        MenuPanel.SetActive(true);
    }
    /// <summary>
    /// Opens the leader boarde when the player clicks the leaderboard button.
    /// </summary>
    public void OpenLeaderBoardeOnClick()
    {
        MenuPanel.SetActive(false);
        LeaderboardPanel.SetActive(true);
    }
    /// <summary>
    /// Closes the leader board when the player clicks the back button.
    /// </summary>
    public void CloseLeaderBoardOnClick()
    {
        LeaderboardPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }
    /// <summary>
    /// Opens the achievements when the player clicks the achievement button.
    /// </summary>
    public void OpenAchievementsOnClick()
    {
        MenuPanel.SetActive(false);
        AchievementsPanel.SetActive(true);
    }
    /// <summary>
    /// Closes the achievements when the player clicks the back button.
    /// </summary>
    public void CloseAchievementsOnClick()
    {
        AchievementsPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }
    /// <summary>
    /// Opens the instructions when the player click the Instruction button.
    /// </summary>
    public void OpenInstructionsOnClick()
    {
        SceneManager.LoadScene("Tutorial");
        
        // InstructionPanel.SetActive(true);
        // MenuPanel.SetActive(false);
    }
    /// <summary>
    /// Closes the instructions when the player clicks the back button.
    /// </summary>
    // public void CloseInstructionsOnClick()
    // {
    //     InstructionPanel.SetActive(false);
    //     MenuPanel.SetActive(true);
    // }

}
