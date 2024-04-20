using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using FirstGame.Game.objects.bodies.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Farseer;
using Nez.Sprites;
using Nez.Textures;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FirstGame.Game.components
{
    internal class PlayerRenderer : SpriteRenderer
    {
        private Player player;
        public PlayerRenderer(Texture2D texture) : base(texture)
        {
        }

        public PlayerRenderer(Sprite sprite) : base(sprite)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            player = MasterScene.Instance.player;
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
