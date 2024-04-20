using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;
using FirstGame.Game.entyties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;

namespace FirstGame.Game.objects
{
    public class CollisionHandler<T> : Component, BaseCollisionHandler
    {
        protected T data;
        protected Body body;
        protected Fixture thisFixture;
        protected Fixture otherFixture;

        protected Body thisBody;
        protected Body otherBody;

        protected Object thisUserData;
        protected Object otherUserData;

        protected Object thisFixtureUserData;
        protected Object otherFixtureUserData;

        protected String otherBodyUserName;

        public CollisionHandler(T data, Body body)
        {
            this.data = data;
            this.body = body;
        }

        public virtual bool OnBeginContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
        {
            return true;
        }

        public virtual void OnEndContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
        {
        }
        protected void preCol(Contact contact)
        {
            if (contact.FixtureA.Body == body)
            {
                thisFixture = contact.FixtureA;
                otherFixture = contact.FixtureB;
            }
            else
            {
                thisFixture = contact.FixtureB;
                otherFixture = contact.FixtureA;
            }

            thisBody = body;
            otherBody = otherFixture.Body;

            thisUserData = thisBody.UserData;
            otherUserData = otherBody.UserData;

            thisFixtureUserData = thisFixture.UserData;
            otherFixtureUserData = otherFixture.UserData;

            otherBodyUserName = otherUserData is BodyData bodyData ? bodyData.GetName() : "";
        }
    }
}
