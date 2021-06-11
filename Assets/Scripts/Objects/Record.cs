using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record
{
    public string DateTime
    {
        get;
        private set;
    }

    public int Difficulty
    {
        get;
        private set;
    }

    public int Category
    {
        get;
        private set;
    }

    public string playerName
    {
        get;
        private set;
    }

    public int Points
    {
        get;
        private set;
    }

    public int Rank
    {
        get;
        private set;
    }

    public Dictionary<string, int> Responses
    {
        get;
        private set;
    }

    public Dictionary<string, object> MRQ
    {
        get;
        private set;
    }

    // Start is called before the first frame update
    public Record(string datetime, int diff, int cat, string playerName, int points, int rank)
    {
        this.DateTime = datetime;
        this.Difficulty = diff;
        this.Category = cat;

        this.playerName = playerName;
        this.Points = points;
        this.Rank = rank;
    }

    public void attachResponses(Dictionary<string, int> resp, Dictionary<string, object> MRQRecord)
    {
        this.Responses = resp;
        this.MRQ = MRQRecord;
    }
}
