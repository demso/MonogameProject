using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using Microsoft.Xna.Framework;
using Penumbra;

namespace FirstGame.Game
{

    public class Game1 : Core
    {
        static int width = 1920;
        static int height = 1080;
        public Game1() : base(width, height)
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 144f); //60);
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            Scene = new MasterScene();
            Scene.SetDefaultDesignResolution(width, height, Scene.SceneResolutionPolicy.None);
            Scene.SamplerState = new SamplerState()
            {
                Filter = TextureFilter.Anisotropic
            };
        }

        protected override void Draw(GameTime gameTime)
        {
            ((MasterScene)Scene).gameTime = gameTime;
            base.Draw(gameTime);
        }
    }
}
