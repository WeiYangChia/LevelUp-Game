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

    // Player List that stores data of the players in the room
    public int playerData;

    // Category and difficulty variables
    public int category;
    public int catLevel;

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
