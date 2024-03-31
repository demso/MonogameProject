using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;

namespace FirstGame.Game
{
    internal class MasterScene : Scene
    {
        private Texture2D playerSpriteTexture;

        Entity playerEntity;
        Entity tiledEntity;

        float zoomStep = 0.1f;
        public override void Initialize()
        {
            ClearColor = Color.AliceBlue;
            AddRenderer(new DefaultRenderer());

            playerSpriteTexture = Texture2D.FromFile(Core.GraphicsDevice, "Content/assets/ClassicRPG_Sheet.png");
            var tiledMap = Content.LoadTiledMap("Content/assets/tiledmap/newmap.tmx");

            tiledEntity = CreateEntity("tiledmap");
            tiledEntity.AddComponent(new TiledMapRenderer(tiledMap));


            playerEntity = CreateEntity("player");

            playerEntity.Transform.SetPosition(200, 200);
            var ass = new SpriteRenderer(new Sprite(playerSpriteTexture, new Rectangle(17, 0, 16, 16)));
            
            playerEntity.AddComponent(ass);
            playerEntity.Transform.Scale = Vector2.One * 1.8f;
            playerEntity.AddComponent(new PlayerController());
            

            //sprite1 = CreateEntity("sprite1");
            //sprite1.AddComponent(new SpriteRenderer(new Sprite(playerSpriteTexture, new Rectangle(17, 0, 16, 16))));
            //SpriteAnimation downWalking = new SpriteAnimation(new Sprite[]{new Sprite(playerSpriteTexture, new Rectangle(0, 0, 16, 16))}, 0.25f);
        }

        public override void Update()
        {
            base.Update();
            Camera.SetPosition(playerEntity.Position);

            if (Input.IsKeyPressed(Keys.OemPlus))
            {
                Camera.ZoomIn(zoomStep);
            }
            if (Input.IsKeyPressed(Keys.OemMinus))
            {
                Camera.ZoomOut(zoomStep);
            }
        }


    }
}
