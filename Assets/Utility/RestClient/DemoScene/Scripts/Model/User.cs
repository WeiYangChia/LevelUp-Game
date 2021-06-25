using System;


[Serializable]
public class User
{
	public string username;

    public Record record;

    public string dob;

    public int Total_Points;

	public override string ToString(){
		return UnityEngine.JsonUtility.ToJson (this, true);
	}

    public User(string name,  string dob, int Total_Points)
    {
        this.username = name;
        this.dob = dob;
        this.Total_Points = 0;
    }
}
