using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using Proyecto26;
using Newtonsoft.Json;
using Photon.Realtime;

/// <summary>
/// This script processes all logic related to avatar selection of the player in the room
/// </summary>
public class AvatarController : MonoBehaviour
{

    // Panels
    public GameObject RoomPanel;
    public GameObject AvatarPanel;

    // Character toggles
    public Toggle char1;
    public Toggle char2;
    public Toggle char3;

    public GameObject blockChar2;
    public GameObject blockChar3;


    private List<Toggle> toggles = new List<Toggle>();

    // Color buttons
    public Button blue;
    public Button pink;
    public Button green;
    public Button yellow;
    public Button purple;
    public Button brown;

    private List<Button> buttons = new List<Button>();

    // Control buttons
    public Button confirm;
    public Button customize;


    // Current User Info

    private int curSelection;
    public Image curAvatar;
    private bool colorSelected = false;
    private bool charSelected = false;

    // Main Room Avatar

    public TextMeshProUGUI Name;
    public Image mainAvatar;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        InitializeButtons();
        InitializeToggles(600);

        // Connects to the database
        Achievement playerinfo = new Achievement();
        string playerurl = "https://quizguyz.firebaseio.com/Users/" + Login.localid;

        //RestClient.Get(url: playerurl + ".json").Then(onResolved: response =>
        //{
        //    //Get information of each player (Achievement points)
        //    playerinfo = JsonConvert.DeserializeObject<Achievement>(response.Text);

        //    //Initialise the buttons and toggles
        //    InitializeButtons();
        //    InitializeToggles(playerinfo.achievementPoints);

