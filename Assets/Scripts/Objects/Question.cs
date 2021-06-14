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
    public qImage Distractor1 { get; set; }
    public qImage Distractor2 { get; set; }
    public qImage Distractor3 { get; set; }

    public difficulty(qImage Distractor1, qImage Distractor2, qImage Distractor3)
    {
        this.Distractor1 = Distractor1;
        this.Distractor2 = Distractor2;
        this.Distractor3 = Distractor3;
    }
}

[Serializable]
public class MatrixReasoningQ
{
    public qImage Correct { get; set; }
    public string ID { get; set; }
    public qImage Question { get; set; }
    public difficulty diff1 { get; set; }

    public MatrixReasoningQ(qImage Correct, string ID, qImage Question, difficulty diff1)
    {
        this.Correct = Correct;
        this.ID = ID;
        this.Question = Question;
        this.diff1 = diff1;
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

