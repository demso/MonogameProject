using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace FirstGame.Game
{
    internal class PlayerController : Component, IUpdatable
    {
        
        public float moveSpeed = 300;
        private Vector2 velocity = new Vector2(0, 0);
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
            velocity *= moveSpeed;

            Entity.Transform.Position += velocity * Time.DeltaTime;
        }
    }
}
