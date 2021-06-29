using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using System.IO;


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

[Serializable]
public class qImage
{
    public string Loc { get; set; }
    public string Name { get; set; }

    public qImage(string Loc, string Name)
    {
        this.Loc = Loc;
        this.Name = Name;
    }
}


[Serializable]
public class difficulty
{
    public string Distractor1 { get; set; }
    public string Distractor2 { get; set; }
    public string Distractor3 { get; set; }

    public difficulty(string Distractor1, string Distractor2, string Distractor3)
    {
        this.Distractor1 = Distractor1;
        this.Distractor2 = Distractor2;
        this.Distractor3 = Distractor3;
    }
}

[Serializable]
public class MatrixReasoningQ
{
    public string Correct { get; set; }
    public string ID { get; set; }
    public string Question { get; set; }
    public difficulty diff1 { get; set; }
    public difficulty diff2 { get; set; }
    public difficulty diff3 { get; set; }

    public MatrixReasoningQ(string Correct, string ID, string Question, difficulty diff1, difficulty diff2, difficulty diff3)
    {
        this.Correct = Correct;
        this.ID = ID;
        this.Question = Question;
        this.diff1 = diff1;
        this.diff2 = diff2;
        this.diff3 = diff3;
    }
}


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
[Serializable]
public class MatrixReasoning
{
    public List<string> questions { get; set; }

    public MatrixReasoning(List<string> questions)
    {
        this.questions = questions;
    }
}

