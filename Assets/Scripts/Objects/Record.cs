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

    public string playerid
    {
        get;
        private set;
    }

    public string dob
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
    public Record(string datetime, int diff, int cat, string playerid, string dob, int points, int rank)
    {
        this.DateTime = datetime;
        this.Difficulty = diff;
        this.Category = cat;

        this.playerid = playerid;
        this.dob = dob;
        this.Points = points;
        this.Rank = rank;
    }

    public void attachResponses(Dictionary<string, object> MRQRecord)
    {
        this.MRQ = MRQRecord;
    }
}
