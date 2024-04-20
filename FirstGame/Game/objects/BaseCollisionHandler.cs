using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstGame.Game.objects
{
    public interface BaseCollisionHandler
    {
        public bool OnBeginContact(Fixture thisFixture, Fixture otherFixture, Contact contact);

        public void OnEndContact(Fixture thisFixture, Fixture otherFixture, Contact contact);
    }
}
