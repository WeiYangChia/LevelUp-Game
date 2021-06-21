using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Newtonsoft.Json;

public class mouseHeatMap : MonoBehaviour
{
    public static mouseHeatMap HMap;
    public List<RawImage> selection;
    private int count;
    public Dictionary<int, Dictionary<string, float>> MouseMovements;

    // Start is called before the first frame update
    void Start()
    {
        newMouseTracking();
    }


    public void newMouseTracking()
    {
        count = 0;
        MouseMovements = new Dictionary<int, Dictionary<string, float>>() ;
    }

    public void RecordMouse(string location, float timing)
    {
        Dictionary<string, float> toAdd = new Dictionary<string, float>();
        toAdd.Add(location, timing);
        MouseMovements.Add(count, toAdd);
        count += 1;
    }

    public Dictionary<int, Dictionary<string, float>> getMaps()
    {
        Dictionary<int, Dictionary<string, float>> toSend = MouseMovements;
        newMouseTracking();
        return toSend;
        
    }


}

