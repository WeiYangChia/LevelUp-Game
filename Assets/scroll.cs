using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroll : MonoBehaviour
{
    public TMPro.TMP_Dropdown TMPDropdown;
    public List<string> list;

    void Start()
    {
        TMPDropdown.options.Clear();
        foreach (string t in list)
        {
            TMPDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = t });
        }
    }
}
