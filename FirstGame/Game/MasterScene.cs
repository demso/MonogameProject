using System;
using System.Collections;
using System.Collections.Generic;
using Box2DLight;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FirstGame.Game.components;
using FirstGame.Game.entyties;
using FirstGame.Game.objects.bodies;
using FirstGame.Game.objects.bodies.player;
using FirstGame.Game.tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;
using Nez.Farseer;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using Light = Box2DLight.Light;

namespace FirstGame.Game
{
    public partial class MasterScene : Scene
    {
        public static MasterScene Instance;

        public Player player;
        public Entity tiledEntity;
        public Entity debugViewEntity;
        public TmxMap tiledMap;
        public FSWorld world;
        public FSDebugView debugView;

        public Renderer renderer;

        public float zoomStep = 0.1f;
        public bool phDebug = false;
        public Sprite sprite;
        public GameTime gameTime;
        public RayHandler rh;
        public SpriteBatch spriteBatch;
        public float physicsStep = 1 / 100f;
        public Texture2D UserSelection;

        public override void Update()
        {
            base.Update();

            Camera.Position = player.Position;

            if (Input.IsKeyPressed(Keys.OemPlus))
            {
                Camera.ZoomIn(zoomStep);
            }

            if (Input.IsKeyPressed(Keys.OemMinus))
            {
                Camera.ZoomOut(zoomStep);
            }

            if (Input.IsKeyPressed(Keys.E))
            {
                //player.;
            }
        }

        internal void FixedUpdate()
        {
            Entities.FixedUpdate();
        }

        private float accumulator;
        //private void fixedUpdate(float delta)
        //{
        //    // fixed time step
        //    // max frame time to avoid spiral of death (on slow devices)
        //    float frameTime = Math.Min(delta, 0.25);
        //    accumulator += frameTime;
        //    while (accumulator >= options.getTimeStep())
        //    {
        //        var timeStep = options.getTimeStep();

        //        updateFixedObjects();

        //        getWorld().step(timeStep, options.getVelocityIteration(), options.getPositionIterations());
        //        contactListener.update();

        //        accumulator -= timeStep;
        //    }

        //    if (options.isInterpolateMovement())
        //    {
        //        interpolateMovement(accumulator);
        //    }
        //}

        public override void Render()
        {
            ClearColor = Color.AliceBlue;
            base.Render();

            if (player.ClosestObject != null)
            {
                spriteBatch.Begin(transformMatrix: Camera.TransformMatrix);
                spriteBatch.Draw(UserSelection,Vector2.Floor(player.ClosestObject.Position) * FSConvert.SimToDisplay, Color.White);
                spriteBatch.End();
            }
        }
    }
}
