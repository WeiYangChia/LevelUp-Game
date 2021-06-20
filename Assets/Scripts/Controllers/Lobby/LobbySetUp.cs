using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class LobbySetUp : MonoBehaviour
{
    // Creates a singleton class
    public static LobbySetUp LS;

    // Platform UI objects in the room
    public Image[] platforms;

    // Avatar UI objects
    public Image[] Avatars;
    public List<Image> CurrentAvatars = new List<Image>();

    // Name UI objects
    public TextMeshProUGUI[] Names;
    public List<TextMeshProUGUI> CurrentNames = new List<TextMeshProUGUI>();

    // Player List that stores data of the players in the room
    public int playerData;

    // Category and difficulty variables
    public int category;
    public int difficulty;

    // Map index
    public int mapIndex;

    private void OnEnable()
    {
        if (LobbySetUp.LS == null)
        {
            LobbySetUp.LS = this;
        }
    }
}
