using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Box2DLight.box2dlight.shaders
{
    internal class ShadowShader
    {
        static public Effect CreateShadowShader()
        {
            Effect tmp = Core.Content.Load<Effect>(@"assets\shaders\ShadowEffect");

            return tmp;
        }
    }
}
