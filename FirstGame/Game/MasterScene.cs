using System;
using System.Collections;
using System.Collections.Generic;
using Box2DLight;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FirstGame.Game.components;
using FirstGame.Game.tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Farseer;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using NVorbis.Ogg;
using Penumbra;
using static Nez.Farseer.FSDebugView;

namespace FirstGame.Game
{
    internal class MasterScene : Scene
    {
        Player playerEntity;
        Entity tiledEntity;
        private Entity debugViewEntity;
        private TmxMap tiledMap;
        internal static FSWorld world;
        internal FSDebugView debugView;

        Renderer renderer;

        float zoomStep = 0.1f;
        internal bool phDebug = false;
        internal static Sprite sprite;
        internal PenumbraComponent penumbra;
        internal GameTime gameTime;
        private PointLight _light;
        internal RayHandler rh;
        static SpriteBatch spriteBatch;
        public bool Toggle;

        public override void Initialize()
        {
            Graphics.Instance.Batcher.ShouldRoundDestinations = false;
            Graphics.Instance.Batcher.SetIgnoreRoundingDestinations(true);

            ClearColor = Color.Black;
            renderer = new DefaultRenderer();
            renderer.ShouldDebugRender = true;
            //Core.DebugRenderEnabled = true;
            AddRenderer(renderer);
            Camera.ZoomIn(0.2f);
            spriteBatch = new SpriteBatch(Core.GraphicsDevice);

            FSConvert.SetDisplayUnitToSimUnitRatio(32);
            world = GetOrCreateSceneComponent<FSWorld>();
            world.TimeStep = 1 / 144f;
            world.World.Gravity = Vector2.Zero;

            rh = new RayHandler(world.World);
            //AddRenderer(new LightRenderer(rh));

            tiledMap = Content.LoadTiledMap("Content/assets/tiledmap/newmap.tmx");

            debugView = new FSDebugView(world);

            tiledEntity = CreateEntity("tiledmap");
            tiledEntity.AddComponent(new TiledMapRenderer(tiledMap));

            playerEntity = new Player("Demass");
            AddEntity(playerEntity)
                .AddComponent(new PressKeyToPerformAction(Keys.N, e =>
                {
                    Toggle = !Toggle;
                }));
            playerEntity.InitBody();

            debugViewEntity = CreateEntity("debug-view")
                .AddComponent(new PressKeyToPerformAction(Keys.B, e =>
                {
                    phDebug = !phDebug;
                }))
                .AddComponent(debugView).SetEnabled(false).Entity;
            
            Camera.Entity.AddComponent(new FollowCamera(playerEntity));
            Camera.ZoomIn(zoomStep * 4);
            new TiledBodiesLoader(this).LoadBodies(tiledMap);

            
            rh.setCombinedMatrix(Camera.ViewProjectionMatrix, 0, 0, Core.GraphicsDevice.DisplayMode.Width, Core.GraphicsDevice.DisplayMode.Height);
            RayHandler.useDiffuseLight(true);
            rh.setAmbientLight(0f, 0f, 0f, 1f);
            rh.setBlur(false);
            rh.setBlurNum(0);

            Box2dLight.PointLight light = new Box2dLight.PointLight(rh, 1300, Color.White, 50, 0 , 0);
            light.SetSoft(true);
            light.SetSoftnessLength(1.5f);
            light.AttachToBody(playerEntity.Body.Body);
            light.SetIgnoreAttachedBody(true);
        }
        
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

        public MasterScene() : base()
        { }

        public override void End()
        {
            base.End();

            rh.Dispose();
        }
    }
}
