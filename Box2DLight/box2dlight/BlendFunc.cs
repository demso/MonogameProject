using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Box2DLight;
public class BlendFunc
{
    private Blend defaultSfactor;
    private Blend defaultDfactor;
    public Blend Sfactor { get; set; }
    public Blend Dfactor { get; set; }

    public BlendFunc(Blend sfactor, Blend dfactor)
    {
        defaultSfactor = sfactor;
        defaultDfactor = dfactor;
        Sfactor = sfactor;
        Dfactor = dfactor;
    }

    // Sets source and destination blending factors
    public void Set(Blend sfactor, Blend dfactor)
    {
        Sfactor = sfactor;
        Dfactor = dfactor;
    }

    // Resets source and destination blending factors to default values
    // that were set on instance creation
    public void Reset()
    {
        Sfactor = defaultSfactor;
        Dfactor = defaultDfactor;
    }

    // Calls glBlendFunc with own source and destination factors
    public void Apply()
    {
        // Assuming you have imported the appropriate OpenGL library
        BlendState bs = new BlendState();
        bs.Name = "mycustomblendstate";
        bs.AlphaSourceBlend = Sfactor;
        bs.ColorSourceBlend = Sfactor;
        bs.AlphaDestinationBlend = Dfactor;
        bs.ColorDestinationBlend = Dfactor;

        Core.GraphicsDevice.BlendState = bs;
    }

    public BlendState Get()
    {
        // Assuming you have imported the appropriate OpenGL library
        BlendState bs = new BlendState();
        bs.Name = "mycustomblendstate";
        bs.AlphaSourceBlend = Sfactor;
        bs.ColorSourceBlend = Sfactor;
        bs.AlphaDestinationBlend = Dfactor;
        bs.ColorDestinationBlend = Dfactor;

        return bs;
    }
}