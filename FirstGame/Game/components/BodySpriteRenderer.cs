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
        public BodySpriteRenderer()
        { }

        public BodySpriteRenderer(Texture2D texture) : base(texture)
        { }

        public BodySpriteRenderer(Sprite sprite) : base(sprite)
        { }

        //public void Draw(
        //    Texture2D texture,
        //    Vector2 position,
        //    Rectangle? sourceRectangle,
        //    Color color,
        //    float rotation,
        //    Vector2 origin,
        //    Vector2 scale,
        //    SpriteEffects effects,
        //    float layerDepth);

        
        public override void Render(Batcher batcher, Camera camera)
        {
            //sb.Begin();
            //sb.Draw(Sprite.Texture2D, Entity.GetComponent<FSRigidBody>().Body.DisplayPosition + LocalOffset, Sprite.SourceRect, Color,
            //    Entity.Transform.Rotation, Origin, Entity.Transform.Scale, SpriteEffects, _layerDepth);
            //sb.Begin();
            //Graphics.Instance.Batcher.ShouldRoundDestinations = false;
            //Graphics.Instance.Batcher.SetIgnoreRoundingDestinations(true);
            batcher.Draw(Sprite, Entity.Position + LocalOffset, Color,
                Entity.Transform.Rotation, Origin, Entity.Transform.Scale, SpriteEffects, _layerDepth);
        }
    }
}
