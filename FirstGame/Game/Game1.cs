using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace FirstGame.Game
{

    public class Game1 : Core
    {
        static int width = 640;
        static int height = 480;
        public Game1() : base(width, height)
        {
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
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
    }
}
