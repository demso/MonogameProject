using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Nez.UI;
using System;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Nez.Tiled;
using static FirstGame.Game.entyties.BodyData;
using FirstGame.Game.objects.bodies;

namespace FirstGame.Game.factories;

public class BodyResolver(World world)
{
    public enum Type
    {
        FullBody,
        MetalCloset,
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

    public Body ResolveBody(float x, float y, Object userData, Type type, Direction direction)
    {
        Body body = type switch
        {
            Type.FullBody => FullBody(x, y, userData),
            Type.MetalCloset => MetalClosetBody(x, y, userData),
            Type.Window => Window(x, y, userData, direction),
            _ => null
        };
        return body;
    }

    public Body MetalClosetBody(float x, float y, Object userData)
    {
        tempvec.X = x;
        tempvec.Y = y;
        Body body = new Body(world, tempvec, 0, BodyType.Static, userData);
        Fixture fixture = FixtureFactory.AttachRectangle(0.33f, 0.25f, 1, Vector2.Zero, body);

        return body;
    }

    public Body Window(float x, float y, Object userData, Direction direction)
    {
        tempvec.X = x;
        tempvec.Y = y;
        Body body = new Body(world, tempvec, 0, BodyType.Static, userData);

        Fixture fixture = direction switch
        {
            (Direction.North) => FixtureFactory.AttachRectangle(1f, 0.1f, 1, new Vector2(0, -0.4f), body),
            (Direction.South) => FixtureFactory.AttachRectangle(1f, 0.1f, 1, new Vector2(0, 0.4f), body),
            (Direction.East) => FixtureFactory.AttachRectangle(0.1f, 1f, 1, new Vector2(0.4f, 0), body),
            (Direction.West) => FixtureFactory.AttachRectangle(0.1f, 1f, 1, new Vector2(-0.4f, 0), body)
        };

        fixture.CollisionGroup = Globals.TRANSPARENT_GROUP;

        return body;
    }

    public Body FullBody(float x, float y, Object userData)
    {
        tempvec.X = x;
        tempvec.Y = y;
        Body body = new Body(world, tempvec, 0, BodyType.Static, userData);
        Fixture fixture = FixtureFactory.AttachRectangle(1f, 1f, 1, Vector2.Zero, body);

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

    public Direction GetDirection(TmxLayerTile layerTile)
    {

        bool southWard = !layerTile.DiagonalFlip &&  layerTile.VerticalFlip,
             northWard = !layerTile.DiagonalFlip &&  !layerTile.VerticalFlip,
             eastWard = layerTile.DiagonalFlip && layerTile.HorizontalFlip,
             westWard = layerTile.DiagonalFlip && !layerTile.HorizontalFlip;

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

    public static Filter CreateFilter(short mask, short category, short group)
    {
        Filter filter = new Filter();
        filter.maskBits = mask;
        filter.categoryBits = category;
        filter.groupIndex = group;
        return filter;
    }
}
