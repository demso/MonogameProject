using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FirstGame.Game.entyties;
using FirstGame.Game.factories;
using FirstGame.Game.objects.tiles;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Farseer;
using Nez.Tiled;
using Nez.UI;

namespace FirstGame.Game.tiled;

public class TiledLoader
{
    private static BodyResolver resolver;
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
        resolver = new BodyResolver(world);
        ITmxLayer tempLayer;
        map.Layers.TryGetValue("obstacles", out tempLayer);
        TmxLayer obstacleLayer = (TmxLayer) tempLayer;

        


        foreach ( TmxLayerTile ltile in obstacleLayer.Tiles)
        {
            if (ltile != null)
                LoadTile(ltile);
        }
    }

    static void LoadTile(TmxLayerTile cell)
    {
        var tileProperties = cell.TilesetTile.Properties;
        Body body;
        string bodyType = null;
        try {

            if (cell == null || cell.TilesetTile == null || cell.TilesetTile.Properties == null)
                return;

            if (!cell.TilesetTile.Properties.TryGetValue("body type", out bodyType))
                return;

            Enum.TryParse(bodyType, true, out BodyResolver.Type realBodyType);

            if (bodyType == null) 
                return;

            body = resolver.ResolveBody(cell.X + 0.5f, cell.Y + 0.5f, new SimpleBodyUserData(cell, bodyType), realBodyType, resolver.GetDirection(cell));

            Entity sceneEntity = new Entity();
            //sceneEntity.SetTag(Globals.TAG_FOR_FIXED_UPDATE);
            MasterScene.Instance.AddEntity(sceneEntity);
            sceneEntity.AddComponent(new FSGenericBody(body));

        string name = tileProperties.GetValueOrDefault("name", null);

        List<string> nameEntries = new List<string>(name.Split('_'));

        BodyData bodyData = null;

        if (nameEntries[0] == "t")
        {
            switch (nameEntries[1])
            {
                case "door":
                    Door door = new Door(cell, body);
                    if (nameEntries.Contains("o"))
                    {
                        door.OpenTile = Tiles.GetTile(name);
                        int index = nameEntries.IndexOf("o");
                        nameEntries[index] = "c";
                        door.ClosedTile = Tiles.GetTile(string.Join("_", nameEntries));
                        door.Open();
                    }
                    else if (nameEntries.Contains("c"))
                    {
                        door.ClosedTile = Tiles.GetTile(name);
                        int index = nameEntries.IndexOf("c");
                        nameEntries[index] = "o";
                        door.OpenTile = Tiles.GetTile(string.Join("_", nameEntries));
                        door.Close();
                    }
                    if (nameEntries.Contains("boarded")) door.Board();
                    if (nameEntries.Contains("peep")) door.Peep = true;
                    bodyData = door;
                    break;
                //case "window":
                //    Window window = new Window(cell, body);
                //    if (nameEntries.Contains("o"))
                //    {
                //        window.OpenTile = Tiles.GetTile(name);
                //        int index = nameEntries.IndexOf("o");
                //        nameEntries[index] = "c";
                //        window.ClosedTile = Tiles.GetTile(string.Join("_", nameEntries));
                //        window.Open();
                //    }
                //    else if (nameEntries.Contains("c"))
                //    {
                //        window.ClosedTile = Tiles.GetTile(name);
                //        int index = nameEntries.IndexOf("c");
                //        nameEntries[index] = "o";
                //        window.OpenTile = Tiles.GetTile(string.Join("_", nameEntries));
                //        window.Close();
                //    }
                //    bodyData = window;
                //    break;
                //case "closet":
                //    Closet closet = new Closet(cell, body);
                //    bodyData = closet;
                //    break;
                default:
                    bodyData = new SimpleBodyUserData(cell, name);
                    break;
            }
        }

        nameEntries.Clear();

        body.UserData = bodyData;

        } catch (Exception e) {
            Helper.Log("[Error] [TileInitializer] Problem with creating tile \n  " +
                       "name " + cell.TilesetTile?.Properties?.GetValueOrDefault("name", null) + 
                       ",  body type " + bodyType + 
                       ", direction " + resolver.GetDirection(cell) + 
                       " at x: " + cell.X + 
                       ", y: " + cell.Y + 
                       " \n " + e.Message);
        }



    }
}