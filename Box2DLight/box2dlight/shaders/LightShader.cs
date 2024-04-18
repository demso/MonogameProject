using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Box2DLight.box2dlight.shaders
{
    internal class LightShader
    {
        static public Effect createLightShader()
        {
            Effect tmp = Core.Content.Load<Effect>(@"assets\shaders\LightEffect");

            return tmp;
        }
    }
}
