using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlatform : MonoBehaviour
{
    // Collection of Blocks
    public GameObject block1;
    public GameObject block2;

    // Current State and Record of active/special scripts and blocks
    ActivatedBlock ABscript = null;

    // Information on ALL existing players in game:
    public GameObject player = null;

    // Steps flags:
    private int stage = 0;
    private bool inStage = false;

    // Start is called before the first frame update
    void Start()
    {
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

        if (player != null && !inStage){
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

        GameObject block = block1;

        print("1");

        // Activate Block based on blockIndex

        ABscript = block.transform.GetChild(0).gameObject.GetComponent<ActivatedBlock>();
        ABscript.colorIndex = player.GetComponent<PlayerController>().colorIndex;
        ABscript.enabled = true;
        print("2");

        ABscript.blockActivated = true;
    }

    void step2(){
        inStage = true;
        stage = 2;
    }

    void step3(){
        inStage = true;
        stage = 3;

    }

    void step4(){
        inStage = true;
        stage = 4;

    }

    void tutorialOver(){

    }
}
