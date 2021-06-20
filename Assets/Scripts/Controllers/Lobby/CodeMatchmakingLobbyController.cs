using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
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

    // Code display
    [SerializeField]
    private TextMeshProUGUI codeDisplay;

    // Join/Create buttons
    [SerializeField]
    private Button Create;

    // Category buttons
    public Button translation;
    public Button rotation;
    public Button texturing;
    public Button flipping;

    private List<Button> buttonsCat = new List<Button>();
    private bool catChosen = false;
    public static int cat;

    // Difficulty buttons
    public Button easy;
    public Button medium;
    public Button hard;

    private List<Button> buttonsDiff = new List<Button>();
    private bool diffChosen = false;
    public static int diff;

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
    }

    /// <summary>
    /// Update is called every frame
    /// Checks if room options are valid and a room can be created
    /// Handles error message when player fails to join a room
    /// </summary>
    private void Update()
    {
        // If category and difficulty is chosen and a valid room size is entered,
        // set create button to interactable so it can be clicked
        if (catChosen && diffChosen)
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
    /// Initialise the Difficulty and Category buttons in the Create/Join Room screen
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

        buttonsDiff.Add(easy);
        buttonsDiff.Add(medium);
        buttonsDiff.Add(hard);


        for (int i = 0; i < buttonsDiff.Count; i++)
        {
            int index = i;
            buttonsDiff[i].onClick.AddListener(delegate { DiffClicked(index); });
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

    /// <summary>
    /// When a difficuly is selected, the other difficulty levels cannot be selected.
    /// To select another difficulty level, the player must click on the current one to deselect it first.
    /// </summary>
    /// <param name="index"></param>
    void DiffClicked(int index)
    {
        if (!diffChosen)
        {
            for (int i = 0; i < buttonsDiff.Count; i++)
            {
                if (i != index)
                {
                    buttonsDiff[i].interactable = false;
                }
            }
            diff = index + 1;
            diffChosen = true;
        }

        else
        {
            for (int i = 0; i < buttonsDiff.Count; i++)
            {
                if (i != index)
                {
                    buttonsDiff[i].interactable = true;
                }
            }
            diff = -1;
            diffChosen = false;
        }
        LobbySetUp.LS.difficulty = diff;
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
