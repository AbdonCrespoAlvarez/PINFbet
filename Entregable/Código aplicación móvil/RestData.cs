using System;
using System.Collections.Generic;

/*
Classes used to represent in the local app the information stored in the PINFbet database.
*/

/*
Subobjectives: SO_9.
*/
[Serializable]
public class RestData { }

/*
Subobjectives: SO_9.
*/
[Serializable]
public class UserData : RestData
{
    public string name;
    public string fullname;
    public string password;
    public string salt;
    public UserData(string n, string f, string p, string s)
    {
        name = n;
        fullname = f;
        password = p;
        salt = s;
    }
}

/*
Subobjectives: SO_9.
*/
[Serializable]
public class MarkData : RestData
{
    public string student;
    public float numericScore;
    public MarkData(string s, float n)
    {
        student = s;
        numericScore = n;
    }
}

/*
Subobjectives: SO_9.
*/
[Serializable]
public class BetData : RestData
{
    public string username;
    public string studentname;
    public string subjectname;
    public int coins;
    public float markExpected;
    public BetData(string st, string su, int c, float m)
	{
        username = RequestHandler.sessionUsername;
        studentname = st;
        subjectname = su;
        coins = c;
        markExpected = m;
    }
}

/*
Subobjectives: SO_9.
*/
[Serializable]
public class FriendData : RestData
{
    public string username;
    public int coins;
}

/*
Subobjectives: SO_9.
*/
[Serializable]
public class SubjectListData : RestData
{
    public List<string> names;
    public SubjectListData(List<string> n) => names = n;
}

/*
Subobjectives: SO_9.
*/
[Serializable]
public class RestResult : RestData
{
    public int RequestCode;
    public string[] data;
    public string[] errors;
}

/*
Subobjectives: SO_9.
*/
[Serializable]
public class Config
{
    public string host;
    public string port;
}