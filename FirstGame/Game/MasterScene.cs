using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FirstGame.Game.components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Farseer;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;

namespace FirstGame.Game
{
    internal class MasterScene : Scene
    {
        private Texture2D playerSpriteTexture;

        Entity playerEntity;
        private FSRigidBody playerBody;
        Entity tiledEntity;
        private Entity debugViewEntity;
        private float playerScale = 1.8f;
        private TmxMap tiledMap;

        private Sprite sprite;

        DefaultRenderer renderer;

        float zoomStep = 0.1f;
        public override void Initialize()
        {
            Graphics.Instance.Batcher.ShouldRoundDestinations = false;
            Graphics.Instance.Batcher.SetIgnoreRoundingDestinations(true);
            

            ClearColor = Color.Black;
            renderer = new DefaultRenderer();
            AddRenderer(renderer);
            Camera.ZoomIn(0.2f);
            
            playerSpriteTexture = Texture2D.FromFile(Core.GraphicsDevice, "Content/assets/ClassicRPG_Sheet.png");
            tiledMap = Content.LoadTiledMap("Content/assets/tiledmap/newmap.tmx");
            sprite = new Sprite(playerSpriteTexture, new Rectangle(16, 0, 16, 16));


            FSConvert.SetDisplayUnitToSimUnitRatio(32);
            FSWorld world = GetOrCreateSceneComponent<FSWorld>();
            world.MinimumUpdateDeltaTime = 1/144f;
            world.World.Gravity = Vector2.Zero;

            FSDebugView debugView = new FSDebugView(world);
            debugView.SetEnabled(false);
            //debugView.AppendFlags(FSDebugView.DebugViewFlags.Shape);
            //debugView.AppendFlags(FSDebugView.DebugViewFlags.AABB);
            //debugView.AppendFlags(FSDebugView.DebugViewFlags.DebugPanel);
            debugView.AppendFlags(FSDebugView.DebugViewFlags.PolygonPoints);
            //debugView.AppendFlags(FSDebugView.DebugViewFlags.PerformanceGraph);
            debugView.AppendFlags(FSDebugView.DebugViewFlags.CenterOfMass);
            
            tiledEntity = CreateEntity("tiledmap");
            tiledEntity.AddComponent(new TiledMapRenderer(tiledMap));

            playerEntity = CreateEntity("player");

            playerBody = new FSRigidBody();
            FSCollisionCircle c = new FSCollisionCircle(15);
            c.SetCollidesWith(Category.All);
            c.SetCollisionCategories(Category.Cat1);
            c.SetRestitution(0);
            c.SetFriction(0.5f);
            c.SetDensity(1);
            playerBody.SetBodyType(BodyType.Dynamic);
            playerBody.SetFixedRotation(true);
            playerBody.SetMass(60f);
            playerBody.SetLinearDamping(12f);

            playerEntity
                .SetScale(Vector2.One * playerScale)
                .SetPosition(200, 200)
                //.AddComponent(playerBody)
                //.AddComponent(c)
                .AddComponent(new BodySpriteRenderer(new Sprite(playerSpriteTexture, new Rectangle(16, 0, 16, 16))))
                .AddComponent(new PlayerController());

            //playerEntity.Position = Vector2.One;

            CreateEntity("debug-view")
                .AddComponent(new PressKeyToPerformAction(Keys.B, e => debugView.SetEnabled(!debugView.Enabled)))
                .AddComponent(debugView);
            //FollowCamera fc = new FollowCamera(playerEntity);
            //fc.
            Camera.Entity.AddComponent(new FollowCamera(playerEntity));
        }

        public override void Update()
        {
            base.Update();
            //Camera.SetPosition(playerEntity.Position);


            if (Input.IsKeyPressed(Keys.OemPlus))
            {
                Camera.ZoomIn(zoomStep);
            }
            if (Input.IsKeyPressed(Keys.OemMinus))
            {
                Camera.ZoomOut(zoomStep);
            }
        }
        private SpriteBatch sb = new SpriteBatch(Core.GraphicsDevice);
       
        //protected override void PostPostRender()
        //{
        //    TiledMapMove
        //    sb.Begin();
        //    sb.Draw(sprite.Texture2D, playerEntity.GetComponent<FSRigidBody>().Body.DisplayPosition, sprite.SourceRect, Color.White,
        //        playerEntity.Transform.Rotation, Vector2.Zero, playerEntity.Transform.Scale, SpriteEffects.None, 0);
        //    sb.End();
        //}
    }
}
