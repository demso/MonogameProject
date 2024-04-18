using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez.Tiled;

namespace FirstGame.Game.factories
{
    public static class Tiles
    {
        public static Dictionary<string, TmxTilesetTile> TilesDic;
        public static TmxTilesetTile getTile(String name)
        {
            return TilesDic[name];
        }
    }
}
