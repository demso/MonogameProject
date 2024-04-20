using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FirstGame.Game.components;
using FirstGame.Game.entyties;
using FirstGame.Game.objects;
using FirstGame.Game.objects.bodies;
using FirstGame.Game.objects.bodies.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Farseer;

namespace FirstGame.Game
{
    internal class PlayerController : Component, IFixedUpdatable, IUpdatable
    {
        private Vector2 moveVector;
        private Vector2 moveImpulse;
        private Player player;

        public void Update()
        {
            if (Input.IsKeyPressed(Keys.E))
            {
                if (player.ClosestObject?.UserData is IInteractable interactable)
                {
                    interactable.Interact(player);
                }
            }
        }

        public void FixedUpdate()
        {
           
            ApplyMovement();
        }

        public void ApplyMovement()
        {
            moveVector = Vector2.Zero;
            if (Input.IsKeyDown(Keys.Right) || Input.IsKeyDown(Keys.D))
            {
                moveVector.X = 1;
            }
            if (Input.IsKeyDown(Keys.Left) || Input.IsKeyDown(Keys.A))
            {
                moveVector.X = -1;
            }
            if (Input.IsKeyDown(Keys.Up) || Input.IsKeyDown(Keys.W))
            {
                moveVector.Y = -1;
            }
            if (Input.IsKeyDown(Keys.Down) || Input.IsKeyDown(Keys.S))
            {
                moveVector.Y = 1;
            }
            if (!moveVector.Equals(Vector2.Zero))
            {
                moveVector.Normalize();
            }
            if (Input.IsKeyDown(Keys.LeftShift))
                moveVector *= 1.5f;
            else if (Input.IsKeyDown(Keys.C))
                moveVector *= 0.75f;

            FSRigidBody rigidBody = Entity.Components.GetComponent<FSRigidBody>(true);
            Body body = rigidBody.Body;

            moveImpulse = ((moveVector * player.moveSpeed * body.Mass) / (1 - MasterScene.Instance.physicsStep * body.LinearDamping)) * MasterScene.Instance.physicsStep * 10f;

            if (!moveImpulse.Equals(Vector2.Zero))
                body.ApplyLinearImpulse(moveImpulse);
        }

        public override void Initialize()
        {
            player = (Player) Entity;
        }
    }
}
