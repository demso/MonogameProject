using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Farseer;

namespace FirstGame.Game
{
    internal class PlayerController : Component, IUpdatable
    {
        
        public float moveSpeed = 160;
        private Vector2 velocity = new Vector2(0, 0);
        private Vector2 movement = new Vector2();

        private SubpixelFloat sfX = new SubpixelFloat();
        private SubpixelFloat sfY = new SubpixelFloat();
        private SubpixelVector2 sv = new SubpixelVector2();
        public void Update()
        {
            velocity = Vector2.Zero;
            if (Input.IsKeyDown(Keys.Right) || Input.IsKeyDown(Keys.D))
            {
                velocity.X = 1;
            }
            if (Input.IsKeyDown(Keys.Left) || Input.IsKeyDown(Keys.A))
            {
                velocity.X = -1;
            }
            if (Input.IsKeyDown(Keys.Up) || Input.IsKeyDown(Keys.W))
            {
                velocity.Y = -1;
            }
            if (Input.IsKeyDown(Keys.Down) || Input.IsKeyDown(Keys.S))
            {
                velocity.Y = 1;
            }
            if (!velocity.Equals(Vector2.Zero))
            {
                velocity.Normalize();
            }
            if (Input.IsKeyDown(Keys.LeftShift))
                velocity *= moveSpeed * 1.5f;
            else if (Input.IsKeyDown(Keys.C))
                velocity *= moveSpeed * 0.75f;
            else
                velocity *= moveSpeed;

            //movement = velocity;
            //FSRigidBody rigidBody = Entity.Components.GetComponent<FSRigidBody>(true);
            //Body body = rigidBody.Body;
            //body.ApplyLinearImpulse(movement);
            var movement = velocity * Time.DeltaTime;

            //sfX.Update(ref movement.X);
            //sfY.Update(ref movement.Y);
            Entity.Transform.Position += movement;
        }
    }
}