        //});
    }

    /// <summary>
    /// This function is called every every frame
    /// Checks if the user has selected a valid combination of character + color
    /// </summary>
    private void Update()
    {
        if (selectionValid(curSelection))
        {
            confirm.interactable = true;
        }
        else
        {
            confirm.interactable = false;
        }
    }

    /// <summary>
    /// Updates the avatar selected by the player (Character + color)
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="picIndex"></param>

    public void updateAvatar()
    {
        try{
            Name.SetText(Login.currentUser.username);
        }
        catch(Exception e){
            Name.SetText("Test Mode");
        }
        
        displayAvatar(mainAvatar, curSelection);
        LobbySetUp.LS.playerData = curSelection;
    }


    /// <summary>
    /// Displays avatar based on the player's selection
    /// </summary>
    /// <param name="avatar"></param>
    /// <param name="selection"></param>
    public void displayAvatar(Image avatar, int selection)
    {
        String avatarPath = findAvatarPath(selection);

        if (avatarPath.Contains("Unknown"))
        {
            avatar.rectTransform.localPosition = new Vector3(0, 7.4f, 0);
            avatar.rectTransform.sizeDelta = new Vector2(10, 12);
            avatar.color = Color.white;
        }

        else if (avatarPath.Contains("Mummy"))
        {
            avatar.rectTransform.localPosition = new Vector3(0, 7.4f, 0);
            avatar.rectTransform.sizeDelta = new Vector2(15, 18);
            avatar.color = new Color(0.8f, 0.8f, 0.8f);
        }
        else if (avatarPath.Contains("Astronaut"))
        {
            avatar.rectTransform.localPosition = new Vector3(0, 7.4f, 0);
            avatar.rectTransform.sizeDelta = new Vector2(17, 22);
            avatar.color = new Color(0.92f, 0.92f, 0.92f);
        }
        else if (avatarPath.Contains("Ball"))
        {
            avatar.rectTransform.localPosition = new Vector3(0, 5.5f, 0);
            avatar.rectTransform.sizeDelta = new Vector2(14, 18);
            avatar.color = new Color(0.85f, 0.85f, 0.85f);
        }

        avatar.sprite = Resources.Load<Sprite>(avatarPath);
    }

    
    /// <summary>
    /// Activated when the player clicks on the 'Customize' button
    /// </summary>
    public void CustomizeCharacterOnClick()
    {
        // Brings the player to the avatar panel
        RoomPanel.SetActive(false);
        AvatarPanel.SetActive(true);
    }

    // / <summary>
    // / Activated when the player confirms the character chosen
    // / </summary>
    public void ConfirmCharacterOnClick()
    {
        AvatarPanel.SetActive(false);
        RoomPanel.SetActive(true);

            // Updates the avatar selected by the player for all clients in the room
        updateAvatar();
    }


    /// <summary>
    /// This initialises the buttons of the colours that can be selected by the player
    /// </summary>
    private void InitializeButtons()
    {
        buttons.Add(blue);
        buttons.Add(pink);
        buttons.Add(green);
        buttons.Add(yellow);
        buttons.Add(purple);
        buttons.Add(brown);

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;

            buttons[i].onClick.AddListener(delegate { ColorClicked(index); });
        }
    }

    /// <summary>
    /// Initialises the character toggles for the player to select
    /// The characters that can be selected by the player depends on the achivement points the player has
    /// </summary>
    /// <param name="points"></param>
    private void InitializeToggles(int points)
    {
        if (points < 250)
        {
            toggles.Add(char1);
            disableToggle(2);
            disableToggle(3);
        }
        else if (points < 500)
        {
            toggles.Add(char1);
            toggles.Add(char2);

            disableToggle(3);
        }
        else
        {
            toggles.Add(char1);
            toggles.Add(char2);
            toggles.Add(char3);
        }
        

        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i;

            toggles[i].onValueChanged.AddListener(delegate { CharClicked(index); });
        }
    }

    /// <summary>
    /// Blocks the characters that cannot be chosen by the player
    /// </summary>
    /// <param name="index"></param>
    private void disableToggle(int index)
    {
        if (index == 2)
        {
            blockChar2.SetActive(true);
            char2.interactable = false;
        }
        else if (index == 3)
        {
            blockChar3.SetActive(true);
            char3.interactable = false;
        }
        
    }

    /// <summary>
    /// Activated when the player clicks on a character
    /// </summary>
    /// <param name="index"></param>
    void CharClicked(int index)
    {
        if (!charSelected)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (i != index)
                {
                    toggles[i].interactable = false;
                }
            }
            curSelection += ((index + 1) * 10);
            charSelected = true;
        }

        else
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (i != index)
                {
                    toggles[i].interactable = true;
                }
            }
            curSelection -= ((index+1) * 10);
            charSelected = false;
        }
        displayAvatar(curAvatar, curSelection);
    }

    /// <summary>
    /// Activated when the player clicks on a color
    /// </summary>
    /// <param name="index"></param>
    void ColorClicked(int index)
    {
        // When the player selects a color
        if (!colorSelected)
        {           
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i != index)
                {
                    buttons[i].interactable = false;
                }
            }

            curSelection += (index + 1);
            colorSelected = true;
        }

        // When the player deselects a color
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].interactable = true;
            }

            curSelection -= (index + 1);
            colorSelected = false;
        }

        displayAvatar(curAvatar, curSelection);
    }

    /// <summary>
    /// Sets the avatar path based on the colour and character they chose
    /// </summary>
    /// <param name="selection"></param>
    /// <returns></returns>
    private string findAvatarPath(int selection)
    {
        if (selection == -1)
        {
            return "Avatars/Unknown";
        }

        int chosenCharacter = -1;
        int chosenColor = -1;

        string avatarPath = "Avatars/";

        chosenCharacter = selection / 10;
        chosenColor = selection % 10;

        switch (chosenCharacter)
        {
            case 1:
                avatarPath += "Mummy_";
                break;

            case 2:
                avatarPath += "Astronaut_";
                break;

            case 3:
                avatarPath += "Ball_";
                break;

            default:
                return "Avatars/Unknown";
        }

        switch (chosenColor)
        {
            case 1:
                avatarPath += "Blue";
                break;

            case 2:
                avatarPath += "Pink";
                break;

            case 3:
                avatarPath += "Green";
                break;

            case 4:
                avatarPath += "Yellow";
                break;

            case 5:
                avatarPath += "Purple";
                break;

            case 6:
                avatarPath += "Brown";
                break;

            default:
                return "Avatars/Unknown";
        }

        return avatarPath;
    }

    /// <summary>
    /// Checks if selection is valid (character + color)
    /// </summary>
    /// <param name="sel"></param>
    /// <returns></returns>
    private bool selectionValid(int sel)
    {
        // sel/10 represents the chosen character, sel%10 represents the chosen color
        if ((sel/10)>=1 && (sel % 10) >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
