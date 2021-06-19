using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Information on ALL existing players in game:
    public GameObject player = null;
    public PlayerController PC;

    // Steps flags:
    private int stage = 0;
    private bool inStage = false;
    private bool inDialogue = false;

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
                case 5:
                    tutorialOver();
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
        print("Hi "+PC.playerName+", welcome to the tutorial! Here, we will teach you the basics of the game! Click Continue");
        
        // tutorialInstruction
    }

    void step2(){
        inStage = true;
        stage = 2;

        GameObject block = block1;

        // Activate Block based on blockIndex

        TBscript = block.transform.GetChild(0).gameObject.GetComponent<TutorialBlock>();
        TBscript.colorIndex = player.GetComponent<PlayerController>().colorIndex;
        TBscript.timeReward = false;
        TBscript.enabled = true;

        TBscript.blockActivated = true;

        giveInstruction(2);
        print("When you enter the arena, blocks will start lighting up. Go to the block to answer a question and earn some points!");
    }

    void step3(){
        inStage = true;
        stage = 3;

        TBscript.startDropBlock();

        GameObject block = block2;

        // Activate Block based on blockIndex

        TBscript = block.transform.GetChild(0).gameObject.GetComponent<TutorialBlock>();
        TBscript.colorIndex = player.GetComponent<PlayerController>().colorIndex;
        TBscript.timeReward = true;
        TBscript.enabled = true;

        TBscript.blockActivated = true;

        giveInstruction(3);
        print("Great job on answering the first question! When you get a question right, a coin will appear over your next question block. Collect it to earn a bonus!");
    }

    void step4(){
        inStage = true;
        stage = 4;

        TBscript.startDropBlock();

        giveInstruction(4);
        print("Great job! Now that you've got the hang of the game, we'll let you continue practicing! Click EXIT at the top left hand corner to finish the tutorial. Have fun!");
    }

    void tutorialOver(){
        AP.GetComponent<ActivePlatform>().enabled = true;
    }

    public void finishStage(){
        giveInstruction(stage);
    }

    private void giveInstruction(int stage){
        tutorialInstructionParent.SetActive(true);

        //Set image with switch case

        inDialogue = true;
        PC.moveable = false;
    }

    public void finishInstruction(){
        tutorialInstructionParent.SetActive(false);
        
        inDialogue = false;
        PC.moveable = true;

        if (stage == 1 || stage == 4){
            inStage = false;
        }
    }
}
