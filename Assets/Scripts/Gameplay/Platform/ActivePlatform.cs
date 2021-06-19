using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// This script determines the activation of all player blocks and special blocks throughout the entire gameplay.
/// It is assigned to the group of active blocks on each arena.
/// It processes all logic required for the random assignment and reshuffling of blocks in the game;
/// and activates the chosen blocks for their required function.
/// </summary>

public class ActivePlatform : MonoBehaviour
{
    // Active Block Generation Parameters:
    public int cooldownAB = 8;
    bool putActiveBlock = true;

    // Collection of Blocks
    public Dictionary<int, GameObject> blocks = new Dictionary<int, GameObject>();
    int totalNumBlocks;

    // Current State and Record of active/special scripts and blocks
    ActivatedBlock ABscript = null;
    public int curNum = -1;
    public int prevNum = -1;

    // Information on ALL existing players in game:
    public GameObject player;

    /// <summary>
    /// Start is called before first frame update,
    /// to initialize certain settings and paramters (mainly for the arena)
    /// </summary>
    
    void Start()
    {
        totalNumBlocks = transform.childCount;
        for (int i = 0; i < totalNumBlocks; i++)
        {
            blocks.Add(i, transform.GetChild(i).gameObject);
        }
    }
    
    /// <summary>
    /// Update is called every frame, and is the main function used to determine the arena behavior
    /// </summary>

    void Update()
    {
        // Players must be found at every frame, to avoid errors in cases where players leave mid game.
        // If a player ceases to exist, his blocks will no longer appear.
        player = GameObject.FindWithTag("Player");


        // Updates blocks list if the previously assigned block has fallen/been destroyed. If not, it can be deactivated.
        if (blocks.ContainsKey(prevNum))
        {
            if (blocks[prevNum] == null)
            {
                blocks.Remove(prevNum);
            }
        }

        if (ABscript != null && ABscript.droppingBlock){
            putActiveBlock = true;
        }

        // Conducts randomization of blocks for the local player.
        // Using this, each player in the game will calculate the block on the arena to be assigned to them.
        // After it is calculated, a network event is called to ensure all players see the same block assigned to the particular player.

        // Player:
        if (player != null &&
            putActiveBlock && blocks.Count > totalNumBlocks / 4 && player.GetComponent<PlayerController>().moveable)
        {
            // Deactivate previous block if it has not been destroyed
            if (blocks.ContainsKey(prevNum))
            {
                DeactivateBlock();
            }

            // Block randomization and activation
            curNum = -1;
            int temp;

            while (curNum == -1)
            {
                temp = Random.Range(0, totalNumBlocks);

                if (blocks.ContainsKey(temp)
                    && temp != prevNum)
                    // && temp != power2Num && temp != power1Num && temp != power0Num)
                {
                    ActivateBlock(temp);
                }
            }
        }
    }

    /// <summary>
    /// This function is called to trigger a network event where a chosen block is activated for a particular player
    /// </summary>
    /// <param name="blockIndex"></param>


    void ActivateBlock(int blockIndex)
    {

        GameObject block = blocks[blockIndex];

        // Activate Block based on blockIndex

        curNum = blockIndex;

        ABscript = block.transform.GetChild(0).gameObject.GetComponent<ActivatedBlock>();
        ABscript.colorIndex = player.GetComponent<PlayerController>().colorIndex;
        ABscript.enabled = true;

        ABscript.blockActivated = true;

        putActiveBlock = false;

        // Initiate cooldown to determine when a certain player should be assigned a new block
        StartCoroutine(CooldownAB());

    }


    /// <summary>
    /// This function is called to trigger a network event to deactivate a particular player's block
    /// </summary>

    void DeactivateBlock()
    {
        ActivatedBlock curABScript = null;

        curABScript = ABscript;

        curABScript.blockActivated = false;
        curABScript.enabled = false;
    }

    /// <summary>
    /// This function is used to track the time since a block has been activated for a particular user.
    /// If the specified time is up, then a new block should be assigned to the user.
    /// </summary>
    /// <returns></returns>
    IEnumerator CooldownAB()
    {
        int counter = cooldownAB;

        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;

            if (counter < 1)
            {
                prevNum = curNum;
                putActiveBlock = true;
            }
        }
    }
}
