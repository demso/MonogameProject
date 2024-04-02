using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Nez.UI;
using System;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Nez.Tiled;

namespace FirstGame.Game.factories;

public class BodyTileResolver
{
    World world;
    public enum Type
    {
        FullBody,
        MetalClosetBody,
        Window,
    }
    public enum Direction
    {
        North,
        South,
        West,
        East
    }

    private Vector2 tempvec = new Vector2();

    BodyTileResolver(World world)
    {
        this.world = world;
    }
    public Body resolveBody(float x, float y, Object userData, Type type, Direction direction)
    {
        Body body = type switch
        {
            Type.FullBody => fullBody(x, y, userData),
            Type.MetalClosetBody => metalClosetBody(x, y, userData),
            Type.Window => window(x, y, userData, direction),
            _ => null
        };
        return body;
    }

    public Body resolveBody(float x, float y, Object userData, Type type)
    {
        Body body = type switch
        {
            Type.FullBody => fullBody(x, y, userData),
            Type.MetalClosetBody => metalClosetBody(x, y, userData),
            _ => null
        };
        return body;
    }

    public Body metalClosetBody(float x, float y, Object userData)
    {
        tempvec.X = x;
        tempvec.Y = y;
        Body body = new Body(world, tempvec, 0, BodyType.Dynamic, userData);
        Fixture fixture = FixtureFactory.AttachRectangle(0.33f, 0.25f, 1, Vector2.Zero, body);

        return body;
    }

    public Body window(float x, float y, Object userData, Direction direction)
    {
        //BodyDef windowHorBodyDef = new BodyDef();
        //PolygonShape windowHorBox = new PolygonShape();
        //FixtureDef windowHorFixtureDef = new FixtureDef();
        //windowHorBox.setAsBox(0.5f, 0.05f);
        //windowHorFixtureDef.shape = windowHorBox;
        //windowHorFixtureDef.filter.groupIndex = -10;

        //BodyDef windowVertBodyDef = new BodyDef();
        //PolygonShape windowVertBox = new PolygonShape();
        //FixtureDef windowVertFixtureDef = new FixtureDef();
        //windowVertBox.setAsBox(0.05f, 0.5f);
        //windowVertFixtureDef.shape = windowVertBox;
        //windowVertFixtureDef.filter.groupIndex = -10;

        tempvec.X = x;
        tempvec.Y = y;
        Body body = new Body(world, tempvec, 0, BodyType.Dynamic, userData);

        Fixture fixture = direction switch
        {
            (Direction.North or Direction.South) => FixtureFactory.AttachRectangle(1f, 0.1f, 1, Vector2.Zero, body),
            (Direction.East or Direction.West) => FixtureFactory.AttachRectangle(0.1f, 1f, 1, Vector2.Zero, body)
        };

        fixture.CollisionGroup = -Globals.LightCG;

        return body;
    }

public Body fullBody(float x, float y, Object userData)
{
    //BodyDef fullBodyDef = new BodyDef();
    //fullBodyDef.position.set(x, y);

    //PolygonShape fullBox = new PolygonShape();

    //FixtureDef fullFixtureDef = new FixtureDef();
    //fullBox.setAsBox(0.5f, 0.5f);
    //fullFixtureDef.shape = fullBox;
    //fullFixtureDef.filter.groupIndex = 0;

    //Body body = world.createBody(fullBodyDef);
    //body.createFixture(fullFixtureDef);
    //body.setUserData(userData);

    tempvec.X = x;
    tempvec.Y = y;
    Body body = new Body(world, tempvec, 0, BodyType.Dynamic, userData);
    Fixture fixture = FixtureFactory.AttachRectangle(0.5f, 0.5f, 1, Vector2.Zero, body);

    return body;
}

//public Body transparentFullBody(float x, float y, Object userData)
//{
//    BodyDef transparentBodyDef = new BodyDef();
//    PolygonShape transparentBox = new PolygonShape();
//    FixtureDef transparentFixtureDef = new FixtureDef();
//    transparentBox.setAsBox(x, y);
//    transparentFixtureDef.shape = transparentBox;
//    transparentFixtureDef.filter.groupIndex = -10;

//    Body body = world.createBody(transparentBodyDef);
//    body.createFixture(transparentFixtureDef);
//    body.setUserData(userData);
//    return body;
//}

    public Direction getDirection(TmxLayerTile layerTile)
    {
        //boolean southWard = cell.getRotation() == TiledMapTileLayer.Cell.ROTATE_0 && cell.getFlipVertically() && cell.getFlipVertically();
        //boolean northWard = cell.getRotation() == TiledMapTileLayer.Cell.ROTATE_0 && !cell.getFlipVertically() && !cell.getFlipVertically();
        //boolean eastWard = cell.getRotation() == TiledMapTileLayer.Cell.ROTATE_270 && !cell.getFlipVertically() && !cell.getFlipVertically();
        //boolean westWard = cell.getRotation() == TiledMapTileLayer.Cell.ROTATE_90 && !cell.getFlipVertically() && !cell.getFlipVertically();

        bool southWard = !layerTile.DiagonalFlip && !layerTile.HorizontalFlip && layerTile.VerticalFlip,
             northWard = !layerTile.DiagonalFlip && !layerTile.HorizontalFlip && !layerTile.VerticalFlip,
             eastWard = layerTile.DiagonalFlip && layerTile.HorizontalFlip && !layerTile.VerticalFlip,
             westWard = layerTile.DiagonalFlip && !layerTile.HorizontalFlip && layerTile.VerticalFlip;

        if (northWard)
            return Direction.North;
        if (southWard)
            return Direction.South;
        if (westWard)
            return Direction.West;
        if (eastWard)
            return Direction.East;

        return Direction.North;
    }
}
