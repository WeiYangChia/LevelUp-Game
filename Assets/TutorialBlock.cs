using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlock : MonoBehaviour
{
// original inactive block material
    public Material originalMaterial;

    // TileColorController class to determine Player Block and Countdown materials
    TileColorController TCC;
    public TutorialPlatform TP;

    // Parent Block and Components
    GameObject parentBlock;
    GameObject highlight;
    MeshRenderer rend;
    Rigidbody rb;

    // Booleans for Behavior
    public bool blockActivated = false;
    bool questionActivated = false;

    // Player Information
    public int colorIndex;
    string playerTag;
    GameObject player;

    // Question
    GameObject question;

    // Time Reward
    public bool timeReward;
    GameObject reward;
    int timeRewardLimit = 4;
    timeReward rewardController;

    /// <summary>
    /// This function is called each time the ActivatedBlock script is enabled by the Platform
    /// It obtains references to necessary components on the parent block to be changed.
    /// It intiializes the Block to reflect the color of the Player assigned
    /// </summary>
    private void OnEnable()
    {
        gameObject.tag = "Question";

        // Get Parent Block and Components
        parentBlock = transform.parent.gameObject;
        TCC = parentBlock.transform.parent.gameObject.GetComponent<TileColorController>();
        TP = parentBlock.transform.parent.gameObject.GetComponent<TutorialPlatform>();
        rend = parentBlock.GetComponent<MeshRenderer>();
        rb = parentBlock.GetComponent<Rigidbody>();
        highlight = parentBlock.transform.GetChild(1).gameObject;

        // Ensures Parent Block does not fall
        rb.isKinematic = true;

        // Lifts parent block up for it to be noticed by players
        parentBlock.transform.Translate(Vector3.up * .15f);

        // Intializes parent block color based on player assigned
        Material[] materials = rend.materials;
        materials[0] = TCC.getTileMaterial(colorIndex);
        rend.materials = materials;

        // Initializes playerTag to identify correct Player
        playerTag = "Player";

        // giveTimeReward();
        if (timeReward){
            giveTimeReward();
        }
    }

    /// <summary>
    /// This function is called when the correct player enters the trigger zone on the block.
    /// It activates the question and allows the player to earn points by answering the question
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerEnter(Collider other)
    {
        // Conditions to ensure the right player is activating the block
        if (other.gameObject.tag == playerTag & !questionActivated & blockActivated)
        {
            // obtain player references
            player = other.gameObject;
            question = player.GetComponent<PlayerController>().question;

            // Move Block down for visual effect
            parentBlock.transform.Translate(Vector3.down * 0.1f);
            questionActivated = true;

            // Activate Highlight and disable collisions for current player
            highlight.GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine("HighlightFadeIn");

            Physics.IgnoreCollision(player.GetComponent<CharacterController>(), highlight.GetComponent<CapsuleCollider>(), true);
            Physics.IgnoreCollision(player.GetComponent<BoxCollider>(), highlight.GetComponent<CapsuleCollider>(), true);
            highlight.GetComponent<CapsuleCollider>().enabled = true;

            // Restrict Player
            player.GetComponent<PlayerController>().moveable = false;

            // Start Question
            StartCoroutine("Question");
        }
    }

    /// <summary>
    /// This function is called to activate the Question UI when the player activates the block
    /// </summary>
    /// <returns></returns>

    IEnumerator Question()
    {
        // Initialize Question settings
        question.GetComponent<DoQuestion>().answered = false;
        question.GetComponent<DoQuestion>().correct = false;
        question.GetComponent<DoQuestion>().pointsAwardable = true;

        // Set UI active
        question.SetActive(true);

        // Initialize and begin question counter
        int counter = Mathf.RoundToInt(GameObject.FindWithTag("GameController").GetComponent<QuestionManager>().getMaxTime());
        bool moveableChanged = false;

        while (counter > 0)
        {
            yield return new WaitForSeconds(1);

            counter--;

            var materials = rend.materials;

            // Case 1: Player answers correctly
            if (question.GetComponent<DoQuestion>().answered == true && question.GetComponent<DoQuestion>().correct == true)
            {
                if (!moveableChanged)
                {
                    player.GetComponent<PlayerController>().moveable = true;
                    moveableChanged = true;
                }

                question.SetActive(false);

                StartCoroutine("HighlightFadeOut");
             
            }

            // Case 2: Player answers wrongly
            else if (question.GetComponent<DoQuestion>().answered == true && question.GetComponent<DoQuestion>().correct == false)
            {
                StartCoroutine("HighlightFadeOut");
                dropBlock();
                yield return new WaitForSeconds(3);
                Destroy(transform.parent.gameObject);

                // Avoids bug where player is stuck because he fails to drop properly
                if (!player.GetComponent<PlayerController>().respawning)
                {
                    player.GetComponent<PlayerController>().moveable = true;
                }
            }

            // Handles color change for final seconds
            if (counter > 4)
            {
                //
            }
            else if (counter > 0)
            {
                materials[0] = TCC.getCountdownMaterial(counter);
                rend.materials = materials;
            }
            else
            {
                // Case 3: Player does not answer question
                if (question.GetComponent<DoQuestion>().answered == false)
                {
                    StartCoroutine("HighlightFadeOut");
                }

                dropBlock();
                yield return new WaitForSeconds(3);
                Destroy(transform.parent.gameObject);

                // Avoids bug where player is stuck because he fails to drop properly
                if (!player.GetComponent<PlayerController>().respawning)
                {
                    player.GetComponent<PlayerController>().moveable = true;
                }
                
                break;
            }
        }
    }

    /// <summary>
    /// This function is called to activate the Highlight and allow visual of it fading in
    /// </summary>
    /// <returns></returns>

    IEnumerator HighlightFadeIn()
    {
        MeshRenderer highlightRend = highlight.GetComponent<MeshRenderer>();
        Color highlightColor = highlightRend.material.color;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
        {
            Color newColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, Mathf.Lerp(0, highlightColor.a, t));

            highlightRend.material.SetColor("_Color", newColor);

            yield return null;
        }

        TP.finishStage();
    }

    /// <summary>
    /// This function is called to deactive Highlight and allow visual of it fading out
    /// </summary>
    /// <returns></returns>

    IEnumerator HighlightFadeOut()
    {
        MeshRenderer highlightRend = highlight.GetComponent<MeshRenderer>();
        Color highlightColor = highlightRend.material.color;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
        {
            Color newColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, Mathf.Lerp(highlightColor.a, 0, t));

            highlightRend.material.SetColor("_Color", newColor);

            yield return null;
        }
    }

    /// <summary>
    /// This function is called to drop the block.
    /// A network event is called to ensure all players see that the block drops.
    /// </summary>

    void dropBlock()
    {
        Physics.IgnoreCollision(player.GetComponent<CharacterController>(), highlight.GetComponent<CapsuleCollider>(), false);
        Physics.IgnoreCollision(player.GetComponent<BoxCollider>(), highlight.GetComponent<CapsuleCollider>(), false);

        highlight.GetComponent<CapsuleCollider>().enabled = false;

        rb.isKinematic = false;

        parentBlock.GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// This function is called when the player block expires and reshuffles.
    /// The block is reset to its inactive state.
    /// </summary>

    private void OnDisable()
    {
        gameObject.tag = "Untagged";

        if (rb.isKinematic)
        {
            Material[] materials = rend.materials;
            materials[0] = originalMaterial;
            rend.materials = materials;

            parentBlock.transform.Translate(Vector3.down * .15f);
        }
    }

    private void giveTimeReward(){
        claimTimeReward();

        // Activate powerup
        reward.SetActive(true);
        rewardController.enabled = true;
    }

    private void claimTimeReward(){
        reward = player = GameObject.FindWithTag("timeReward");
        rewardController = reward.GetComponent<timeReward>();

        reward.transform.parent = parentBlock.transform;
        reward.transform.position = parentBlock.transform.position;
        reward.transform.Translate(new Vector3(0, -0.75f, 0));

        // StartCoroutine(TimeRewardCounter());
    }

    IEnumerator TimeRewardCounter()
    {
        
        rewardController.enabled = true;

        int counter = rewardController.timeRewardLimit;

        while (counter > -1)
        {
            rewardController.setTime(counter);
            yield return new WaitForSeconds(1);
            counter--;

            if (counter == 0){
                releaseTimeReward();
                break;
            }
        }
    }

    private void releaseTimeReward(){
        rewardController.deactivate();
    }
}
