using System;
using Box2DLight;
using FarseerPhysics.Dynamics;
using FirstGame.Game.components;
using FirstGame.Game.tiled;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez.Farseer;
using Nez;
using FirstGame.Game.entyties;
using FirstGame.Game.objects.bodies;
using FirstGame.Game.objects.bodies.player;
using Microsoft.Xna.Framework;

namespace FirstGame.Game
{
    //end() initialize()
    public partial class MasterScene
    {
        public override void Initialize()
        {
            Graphics.Instance.Batcher.ShouldRoundDestinations = false;
            Graphics.Instance.Batcher.SetIgnoreRoundingDestinations(true);

            Instance = this;

            LoadTextures();

            ClearColor = Color.Black;
            renderer = new DefaultRenderer();
            renderer.ShouldDebugRender = true;
            //Core.DebugRenderEnabled = true;
            AddRenderer(renderer);
            Camera.ZoomIn(0.2f);
            spriteBatch = new SpriteBatch(Core.GraphicsDevice);

            FSConvert.SetDisplayUnitToSimUnitRatio(32);
            world = GetOrCreateSceneComponent<MyFSWorld>();
            world.TimeStep = physicsStep;
            world.World.Gravity = Vector2.Zero;
            world.World.ContactManager.OnBeginContact = contact =>
            {
                return
                    (((BodyData)contact.FixtureA.Body.UserData).GetCollisionHandler()?.OnBeginContact(contact.FixtureA, contact.FixtureB, contact) ?? true) &&
                    (((BodyData)contact.FixtureB.Body.UserData).GetCollisionHandler()?.OnBeginContact(contact.FixtureB, contact.FixtureA, contact) ?? true);
            };

            world.World.ContactManager.OnEndContact = contact =>
            {
                ((BodyData)contact.FixtureA.Body.UserData).GetCollisionHandler()?.OnEndContact(contact.FixtureA, contact.FixtureB, contact);
                ((BodyData)contact.FixtureB.Body.UserData).GetCollisionHandler()?.OnEndContact(contact.FixtureB, contact.FixtureA, contact);
            };

            rh = new RayHandler(world.World);
            AddRenderer(new LightRenderer(rh));

            tiledMap = Content.LoadTiledMap("Content/assets/tiled/worldmap.tmx");

            debugView = new FSDebugView(world);

            tiledEntity = CreateEntity("tiledmap");
            tiledEntity.AddComponent(new TiledMapRenderer(tiledMap));

            player = new Player("Demass");
            AddEntity(player);
                
            player.InitBody();

            debugViewEntity = CreateEntity("debug-view")
                .AddComponent(new PressKeyToPerformAction(Keys.B, e =>
                {
                    phDebug = !phDebug;
                }))
                .AddComponent(debugView).SetEnabled(false).Entity;

            //Camera.Entity.AddComponent(new FollowCamera(player));
            Camera.ZoomIn(zoomStep * 4);
            TiledLoader.Load(tiledMap);

            rh.setCombinedMatrix(Camera.ViewProjectionMatrix, 0, 0, Core.GraphicsDevice.DisplayMode.Width, Core.GraphicsDevice.DisplayMode.Height);
            RayHandler.useDiffuseLight(true);
            rh.setShadows(true);
            rh.setAmbientLight(0f, 0f, 0f, 1f);
            rh.setBlur(true);
            rh.setBlurNum(3);

            Box2dLight.PointLight light = new Box2dLight.PointLight(rh, 1300, Color.White, 50, 0, 0);
            Light.GlobalCollisionCategories = (Category) Globals.LIGHT_CONTACT_FILTER;
            Light.GlobalCollidesWith = Category.All;
            Light.GlobalCollisionGroup = Globals.TRANSPARENT_GROUP;
           // Light.GlobalCollisionGroup = Globals.LIGHT_CONTACT_GROUP;
            light.SetSoft(true);
            light.SetSoftnessLength(1.5f);
            light.AttachToBody(player.Body.Body);
            light.SetIgnoreAttachedBody(true);
        }

        public void LoadTextures()
        {
            UserSelection = Content.LoadTexture("Content/assets/selection.png");
        }

        public override void End()
        {
            base.End();
            rh.Dispose();
        }
    }
}
