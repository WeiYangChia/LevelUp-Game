using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;


public class Test_Script : MonoBehaviour
{
    public Dictionary<string, object> maintainance = new Dictionary<string, object>();
    public GameObject maintainanceCanvas;
    public TMP_Text maintainancetxt;

    // Start is called before the first frame update
    void Start()
    {
        RestClient.Get(url: "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Maintainance.json").Then(onResolved: response =>
        {
            maintainance = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Text);
            if ((bool)maintainance["Maintainance"])
            {
                maintainanceCanvas.SetActive(true);
                maintainancetxt.text = (string)maintainance["Notice"];
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

