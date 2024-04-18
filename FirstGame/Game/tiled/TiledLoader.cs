using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FirstGame.Game.entyties;
using FirstGame.Game.factories;
using Microsoft.Xna.Framework;
using Nez.Farseer;
using Nez.Tiled;

namespace FirstGame.Game.tiled;

public class TiledLoader
{
    private static BodyTileResolver resolver;
    private static World world;
    internal static MasterScene _masterScene = MasterScene.Instance;


    public static void Load(TmxMap map)
    {
        TmxList<TmxTileset> tilesets = map.Tilesets;

        foreach (var tmxTileset in tilesets)
        {
            foreach (var tmxTilesetTile in tmxTileset.Tiles.Values)
            {
                string tileName = tmxTilesetTile.Properties["name"];
                if (!string.IsNullOrWhiteSpace(tileName))
                {
                    Tiles.TilesDic.Add(tileName, tmxTilesetTile);
                } 
            }
        }

        world = MasterScene.Instance.world.World;
        resolver = new BodyTileResolver(world);
        ITmxLayer tempLayer;
        map.Layers.TryGetValue("obstacles", out tempLayer);
        TmxLayer obstacleLayer = (TmxLayer) tempLayer;

        Body body;


        foreach ( TmxLayerTile ltile in obstacleLayer.Tiles)
        {
            if (ltile == null || ltile.TilesetTile == null || ltile.TilesetTile.Properties == null)
                continue;
            string bodyType;
            if (!ltile.TilesetTile.Properties.TryGetValue("body type", out bodyType))
                continue;
            BodyTileResolver.Type realBodyType;
            Enum.TryParse(bodyType, true, out realBodyType);
            body = resolver.resolveBody(ltile.X+0.5f, ltile.Y+0.5f, new SimpleBodyUserData(ltile, bodyType), realBodyType, resolver.getDirection(ltile));
        }
    }
}