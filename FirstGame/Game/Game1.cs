﻿

using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using Microsoft.Xna.Framework;

namespace FirstGame.Game
{

    public class Game1 : Core
    {
        static int width = 1280;
        static int height = 720;
        SpriteBatch spriteBatch;
        public Game1() : base(width, height)
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            //this.IsFixedTimeStep = true;//false;
            Screen.SynchronizeWithVerticalRetrace = true;
            Screen.ApplyChanges();
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 75f); //60);
        }

        protected override void Initialize()
        {
            base.Initialize();

            msc = new MasterScene();
            Scene = msc;
            Scene.SetDefaultDesignResolution(width, height, Scene.SceneResolutionPolicy.None);
            Scene.SamplerState = new SamplerState()
            {
                Filter = TextureFilter.Anisotropic
            };
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        private MasterScene msc;
        protected override void Draw(GameTime gameTime)
        {
            
            ((MasterScene)Scene).gameTime = gameTime;
            base.Draw(gameTime);
           
            if (msc.phDebug)
            {
                msc.debugView.Render(Graphics.Instance.Batcher, msc.Camera);
            }
        }
    }
}
