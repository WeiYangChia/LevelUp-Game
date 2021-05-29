using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class Option
{
    public string one;
    public string two;
    public string three;
    public string four;

    public Option(string first, string second, string third, string fourth)
    {
        this.one = first;
        this.two = second;
        this.three = third;
        this.four = fourth;
    }
}
public class Question
{

    public int ID;
    public int Difficulty;
    public int Category;
    public string Description;
    public Option Options;
    public string Correct;
    public float timeLimit;

    //public int id
    //{
    //    get;
    //    private set;
    //}
    //public int difficulty
    //{
    //    get;
    //    private set;
    //}
    //public int category
    //{
    //    get;
    //    private set;
    //}

    //public string description
    //{
    //    get;
    //    private set;
    //}

    //public Dictionary<string, bool> options
    //{
    //    get;
    //    private set;
    //}

    public Question(int id, int diff, int cat, string desc, Option opt, string corr)
    {
        this.ID = id;
        this.Difficulty = diff;
        this.Category = cat;
        this.Description = desc;
        this.Options = opt;
        this.Correct = corr;
    }
}
