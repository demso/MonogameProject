using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstGame.Game.factories;

namespace FirstGame.Game.tiled
{
    internal class MyTilesetTile : TmxTilesetTile
    {
        public override void ProcessProperties()
        {
            base.ProcessProperties();
            BodyTileResolver resolver = new(world: MasterScene.world);
            string value;
            if (Properties.TryGetValue("type", out value))
                SlopeTopRight = int.Parse(value);
            BodyTileResolver.Type bodyType;
           // resolver.resolveBody(0, 0, BodyTileResolver.Type.TryParse(value, true, out bodyType), );
        }
    }
}
