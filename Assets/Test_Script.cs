using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;


public class Test_Script : MonoBehaviour
{
    public List<string> questionlist = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        RestClient.Get(url: "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/QuestionList/Matrix Reasoning.json").Then(onResolved: response =>
        {
            print("Loaded");
            print(response.Text);
            questionlist = JsonConvert.DeserializeObject<List<string> > (response.Text);
            print("JSON Loaded");
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

