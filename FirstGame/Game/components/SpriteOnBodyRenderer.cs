using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Nez.Sprites;

namespace FirstGame.Game.components
{
    internal class SpriteOnBodyRenderer : SpriteRenderer
    {
        public override void Render(Batcher batcher, Camera camera)
        {
            batcher.Draw(Sprite, Entity.Transform.Position + LocalOffset, Color,
                Entity.Transform.Rotation, Origin, Entity.Transform.Scale, SpriteEffects, _layerDepth);
        }
    }
}
