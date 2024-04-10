using Microsoft.Xna.Framework;
using Nez;

namespace FirstGame.Game;

public class MyRenderer : Renderer
{
    public MyRenderer(int renderOrder = 0, Camera camera = null) : base(renderOrder, camera)
    {
    }

    public override void Render(Scene scene)
    {
        var cam = Camera ?? scene.Camera;
        
        
        ((MasterScene)scene).penumbra.BeginDraw();
        Core.GraphicsDevice.Clear(Color.White);
        BeginRender(cam);
        RenderAfterStateCheck(scene.FindComponentOfType<TiledMapRenderer>(), cam);
        EndRender();
        ((MasterScene)scene).penumbra.Draw(((MasterScene)scene).gameTime);
         
        
        BeginRender(cam);

        for (var i = 0; i < scene.RenderableComponents.Count; i++)
        {
            var renderable = scene.RenderableComponents[i];
            if (renderable.Enabled && renderable.IsVisibleFromCamera(cam) && renderable is not TiledMapRenderer )
                RenderAfterStateCheck(renderable, cam);
        }

        if (ShouldDebugRender && Core.DebugRenderEnabled)
            DebugRender(scene, cam);

        EndRender();
       
        
    }
}