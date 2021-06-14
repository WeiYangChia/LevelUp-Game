using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading_Screen : MonoBehaviour
{
    public static Loading_Screen LS;
    public GameObject loadingScreen;
    public Slider Slider;

    int count;
    int totalprogress;

    bool complete;

    public IEnumerable moveSlider()
    {
        count += 1;
        float progress = Mathf.Clamp01(count / totalprogress);
        Slider.value = progress;
        if (progress >= 0.9f)
        {
            loadingScreen.SetActive(false);
            complete = true;
        }
        return null;
    }

    public void percentageValues(int value)
    {
        totalprogress = value;
        count = 0;
        Slider.value = 0f;
        loadingScreen.SetActive(true);
        complete = false;
    }
}
