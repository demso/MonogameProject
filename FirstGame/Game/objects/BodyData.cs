#nullable enable
using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FirstGame.Game.objects;

namespace FirstGame.Game.entyties;

public interface BodyData {
    public Object GetData();

    public string GetName();

    public BaseCollisionHandler GetCollisionHandler()
    {
        return null;
    }
}