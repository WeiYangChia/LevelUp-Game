using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script determines which arena to activate, based on the host's previous input.
/// </summary>

public class ArenaController : MonoBehaviour
{
    // Stores list of map gameObjects and the map choice
    public GameObject[] maps;
    public int activeMapIndex;

    
    /// <summary>
    /// This function is called by the Game Setup script at the start of the game, to activate the correct arena based on mapIndex selected.
    /// </summary>
    /// <param name="mapIndex"></param>

    public void setUpMap(int mapIndex)
    {
        if (mapIndex< maps.Length && mapIndex >= 0)
        {
            activeMapIndex = mapIndex;
        }
        else
        {
            activeMapIndex = 0;
        }
        print(activeMapIndex);

        // ensures all other maps are inactive
        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].SetActive(false);
        }

        // sets chosen map as active
        maps[activeMapIndex].SetActive(true);
    }
}