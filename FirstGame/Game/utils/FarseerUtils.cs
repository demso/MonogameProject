using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FirstGame.Game.utils
{
    public static class FarseerUtils
    {
        //public static float minX(Body body)
        //{
        //    float x = float.PositiveInfinity, tmp;
        //    Array<Fixture> fixtures = body.getFixtureList();
        //    for (int i = 0; i < fixtures.size; i++)
        //        if ((tmp = minX(fixtures.get(i))) < x)
        //            x = tmp;
        //    return x;
        //}

        ///** @return the minimal y value of the vertices of all fixtures of the the given Body */
        //public static float minY(Body body)
        //{
        //    float y = float.PositiveInfinity, tmp;
        //    Array<Fixture> fixtures = body.getFixtureList();
        //    for (int i = 0; i < fixtures.size; i++)
        //        if ((tmp = minY(fixtures.get(i))) < y)
        //            y = tmp;
        //    return y;
        //}

        ///** @return the maximal x value of the vertices of all fixtures of the the given Body */
        //public static float maxX(Body body)
        //{
        //    float x = float.PositiveInfinity, tmp;
        //    Array<Fixture> fixtures = body.getFixtureList();
        //    for (int i = 0; i < fixtures.size; i++)
        //        if ((tmp = maxX(fixtures.get(i))) > x)
        //            x = tmp;
        //    return x;
        //}

        ///** @return the maximal y value of the vertices of all fixtures of the the given Body */
        //public static float maxY(Body body)
        //{
        //    float y = float.PositiveInfinity, tmp;
        //    Array<Fixture> fixtures = body.getFixtureList();
        //    for (int i = 0; i < fixtures.size; i++)
        //        if ((tmp = maxY(fixtures.get(i))) > y)
        //            y = tmp;
        //    return y;
        //}

        //public static float minX(Fixture fixture)
        //{
        //    return minX(fixture.getShape());
        //}

        ///** @see #minY(Shape) */
        //public static float minY(Fixture fixture)
        //{
        //    return minY(fixture.getShape());
        //}

        ///** @see #maxX(Shape) */
        //public static float maxX(Fixture fixture)
        //{
        //    return maxX(fixture.getShape());
        //}

        ///** @see #maxY(Shape) */
        //public static float maxY(Fixture fixture)
        //{
        //    return maxY(fixture.getShape());
        //}

        //public static float minX(Shape shape)
        //{
        //    if (cache.containsKey(shape))
        //        return cache.get(shape).minX;
        //    if (autoCache)
        //        return cache(shape).minX;
        //    return minX0(shape);
        //}

        ///** @return the minimal y value of the vertices of the given Shape */
        //public static float minY(Shape shape)
        //{
        //    if (cache.containsKey(shape))
        //        return cache.get(shape).minY;
        //    if (autoCache)
        //        return cache(shape).minY;
        //    return minY0(shape);
        //}

        ///** @return the maximal x value of the vertices of the given Shape */
        //public static float maxX(Shape shape)
        //{
        //    if (cache.containsKey(shape))
        //        return cache.get(shape).maxX;
        //    if (autoCache)
        //        return cache(shape).maxX;
        //    return maxX0(shape);
        //}

        ///** @return the maximal y value of the vertices of the given Shape */
        //public static float maxY(Shape shape)
        //{
        //    if (cache.containsKey(shape))
        //        return cache.get(shape).maxY;
        //    if (autoCache)
        //        return cache(shape).maxY;
        //    return maxY0(shape);
        //}
    }
}
