using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Farseer;
using Nez.Sprites;
using Nez.Textures;

namespace FirstGame.Game.components
{
    internal class BodySpriteRenderer : SpriteRenderer
    {
        Vector2 oldPos = new Vector2();
        Vector2 pos2 = new Vector2();
        Vector2 pos3 = new Vector2();

        public BodySpriteRenderer(Texture2D texture) : base(texture)
        {
        }

        public BodySpriteRenderer(Sprite sprite) : base(sprite)
        {
            
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            oldPos = Entity.Position;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            //if (!MasterScene.Toggle)
            //    oldPos = Vector2.Lerp(oldPos, Entity.Position + LocalOffset, 0.5f);
            //else
            //    oldPos = Entity.Position;

            batcher.Draw(Sprite, Entity.Position + LocalOffset, Color,
                Entity.Transform.Rotation, Origin, Entity.Transform.Scale, SpriteEffects, _layerDepth);
        }
    }
}
