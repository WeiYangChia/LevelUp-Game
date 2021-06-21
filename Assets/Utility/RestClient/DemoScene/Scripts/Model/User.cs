using System;


[Serializable]
public class User
{
	public string username;

    public Record record;

    public string localid;

    public int Total_Points;

	public override string ToString(){
		return UnityEngine.JsonUtility.ToJson (this, true);
	}

    public User(string name,  string localid, int Total_Points)
    {
        this.username = name;
        this.localid = localid;
        this.Total_Points = 0;
    }
}
