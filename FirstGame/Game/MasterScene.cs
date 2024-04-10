using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public override void Initialize()
        {
            Graphics.Instance.Batcher.ShouldRoundDestinations = false;
            Graphics.Instance.Batcher.SetIgnoreRoundingDestinations(true);

            ClearColor = Color.Black;
            renderer = new MyRenderer();
            renderer.ShouldDebugRender = true;
            //Core.DebugRenderEnabled = true;
            AddRenderer(renderer);
            Camera.ZoomIn(0.2f);
            
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

            penumbra = new PenumbraComponent(Core.Instance);
            //penumbra.Debug = true;
            penumbra.Initialize();
            penumbra.AmbientColor = Color.Black;
            new TiledBodiesLoader(this).LoadBodies(tiledMap);
            
            _light = new PointLight
            {
                Position = playerEntity.Position,
                Color = Color.LightYellow,
                Scale = new Vector2(1300),
                ShadowType = ShadowType.Solid
            };
            penumbra.Lights.Add(_light);
            Hull h = new Hull( new Vector2[4]{new Vector2(300, 300), new Vector2(350, 300), new Vector2(350, 350), new Vector2(300, 350)});
            //h.Position = new Vector2(400, 400);
            
            // Adding the Hull to Penumbra
            penumbra.Hulls.Add(h);
        }
        
        
        
        public override void Update()
        {
            base.Update();
            
            //penumbra.Transform = Camera.TransformMatrix;
            //_light.Position = playerEntity.Position;

            if (Input.IsKeyPressed(Keys.OemPlus))
            {
                Camera.ZoomIn(zoomStep);
            }
            if (Input.IsKeyPressed(Keys.OemMinus))
            {
                Camera.ZoomOut(zoomStep);
            }
            penumbra.BeginDraw();
        }

        public override void Render()
        {
            
            base.Render();
           
            // Core.GraphicsDevice.Clear(Color.White);
            // penumbra.Draw(gameTime);
            

            if (phDebug)
            {
                Graphics.Instance.Batcher.Begin();
                debugView.Render(Graphics.Instance.Batcher, Camera);
                Graphics.Instance.Batcher.End();
            }
        }

        public MasterScene() : base()
        { }
    }
}
