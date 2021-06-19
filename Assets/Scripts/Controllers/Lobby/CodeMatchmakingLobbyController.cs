using Photon.Pun;
using Photon.Realtime;
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
public class CodeMatchmakingLobbyController : MonoBehaviourPunCallbacks
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

    private string joinCode = null;
    private string roomName;
    private int roomSize = -1;

    // Join/Create buttons
    [SerializeField]
    private Button Join;
    [SerializeField]
    private Button Create;

    // Category buttons
    public Button math;
    public Button science;
    public Button geog;
    public Button general;

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
    }

    /// <summary>
    /// Update is called every frame
    /// Checks if room options are valid and a room can be created
    /// Checks if there is room code input and the player can attempt to join a room
    /// Handles error message when player fails to join a room
    /// </summary>
    private void Update()
    {
        // If category and difficulty is chosen and a valid room size is entered,
        // set create button to interactable so it can be clicked
        if (catChosen && diffChosen && roomSize > 1 && roomSize <= 4)
        {
            Create.interactable = true;
        }
        else
        {
            Create.interactable = false;
        }

        // If room code is not null or empty, set join button to interactable so it can be clicked
        if (joinCode != null && joinCode != "")
        {
            Join.interactable = true;
        }
        else
        {
            Join.interactable = false;
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
        buttonsCat.Add(math);
        buttonsCat.Add(science);
        buttonsCat.Add(geog);
        buttonsCat.Add(general);

        for (int i = 0; i < buttonsCat.Count; i++)
        {
            int index = i;
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


    /// <summary>
    /// This is a callback function provided in the MonoBehaviourPunCallbacks class  provided by PUN 2 
    /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // This defines if all clients in a room should automatically load the same level as the Master Client.
        PhotonNetwork.AutomaticallySyncScene = true;

        MainPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        InitializeButtons();

        PhotonNetwork.JoinLobby();

        PhotonNetwork.NickName = Login.currentUser.username;
    }

    /// <summary>
    /// Detects when the room size input value changes and updates accordingly
    /// </summary>
    /// <param name="sizeIn"></param>
    public void OnRoomSizeInputChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    /// <summary>
    /// Create a room on click. It generates a 4 digit room code automatically and creates the room with the specified room options.
    /// The room options sets whether the room is visible in the lobby, is open to be joined, and the maximum number of players.
    /// </summary>
    public void CreateRoomOnClick()
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        roomOps.PublishUserId = true;

        int roomCode = Random.Range(1000, 10000);
        roomName = roomCode.ToString();

        roomController.GetComponent<AvatarController>().enabled = true;
        roomController.GetComponent<AvatarController>().maxPlayers = roomSize;
        roomController.GetComponent<AvatarController>().isCreator = true;

        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(true);

        PhotonNetwork.CreateRoom(roomName, roomOps);

        codeDisplay.text = "Code: " + roomName;
    }

    /// <summary>
    /// This is a callback function provided in the MonoBehaviourPunCallbacks class  provided by PUN 2 
    /// A callback for when the room creation failed, which can be due to an existing room with the same room code
    /// This prompts the creation of a room again.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        roomOps.PublishUserId = true;

        int roomCode = Random.Range(1000, 10000);
        roomName = roomCode.ToString();
        PhotonNetwork.CreateRoom(roomName, roomOps);

        codeDisplay.text = roomName;
    }

    /// <summary>
    /// This is a callback function provided in the MonoBehaviourPunCallbacks class provided by PUN 2 
    /// Called when a previous OnJoinRoom call failed on the server
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // Sets the error message when join room fails
        errorMessage.gameObject.SetActive(true);
        errorMessage.text = message;
        timeWhenDisappear = Time.time + timeToAppear;
        Debug.Log(message);
    }

    // This is a function that is called when the code player by the input changes and updates the variable.
    public void OnCodeInputChanged(string code)
    {
        joinCode = code;
    }

    /// <summary>
    /// Joins the room when clicked, with a joinCode
    /// </summary>
    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(joinCode);
        codeDisplay.text = "Code: "+joinCode;
    }

    /// <summary>
    /// Goes back to main menu screen
    /// </summary>
    public void backMainMenuOnClick()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main Menu");
    }
}
