using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace FirstGame.Game.entyties;

public interface BodyData
{
    public Object GetData();

    public string GetName();

    public bool OnBeginContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
    {
        return true;
    }

    public void OnEndContact(Fixture thisFixture, Fixture otherFixture, Contact contact) {}
}