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
        public static Dictionary<string, TmxTilesetTile> TilesDic = new Dictionary<string, TmxTilesetTile>();
        public static TmxTilesetTile GetTile(string name)
        {
            return TilesDic[name];
        }
    }
}
