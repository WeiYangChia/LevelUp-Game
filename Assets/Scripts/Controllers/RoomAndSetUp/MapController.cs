using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script processes all logic related to map selection of the host of the room
/// </summary>
public class MapController : MonoBehaviour
{
    // Toggles to select map
    public Toggle map1;
    public Toggle map2;
    public Toggle map3;
    public Toggle map4;

    private List<Toggle> toggles = new List<Toggle>();

    // Variables to store map settings
    private bool mapSelected = false;
    public static int mapIndex = -1;

    // Variables storing the category and level of the room
    public static int category;
    public static int catLevel;

    public TextMeshProUGUI catDisplay;
    public TextMeshProUGUI catLevelDisplay;

    // Stores the UI panels
    public GameObject MapPanel;
    public GameObject RoomPanel;

    // Button to confirm map selected
    public Button ConfirmMap;

    // Image that renders display of map
    public Image MapDisplay;
    public Image MapBorder;

    /// <summary>
    /// Start is called before the first frame update to initialise variables
    /// </summary>
    void Start()
    {
        InitializeToggles();

        resetMap();
    }

    private void setDisplay(){
        string catName = "";
        string catLevelName = "";

        switch(category){
            case 0:
                catName = "Category: Translation";
                break;
            case 1:
                catName = "Category: Rotation";
                break;
            case 2:
                catName = "Category: Texturing";
                break;
            case 3:
                catName = "Category: Flipping";
                break;
            default:
                break;
        }

        catLevelName = "Level: " + catLevel.ToString();

        catDisplay.text = catName;
        catLevelDisplay.text = catLevelName;
    }

    /// <summary>
    ///  Update is called every frame and checks if a map has been selected. 
    ///  If a map has been selected, set the confirm button to be interactable and
    ///  display the selected map
    /// </summary>
    private void Update()
    {
        category = LobbySetUp.LS.category;
        catLevel = LobbySetUp.LS.catLevel;

        setDisplay();

        if (mapSelected)
        {
            ConfirmMap.interactable = true;
        }
        else
        {
            ConfirmMap.interactable = false;
        }
        displaySelectedMap();
    }

    /// <summary>
    /// INitialise the toggles to be able to enable the player to select the map
    /// </summary>
    private void InitializeToggles()
    {
        toggles.Add(map1);
        toggles.Add(map2);
        toggles.Add(map3);
        toggles.Add(map4);

        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i;

            toggles[i].onValueChanged.AddListener(delegate { MapClicked(index); });
        }
    }

    /// <summary>
    /// This function will be called when a map has been selected, which will disable the
    /// other map toggles. To select another map, the player has to click on the current map
    /// toggle (to unselect it) before selecting the other map.
    /// </summary>
    /// <param name="index"></param>
    void MapClicked(int index)
    {
        if (!mapSelected)
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (i != index)
                {
                    toggles[i].interactable = false;
                }
            }

            mapIndex = index;
            mapSelected = true;
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
            mapIndex = -1;
            mapSelected = false;
        }
    }

    /// <summary>
    /// This displays the map panel when the host clicks on the customize map button
    /// </summary>
    public void CustomizeMapOnClick()
    {
        RoomPanel.SetActive(false);
        MapPanel.SetActive(true);
    }

    /// <summary>
    /// After confirming the map, the panels are set to inactive and active respectively to transition back to the main room page
    /// PV.RPC calls the PunRPC function setMapSettings
    /// </summary>
    public void ConfirmMapOnClick()
    {
        MapPanel.SetActive(false);
        RoomPanel.SetActive(true);
        LobbySetUp.LS.mapIndex = mapIndex; 
        displaySelectedMap();
    }


    /// <summary>
    /// This loads the picture of the selected map and displays it
    /// </summary>
    private void displaySelectedMap()
    {
        string mapPath = "";

        if (mapIndex == -1)
        {
            mapPath = "Maps/Unknown";
            MapBorder.gameObject.SetActive(false);
        }
        else
        {
            mapPath = ("Maps/Map" + mapIndex);
            MapBorder.gameObject.SetActive(true);
        }

        MapDisplay.sprite = Resources.Load<Sprite>(mapPath);
    }

    /// <summary>
    /// Resets the map
    /// </summary>
    public void resetMap()
    {
        mapIndex = -1;
    }
}