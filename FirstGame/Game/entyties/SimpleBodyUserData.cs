using System;

namespace FirstGame.Game.entyties;

public class SimpleBodyUserData(object d, string n) : BodyData
{
    public string Name = n;
    public Object Data = d;

    public string GetName()
    {
        return Name;
    }

    public object GetData()
    {
        return Data;
    }
}