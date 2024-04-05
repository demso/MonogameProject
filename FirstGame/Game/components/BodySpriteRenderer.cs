using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Farseer;
using Nez.Sprites;
using Nez.Textures;

namespace FirstGame.Game.components
{
    internal class BodySpriteRenderer : SpriteRenderer
    {
        public BodySpriteRenderer(Texture2D texture) : base(texture)
        { }

        public BodySpriteRenderer(Sprite sprite) : base(sprite)
        { }
        
        public override void Render(Batcher batcher, Camera camera)
        {
            batcher.Draw(Sprite, Entity.Position + LocalOffset, Color,
                Entity.Transform.Rotation, Origin, Entity.Transform.Scale, SpriteEffects, _layerDepth);
        }
    }
}
