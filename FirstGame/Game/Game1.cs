using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using Microsoft.Xna.Framework;
using Penumbra;

namespace FirstGame.Game
{

    public class Game1 : Core
    {
        static int width = 640;
        static int height = 480;
        private RenderTarget2D renderTarget;
        SpriteBatch spriteBatch;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);  
            renderTarget = new RenderTarget2D(Core.GraphicsDevice, Core.GraphicsDevice.PresentationParameters.BackBufferWidth, Core.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false, SurfaceFormat.ColorSRgb, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        protected override void Draw(GameTime gameTime)
        {
            ((MasterScene)Scene).gameTime = gameTime;
            base.Draw(gameTime);

            //((MasterScene)Scene).rh.setCombinedMatrix(Scene.Camera.ViewProjectionMatrix);
            ((MasterScene)Scene).rh.updateAndRender();

            //Core.GraphicsDevice.SetRenderTarget(renderTarget);
            //Core.GraphicsDevice.Clear(Color.Red);
            //Core.GraphicsDevice.SetRenderTarget(null);
            //Core.GraphicsDevice.Clear(Color.Black);


            //spriteBatch.Begin(SpriteSortMode.Immediate);
            //spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
            //spriteBatch.End();
        }
    }
}
