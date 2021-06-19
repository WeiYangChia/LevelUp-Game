using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialPlatform : MonoBehaviour
{
    // Collection of Blocks
    public ActivePlatform AP;
    public GameObject block1;
    public GameObject block2;
    public GameObject tutorialInstructionParent;
    Image tutorialInstruction;

    // Current State and Record of active/special scripts and blocks
    TutorialBlock TBscript = null;
    TutorialBlock prevTBscript = null;

    // Information on ALL existing players in game:
    public GameObject player = null;
    public PlayerController PC;

    // Steps flags:
    public int stage = 0;
    public bool inStage = false;
    private bool inDialogue = false;

    // End button
    public Button finish;

    // Start is called before the first frame update
    void Start()
    {
        tutorialInstruction = tutorialInstructionParent.GetComponent<Image>();
        // #1 Intro panel to show basic controls (how to move) + (how to jump) + (zoom out)

        // #2 First block:
        // #2a Block appearance

        // #2b Instruction to move to block for activation

        // #2c Give time to move

        // #2d Give instruction to answer question, 'for this tutorial, can only give right answer'. Question script to be modified to allow multiple submission.

        // #3 After finishing first block, collect coin after finishing first block:
        // #3a Congratulate player panel

        // #3b Show second block with COIN

        // #3c Instruction to collect coin

        // #3d Congratulate player on receiving coin for bonus points, instruct to answer next question

        // #4 Once finished, activate normal script to allow player to play for 1 min, or click 'Complete' to complete.
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindWithTag("Player");
        PC = player.GetComponent<PlayerController>();

        if (player != null && !inDialogue && PC.moveable && !inStage){
            switch(stage){
                case 0:
                    step1();
                    break;
                case 1:
                    step2();
                    break;
                case 2:
                    step3();
                    break;
                case 3:
                    step4();
                    break;
                case 4:
                    step5();
                    break;
                default:
                    break;
            }
        }
    }

    void step1(){
        inStage = true;
        stage = 1;

        giveInstruction(1);
    }

    void step2(){
        inStage = true;
        stage = 2;

        giveInstruction(2);
    }

    void step3(){
        inStage = true;
        stage = 3;

        GameObject block = block1;

        // Activate Block based on blockIndex

        TBscript = block.transform.GetChild(0).gameObject.GetComponent<TutorialBlock>();
        TBscript.colorIndex = player.GetComponent<PlayerController>().colorIndex;
        TBscript.timeReward = false;
        TBscript.enabled = true;

        TBscript.blockActivated = true;

        giveInstruction(3);
    }

    void step4(){
        inStage = true;
        stage = 4;

        GameObject block = block2;

        // Activate Block based on blockIndex

        prevTBscript = TBscript;

        TBscript = block.transform.GetChild(0).gameObject.GetComponent<TutorialBlock>();
        TBscript.colorIndex = player.GetComponent<PlayerController>().colorIndex;
        TBscript.timeReward = true;
        TBscript.enabled = true;

        TBscript.blockActivated = true;

        giveInstruction(4);
    }

    void step5(){
        inStage = true;
        stage = 5;

        prevTBscript = TBscript;

        giveInstruction(5);
    }

    public void finishStage(){
        inStage = false;
    }

    private void giveInstruction(int stage){
        print("Stage"+stage.ToString());
        tutorialInstructionParent.SetActive(true);

        //Set image with switch case

        string imageSource = "Instructions/" + (stage%4).ToString();

        tutorialInstruction.sprite = Resources.Load<Sprite>(imageSource);

        switch(stage){
            case 1:
                print("Hi "+PC.playerName+", welcome to the tutorial! Here, we will teach you the basics of the game! Click Continue");
                break;
            case 2:
                print("Show Basic Instructions (Visual): (1) arrows to move (2) spacebar to jump (3) shift to zoom out");
                break;
            case 3:
                print("When you enter the arena, blocks will start lighting up. Go to the block to answer a question and earn some points!");
                break;
            case 4:
                print("Great job on answering the first question! When you get a question right, a coin will appear over your next question block. Collect it to earn a bonus! BUT Quick, get off the current block before it drops!");
                break;
            case 5:
                print("Great job! Now that you've got the hang of the game, we'll let you continue practicing! In real play, time rewards have a 5 second time limit, and you can get questions wrong! Click EXIT at the top left hand corner to finish the tutorial. Remember to get off the current block before it drops! Have fun!");
                break;
            default:
                break;

        }

        inDialogue = true;
        PC.moveable = false;
    }

    public void finishInstruction(){
        print("finish Instruction");
        tutorialInstructionParent.SetActive(false);
        
        inDialogue = false;
        PC.moveable = true;

        if (stage == 1 || stage == 2 || stage == 5){
            inStage = false;
        }

        if (stage == 4 || stage == 5){
            prevTBscript.startDropBlock();
        }

        if (stage == 5){
            AP.GetComponent<ActivePlatform>().enabled = true;
            finish.gameObject.SetActive(true);
        }
    }

    public void end(){
        SceneManager.LoadScene("Main Menu");
    }


}
