using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Nez;

namespace FirstGame.Game.objects.bodies.player
{
    internal class PlayerCollisionHandler : CollisionHandler<Player>, IFixedUpdatable
    {
        List<Body> _closeBodies = new List<Body>();
        public PlayerCollisionHandler(Player data) : base(data, data.Body.Body) { }

        public override bool OnBeginContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
        {
            preCol(contact);
            if (otherUserData is IInteractable && thisFixture.IsSensor && !otherFixture.IsSensor){
                _closeBodies.Add(otherBody);
                UpdatePlayerClosestObject();
            }
            return true;
        }

        public override void OnEndContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
        {
            preCol(contact);
            if (otherUserData is IInteractable && thisFixture.IsSensor && !otherFixture.IsSensor){
                _closeBodies.Remove(otherBody);
                UpdatePlayerClosestObject();
            }
        }

        public void UpdatePlayerClosestObject()
        {
            if (_closeBodies.Count == 0)
            {
                data.ClosestObject = null;
            }
            else
            {
                float minDist = float.MaxValue;
                float dist;
                foreach (Body closeBody in _closeBodies)
                {
                    dist = Vector2.DistanceSquared(body.Position, closeBody.Position);
                    if (dist < minDist)
                    {
                        data.ClosestObject = closeBody;
                        minDist = dist;
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            UpdatePlayerClosestObject();
        }

        public void Update()
        {
            
        }
    }
}
