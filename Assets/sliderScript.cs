using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class sliderScript : MonoBehaviour
{
    public Slider difLevel;
    // Start is called before the first frame update
    void Start()
    {
        difLevel = GetComponent<Slider>();
    }

    public void changeDifficulty(int val)
    {
        difLevel.value += val;
    }

    public int getDifficulty()
    {
        return (int)difLevel.value;
    }
}
