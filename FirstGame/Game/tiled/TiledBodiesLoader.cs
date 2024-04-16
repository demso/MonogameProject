using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FirstGame.Game.entyties;
using FirstGame.Game.factories;
using Microsoft.Xna.Framework;
using Nez.Farseer;
using Nez.Tiled;

namespace FirstGame.Game.tiled;

public class TiledBodiesLoader
{
    private BodyTileResolver resolver;
    private World world;
    internal MasterScene _masterScene;

    internal TiledBodiesLoader(MasterScene masterScene)
    {
        _masterScene = masterScene;
    }

    public void LoadBodies(TmxMap map)
    {
        world = MasterScene.world;
        resolver = new BodyTileResolver(world);
        ITmxLayer tempLayer;
        map.Layers.TryGetValue("obstacles", out tempLayer);
        TmxLayer obstacleLayer = (TmxLayer) tempLayer;
        
        foreach ( TmxLayerTile ltile in obstacleLayer.Tiles)
        {
            if (ltile == null || ltile.TilesetTile == null || ltile.TilesetTile.Properties == null)
                continue;
            string bodyType;
            if (!ltile.TilesetTile.Properties.TryGetValue("body type", out bodyType))
                continue;
            BodyTileResolver.Type realBodyType;
            Enum.TryParse(bodyType, true, out realBodyType);
            Body body = resolver.resolveBody(ltile.X+0.5f, ltile.Y+0.5f, new SimpleBodyUserData(ltile, bodyType), realBodyType, resolver.getDirection(ltile));
        }
    }
}