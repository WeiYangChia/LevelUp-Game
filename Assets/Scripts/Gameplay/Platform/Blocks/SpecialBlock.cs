using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// This script is assigned to the active pad on each active block, to determine its behavior when it becomes a special block for powerups.
/// </summary>

public class SpecialBlock : MonoBehaviourPunCallbacks
{
    // active special block material
    public Material activeMaterial;

    // original inactive block material
    public Material originalMaterial;

    // TileColorController class to determine Countdown materials
    TileColorController TCC;

    // Parent Block and Components
    GameObject parentBlock;
    GameObject highlight;
    MeshRenderer rend;
    Rigidbody rb;

    // Booleans for Behavior
    public bool blockActivated = false;
    bool questionActivated = false;

    // Player Information
    GameObject player;
    public string playerTag;

    // Question
    GameObject question;

    //Powerup:
    public GameObject powerup1;
    public GameObject powerup2;
    public GameObject powerup3;
    public int choice = -1;
    private GameObject chosenPower = null;

    // Photon View
    PhotonView PV;

    /// <summary>
    /// This function is called each time the SpecialBlock script is enabled by the Platform
    /// It obtains references to necessary components on the parent block to be changed.
    /// It intiializes the Block and PowerUp for the required functionality
    /// </summary>

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
        gameObject.tag = "SpecialQuestion";

        // Get Parent Block and Components
        parentBlock = transform.parent.gameObject;
        TCC = parentBlock.transform.parent.gameObject.GetComponent<TileColorController>();
        rend = parentBlock.GetComponent<MeshRenderer>();
        rb = parentBlock.GetComponent<Rigidbody>();
        highlight = parentBlock.transform.GetChild(1).gameObject;

        // Ensures Parent Block does not fall
        rb.isKinematic = true;

        // Lifts parent block up for it to be noticed by players
        parentBlock.transform.Translate(Vector3.up * .15f);

        // Initializes parent block color to special block color
        Material[] materials = rend.materials;
        materials[0] = activeMaterial;
        rend.materials = materials;

        // Set Up Powerup based on choice of powerup assigned

        switch (choice)
        {
            case 0:
                chosenPower = powerup1;
                break;

            case 1:
                chosenPower = powerup2;
                break;

            case 2:
                chosenPower = powerup3;
                break;

            default:
                break;
        }

        setUpPowerUp(chosenPower);
    }

    /// <summary>
    /// This function is called to initialize the powerup and place it above the assigned block
    /// </summary>
    /// <param name="powerup"></param>

    void setUpPowerUp(GameObject powerup)
    {
        // Place powerup above block by changing parent object and position
        powerup.transform.parent = parentBlock.transform;

        powerup.transform.position = parentBlock.transform.position;
        powerup.transform.Translate(new Vector3(0, 1.5f, 0));

        // Ensures powerup is floating
        powerup.GetComponent<Rigidbody>().isKinematic = true;

        // Activate powerup
        powerup.SetActive(true);
        powerup.GetComponent<PickUp>().enabled = true;
    }

    /// <summary>
    /// This function is called when the question is answered correctly;
    /// The block releases the powerup to the player to be picked up
    /// </summary>
    /// <param name="powerup"></param>

    void givePowerUp(GameObject powerup)
    {
        // Allows powerup to fall to user and be picked up
        powerup.GetComponent<Rigidbody>().isKinematic = false;
        powerup.GetComponent<CapsuleCollider>().enabled = true;


        // Once powerup is released, the parent object is reset so the same powerup object can be reassigned to other blocks later.
        powerup.transform.parent = null;
    }

    /// <summary>
    /// This function is called when the question is answered wrongly or unanswered.
    /// The powerup is removed and not released to the player
    /// </summary>
    /// <param name="powerup"></param>

    void removePowerUp(GameObject powerup)
    {
        // set powerup object to inactive and reset parent
        powerup.SetActive(false);
        powerup.transform.parent = null;
    }


    /// <summary>
    /// This function is called when any player enters the trigger zone on the block.
    /// It activates the question and allows the player to earn the powerup by answering the question
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.tag == "Player1" | other.gameObject.tag == "Player2"| other.gameObject.tag == "Player3"| other.gameObject.tag == "Player4") & !questionActivated & blockActivated)
        {

            // Determines which player activated the special block
            player = other.gameObject;
            playerTag = player.tag;
            question = player.GetComponent<PlayerController>().question;

            // Move Block down for visual effect
            parentBlock.transform.Translate(Vector3.down * 0.1f);
            questionActivated = true;

            // Restrict Player
            player.GetComponent<PlayerController>().moveable = false;


            // Activate Highlight and disable collisions for current player and powerup
            highlight.GetComponent<MeshRenderer>().enabled = true;
            StartCoroutine("HighlightFadeIn");

            Physics.IgnoreCollision(player.GetComponent<CharacterController>(), highlight.GetComponent<CapsuleCollider>(), true);
            Physics.IgnoreCollision(player.GetComponent<BoxCollider>(), highlight.GetComponent<CapsuleCollider>(), true);
            Physics.IgnoreCollision(chosenPower.GetComponent<CapsuleCollider>(), highlight.GetComponent<CapsuleCollider>(), true);
            highlight.GetComponent<CapsuleCollider>().enabled = true;

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
        question.GetComponent<DoQuestion>().pointsAwardable = false;

        // Set UI active
        question.SetActive(true);

        // Initialize and begin question counter
        int counter = Mathf.RoundToInt(GameObject.FindWithTag("GameController").GetComponent<QuestionManager>().getTimeLimit());
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
                givePowerUp(chosenPower);
                StartCoroutine("HighlightFadeOut");
            }

            // Case 2: Player answers wrongly
            else if (question.GetComponent<DoQuestion>().answered == true && question.GetComponent<DoQuestion>().correct == false)
            {
                StartCoroutine("HighlightFadeOut");
                PV.RPC("dropSpecBlock", RpcTarget.All);
                yield return new WaitForSeconds(3);
                Destroy(transform.parent.gameObject);

                // Avoids bug where player is stuck because he fails to drop properly
                if (!player.GetComponent<PlayerController>().respawning)
                {
                    player.GetComponent<PlayerController>().moveable = true;
                }
                break;
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
                    removePowerUp(chosenPower);
                    StartCoroutine("HighlightFadeOut");
                }

                PV.RPC("dropSpecBlock", RpcTarget.All);
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

    [PunRPC]
    void dropSpecBlock()
    {
        Physics.IgnoreCollision(player.GetComponent<CharacterController>(), highlight.GetComponent<CapsuleCollider>(), false);
        Physics.IgnoreCollision(player.GetComponent<BoxCollider>(), highlight.GetComponent<CapsuleCollider>(), false);
        highlight.GetComponent<CapsuleCollider>().enabled = false;

        rb.isKinematic = false;
        parentBlock.GetComponent<BoxCollider>().enabled = false;
        removePowerUp(chosenPower);
    }
}
