using System;
using System.Collections;
using System.Collections.Generic;
using Box2DLight;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FirstGame.Game.components;
using FirstGame.Game.entyties;
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
    internal partial class MasterScene : Scene
    {
        public static MasterScene Instance;

        Player playerEntity;
        Entity tiledEntity;
        private Entity debugViewEntity;
        private TmxMap tiledMap;
        internal FSWorld world;
        internal FSDebugView debugView;

        Renderer renderer;

        float zoomStep = 0.1f;
        internal bool phDebug = false;
        internal Sprite sprite;
        internal GameTime gameTime;
        internal RayHandler rh;
        SpriteBatch spriteBatch;
        public bool Toggle;
        public bool Toggle2;
        private int blurrr = 3;

        public override void Update()
        {
            base.Update();

            if (Input.IsKeyPressed(Keys.OemPlus))
            {
                Camera.ZoomIn(zoomStep);
            }

            if (Input.IsKeyPressed(Keys.OemMinus))
            {
                Camera.ZoomOut(zoomStep);
            }
        }

        public override void Render()
        {
            ClearColor = Color.AliceBlue;
            base.Render();
        }
    }
}
