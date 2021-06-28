using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;


/// <summary>
/// This script processes all logic related to the lobby.
/// It is assigned to the CodeMatchmakingLobbyController game object.
/// It controls the logic of players creating a room or joining a room from the lobby.
/// </summary>
public class CodeMatchmakingLobbyController : MonoBehaviour
{
    public GameObject roomController;

    // Panels
    [SerializeField]
    private GameObject LobbyPanel;
    [SerializeField]
    private GameObject MainPanel;
    [SerializeField]
    private GameObject RoomPanel;

    // Join/Create buttons
    [SerializeField]
    private Button Create;

    // Category buttons
    public Button translation;
    public Button rotation;
    public Button texturing;
    public Button flipping;

    private List<int> levels = new List<int>();

    private List<TextMeshProUGUI> levels_UI = new List<TextMeshProUGUI>();
    
    public TextMeshProUGUI translation_level_UI;
    public TextMeshProUGUI rotation_level_UI;
    public TextMeshProUGUI texturing_level_UI;
    public TextMeshProUGUI flipping_level_UI;

    private List<Button> buttonsCat = new List<Button>();
    private bool catChosen = false;
    public static int cat;

    // Error message text
    [SerializeField]
    private TextMeshProUGUI errorMessage;

    // Error message times
    private float timeToAppear = 2f;
    private float timeWhenDisappear;

    /// <summary>
    /// Start is called before first frame update,
    /// Sets the LobbyPanel to inactive
    /// </summary>
    private void Start()
    {
        LobbyPanel.SetActive(true);
        InitializeButtons();

        InitializePlayerLevels();
    }

    /// <summary>
    /// Update is called every frame
    /// Checks if room options are valid and a room can be created
    /// Handles error message when player fails to join a room
    /// </summary>
    private void Update()
    {
        // If category is chosen and a valid room size is entered,
        // set create button to interactable so it can be clicked
        if (catChosen)
        {
            Create.interactable = true;
        }
        else
        {
            Create.interactable = false;
        }

        // Checks if the error message is currently showing and has not been shown for more than 2 seconds
        if (errorMessage.gameObject.activeSelf && (Time.time >= timeWhenDisappear))
        {
            // Disable the text so it is hidden
            errorMessage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Initialise the Category buttons in the Create/Join Room screen
    /// </summary>
    private void InitializeButtons()
    {
        buttonsCat.Add(translation);
        buttonsCat.Add(rotation);
        buttonsCat.Add(texturing);
        buttonsCat.Add(flipping);

        for (int i = 0; i < buttonsCat.Count; i++)
        {
            int index = i;
            print(index);
            buttonsCat[i].onClick.AddListener(delegate { CatClicked(index); });
        }
    }

    private void InitializePlayerLevels(){
        // Get player information from DB

        int translation_level = 1;
        int rotation_level = 3;
        int texturing_level = 2;
        int flipping_level = 5;

        levels.Add(translation_level);
        levels.Add(rotation_level);
        levels.Add(texturing_level);
        levels.Add(flipping_level);

        levels_UI.Add(translation_level_UI);
        levels_UI.Add(rotation_level_UI);
        levels_UI.Add(texturing_level_UI);
        levels_UI.Add(flipping_level_UI);

        for (int i = 0; i < levels.Count; i++){
            levels_UI[i].text = "Level " + levels[i].ToString();
        }
    }

    /// <summary>
    /// When a category is selected,  the other categories cannot be selected.
    /// To select another category, the player must click on the current one to deselect it first.
    /// </summary>
    /// <param name="index"></param>
    void CatClicked(int index)
    {
        print("Hello here");
        if (!catChosen)
        {
            for (int i = 0; i < buttonsCat.Count; i++)
            {
                if (i != index)
                {
                    buttonsCat[i].interactable = false;
                }
            }
            cat = index;
            catChosen = true;

            LobbySetUp.LS.catLevel = levels[cat];
        }

        else
        {
            for (int i = 0; i < buttonsCat.Count; i++)
            {
                if (i != index)
                {
                    buttonsCat[i].interactable = true;
                }
            }
            cat = -1;
            catChosen = false;
        }
        
        LobbySetUp.LS.category = cat;
    }

    public void goToRoom(){
        LobbySetUp.LS.playerData = -1;
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(true);
    }

    /// <summary>
    /// Goes back to main menu screen
    /// </summary>
    public void backMainMenuOnClick()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
