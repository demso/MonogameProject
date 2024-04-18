using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DLight;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;

namespace FirstGame.Game
{
    internal class LightRenderer : Renderer
    {
        RayHandler _rayHandler;
        public LightRenderer(RayHandler rh, int renderOrder = 1) : base(renderOrder)
        {
            _rayHandler = rh;
        }

        public override void Render(Scene scene)
        {
            var cam = Camera ?? scene.Camera;

            //BeginRender(cam);

            _rayHandler.setCombinedMatrix(cam.ViewProjectionMatrix, 0, 0, Core.GraphicsDevice.DisplayMode.Width, Core.GraphicsDevice.DisplayMode.Height);
            _rayHandler.updateAndRender(scene.SceneRenderTarget);

            //EndRender();
        }
    }
}
