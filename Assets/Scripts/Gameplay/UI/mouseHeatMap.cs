using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mouseHeatMap : MonoBehaviour
{
    public static mouseHeatMap HMap;
    public List<List<int>> mapping = new List<List<int>>();
    public List<List<float>> positions = new List<List<float>>();

    // Start is called before the first frame update
    void Start()
    {
        positions = new List<List<float>>();
        newMaps();
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        List<float> temp = new List<float>();
        temp.Add(mousePos.x);
        temp.Add(mousePos.y);
        positions.Add(temp);
    }

    void newMaps()
    {
        mapping = new List<List<int>>();
        for (int i = 0; i<800; i++)
        {
            List<int> sublist = new List<int>();
            for(int j = 0; j<400; j++)
            {
                sublist.Add(0);
            }
            mapping.Add(sublist);
        }
    }

    public List<List<int>> getMaps()
    {
        if (positions.Count > 0)
        {
            foreach (List<float> item in positions)
            {
                if (0<item[0]&& item[0] < 800 && 0<item[1]&& item[1] < 400)
                {
                    mapping[(int)item[0]][(int)item[1]] += 1;
                }
                
            }
            newMaps();
            return mapping;
        }
        return mapping;
    }

    public void Display(List<List<int>> list)
    {
        string newLine = "";
        print("Elements:");
        foreach (var sublist in list)
        {
            newLine = "";
            foreach (var value in sublist)
            {
                newLine += value.ToString();
            }
            print(newLine);

        }
    }
}

