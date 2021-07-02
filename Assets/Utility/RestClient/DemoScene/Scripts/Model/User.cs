using System;

[Serializable]
public class Difficulty
{
    public int TR;
    public int RT;
    public int TX;
    public int FL;

    public Difficulty(int TR, int RT, int TX, int FL)
    {
        this.TR = TR;
        this.RT = RT;
        this.TX = TX;
        this.FL = FL;
    }
}


[Serializable]
public class User
{
	public string username;

    public Record record;

    public string dob;

    public int Total_Points;

    public Difficulty diff;

    public override string ToString(){
		return UnityEngine.JsonUtility.ToJson (this, true);
	}

    public User(string name,  string dob, int Total_Points, Difficulty diff)
    {
        this.username = name;
        this.dob = dob;
        this.Total_Points = 0;
        this.diff = diff;
    }
}
