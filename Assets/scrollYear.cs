using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class scrollYear : MonoBehaviour
{
    public TMPro.TMP_Dropdown TMPDropdown;
    public List<int> list;

    void Start()
    {

        TMPDropdown.options.Clear();

        int curYear = DateTime.Now.Year;

        for (int t = curYear; t > curYear-50; t--)
        {
            TMPDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = t.ToString() });
        }
    }
}
