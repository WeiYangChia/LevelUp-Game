using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class handles the colour of the Astronaut that the player chooses
/// </summary>
public class AstronautMarkerSet : MonoBehaviour
{
    // Different colours that the astronaut character can become
    public Material blue;
    public Material pink;
    public Material green;
    public Material yellow;
    public Material purple;
    public Material brown;

    // The parts of the ball character that the colour can be changed
    SkinnedMeshRenderer rend1;

    // Start is called before the first frame update
    void Start()
    {
        setColor();
    }

    /// <summary>
    /// Sets the colour of the astronaut character that the player has chosen.
    /// </summary>

    private void setColor()
    {
        // Determines which part of the astronaut character will change to the different colours
        rend1 = gameObject.transform.Find("Player").gameObject.GetComponent<SkinnedMeshRenderer>();
        Material[] materials1 = rend1.materials;
        Material chosen = null;

        // Determines the colour that the player has chosen.
        switch (gameObject.GetComponent<PlayerController>().colorIndex)
        {
            case 1:
                chosen = blue;
                break;

            case 2:
                chosen = pink;
                break;

            case 3:
                chosen = green;
                break;

            case 4:
                chosen = yellow;
                break;

            case 5:
                chosen = purple;
                break;

            case 6:
                chosen = brown;
                break;

        }

        // Sets the astronaut colour to the chosen colour
        materials1[1] = chosen;
        rend1.materials = materials1;
    }
}
