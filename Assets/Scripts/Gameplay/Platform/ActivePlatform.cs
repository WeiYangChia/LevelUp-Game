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

    // // Special Block Generation Parameters:
    // public int cooldownSB = 10;
    // bool putSpecialBlock = true;

    // // Powerup Tracking:
    // bool power0used = false;
    // bool power1used = false;
    // bool power2used = false;

    // Collection of Blocks
    public Dictionary<int, GameObject> blocks = new Dictionary<int, GameObject>();
    int totalNumBlocks;

    // Current State and Record of active/special scripts and blocks
    ActivatedBlock ABscript = null;
    public int curNum = -1;
    public int prevNum = -1;

    // SpecialBlock power0Script = null;
    // int power0Num = -1;

    // SpecialBlock power1Script = null;
    // int power1Num = -1;

    // SpecialBlock power2Script = null;
    // int power2Num = -1;

    // int powerChoice = -1;

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

        // // Special Block randomization and activation
        // if (putSpecialBlock && blocks.Count > totalNumBlocks / 4 && !(power0used && power1used && power2used) &&
        //     (player != null && player.GetComponent<PlayerController>().moveable))
        // {

        //     // Block randomization and activation
        //     int newSpecNum = -1;
        //     int temp;

        //     while (newSpecNum == -1)
        //     {
        //         temp = Random.Range(0, totalNumBlocks);

        //         if (blocks.ContainsKey(temp)
        //             && temp != prevNum && temp != curNum
        //             && temp != power1Num && temp != power0Num && temp != power2Num)
        //         {
        //             newSpecNum = temp;

        //             // Determines which powerup to use, depending on which are available
        //             int powerChoice = -1;
        //             if (!power0used && !power1used && !power2used)
        //             {
        //                 powerChoice = Random.Range(0, 3);
        //             }
        //             else
        //             {
        //                 if (power0used)
        //                 {
        //                     if (!power1used && !power2used)
        //                         powerChoice = Random.Range(1, 3);

        //                     else if (power1used)
        //                         powerChoice = 2;

        //                     else if (power2used)
        //                         powerChoice = 1;
        //                 }

        //                 else if (power1used)
        //                 {
        //                     if (!power0used && !power2used)
        //                     {
        //                         powerChoice = Random.Range(0, 2);

        //                         if (powerChoice == 1)
        //                         {
        //                             powerChoice = 2;
        //                         }
        //                     }
                                

        //                     else if (power0used)
        //                         powerChoice = 2;

        //                     else if (power2used)
        //                         powerChoice = 0;
        //                 }

        //                 else if (power2used)
        //                 {
        //                     if (!power0used && !power1used)
        //                         powerChoice = Random.Range(0,2);

        //                     else if (power0used)
        //                         powerChoice = 1;

        //                     else if (power1used)
        //                         powerChoice = 0;
        //                 }
        //             }

        //             SpecialBlock(newSpecNum, powerChoice);
        //         }
        //     }


        // }
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
    /// This function is called to trigger a network event to activate a special block for all players
    /// </summary>
    /// <param name="blockIndex"></param>
    /// <param name="powerChoice"></param>

    // void SpecialBlock(int blockIndex, int powerChoice)
    // {
    //     GameObject block = blocks[blockIndex];

    //     //Activates special block based on block and powerup chosen
    //     if (powerChoice == 0)
    //     {
    //         power0Script = block.transform.GetChild(0).gameObject.GetComponent<SpecialBlock>();
    //         power0Script.choice = powerChoice;
    //         power0Script.blockActivated = true;
    //         power0Script.enabled = true;

    //         power0Num = blockIndex;
    //         power0used = true;
    //     }
    //     else if (powerChoice == 1)
    //     {
    //         power1Script = block.transform.GetChild(0).gameObject.GetComponent<SpecialBlock>();
    //         power1Script.choice = powerChoice;
    //         power1Script.blockActivated = true;
    //         power1Script.enabled = true;

    //         power1Num = blockIndex;
    //         power1used = true;
    //     }
    //     else if (powerChoice == 2)
    //     {
    //         power2Script = block.transform.GetChild(0).gameObject.GetComponent<SpecialBlock>();
    //         power2Script.choice = powerChoice;
    //         power2Script.blockActivated = true;
    //         power2Script.enabled = true;

    //         power2Num = blockIndex;
    //         power2used = true;
    //     }

    //     if (powerChoice != -1)
    //     {
    //         // Cooldown

    //         putSpecialBlock = false;
    //         StartCoroutine(CooldownSB(true));
    //     }
    // }


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
    /// FixedUpdate is called every frame to determine if the special blocks have been used and destroyed.
    /// If so, the powerup can be reassigned to a new special block.
    /// </summary>
    private void FixedUpdate()
    {
        // if (blocks.ContainsKey(power0Num))
        // {
        //     if (blocks[power0Num] == null)
        //     {
        //         // Remove destroyed blocks

        //         blocks.Remove(power0Num);
        //         StartCoroutine(CooldownSB(false));
        //         power0used = false;
        //     }
        // }

        // if (blocks.ContainsKey(power1Num))
        // {
        //     if (blocks[power1Num] == null)
        //     {
        //         // Remove destroyed blocks

        //         blocks.Remove(power1Num);
        //         StartCoroutine(CooldownSB(false));
        //         power1used = false;
        //     }
        // }

        // if (blocks.ContainsKey(power2Num))
        // {
        //     if (blocks[power2Num] == null)
        //     {
        //         // Remove destroyed blocks

        //         blocks.Remove(power2Num);
        //         StartCoroutine(CooldownSB(false));
        //         power2used = false;
        //     }
        // }
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

    /// <summary>
    /// This function is used to track the amount of time since the last special block was assigned.
    /// If the cooldown has passed, then a new special block can be assigned.
    /// </summary>
    /// <param name="initial"></param>
    /// <param name="powerNum"></param>
    /// <returns></returns>
    // IEnumerator CooldownSB(bool initial, int powerNum = -1)
    // {
    //     int counter = 0;
    //     if (initial)
    //     {
    //         counter = cooldownSB;

    //         while (counter > 0)
    //         {
    //             yield return new WaitForSeconds(1);
    //             counter--;

    //             if (counter < 1)
    //             {
    //                 putSpecialBlock = true;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         counter = 5;

    //         while (counter > 0)
    //         {
    //             yield return new WaitForSeconds(1);
    //             counter--;

    //             if (counter < 1)
    //             {
    //                 if (powerNum == 0)
    //                 {
    //                     power0used = false;
    //                     power0Script = null;
    //                     power0Num = -1;
    //                 }

    //                 if (powerNum == 1)
    //                 {
    //                     power1used = false;
    //                     power1Script = null;
    //                     power1Num = -1;
    //                 }

    //                 if (powerNum == 2)
    //                 {
    //                     power2used = false;
    //                     power2Script = null;
    //                     power2Num = -1;
    //                 }
    //             }
    //         }
    //     }


    // }
}
