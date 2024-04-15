

using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using Microsoft.Xna.Framework;
using Penumbra;
using Nez.Farseer;
using System.Diagnostics;

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
            this.IsFixedTimeStep = true;//false;
            //Screen.SynchronizeWithVerticalRetrace = true;
            //Screen.ApplyChanges();
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 144f); //60);
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

            //MasterScene.rh.RenderHere = Scene.SceneRenderTarget;
            //MasterScene.rh.updateAndRender();
            //MasterScene.rh.setCombinedMatrix(Scene.Camera.ViewProjectionMatrix, 0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);

            if (msc.phDebug)
            {
                //msc.debugView.RemoveFlags(FSDebugView.DebugViewFlags.Shape);
                //msc.debugView.AppendFlags(FSDebugView.DebugViewFlags.PolygonPoints);
                msc.debugView.Render(Graphics.Instance.Batcher, msc.Camera);
            }
        }
    }
}
