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
        private FSDebugView debugView;

        Renderer renderer;

        float zoomStep = 0.1f;
        private bool phDebug = false;
        internal static Sprite sprite;
        internal PenumbraComponent penumbra;
        internal GameTime gameTime;
        private PointLight _light;
        internal RayHandler rh;
        static SpriteBatch spriteBatch;

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

            tiledMap = Content.LoadTiledMap("Content/assets/tiledmap/newmap.tmx");

            debugView = new FSDebugView(world);

            tiledEntity = CreateEntity("tiledmap");
            tiledEntity.AddComponent(new TiledMapRenderer(tiledMap));

            playerEntity = new Player("Demass");
            AddEntity(playerEntity);
            playerEntity.InitBody();

            debugViewEntity = CreateEntity("debug-view")
                .AddComponent(new PressKeyToPerformAction(Keys.B, e =>
                {
                   phDebug = !phDebug;
                }))
                .AddComponent(debugView).SetEnabled(false).Entity;
            
            Camera.Entity.AddComponent(new FollowCamera(playerEntity));
            new TiledBodiesLoader(this).LoadBodies(tiledMap);

            rh = new RayHandler(world.World);
            rh.setCombinedMatrix(Camera.ViewProjectionMatrix, Core.GraphicsDevice.Viewport.X, Core.GraphicsDevice.Viewport.Y, Core.GraphicsDevice.Viewport.Width, Core.GraphicsDevice.Viewport.Height);
            RayHandler.useDiffuseLight(true);
            rh.setAmbientLight(0.5f, 0.5f, 0.5f, 0.5f);
            rh.setBlur(false);
            rh.setBlurNum(0);

            Box2dLight.PointLight light = new Box2dLight.PointLight(rh, 5, Color.White, 300, 0 , 0);
            //light.AttachToBody(playerEntity.Body.Body);
            //light.SetIgnoreAttachedBody(true);
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
            base.Render();

            //this.SceneRenderTarget;
            //Core.GraphicsDevice.SetRenderTarget(renderTarget);
            //Core.GraphicsDevice.Clear(Color.Red);
            //Core.GraphicsDevice.SetRenderTarget(SceneRenderTarget);
            //Core.GraphicsDevice.Clear(Color.Black);


            //    spriteBatch.Begin(SpriteSortMode.Immediate);
            //    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            //    spriteBatch.End();


            //if (phDebug)
            //{
            //    Graphics.Instance.Batcher.Begin();
            //    debugView.Render(Graphics.Instance.Batcher, Camera);
            //    Graphics.Instance.Batcher.End();
            //}
        }

        public MasterScene() : base()
        { }
    }
}
