using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstGame.Game.objects.bodies
{
    public class Filter
    {
        /** The collision category bits. Normally you would just set one bit. */
        public short categoryBits = 0x0001;

        /** The collision mask bits. This states the categories that this shape would accept for collision. */
        public short maskBits = -1;

        /** Collision groups allow a certain group of objects to never collide (negative) or always collide (positive). Zero means no
         * collision group. Non-zero group filtering always wins against the mask bits. */
        public short groupIndex = 0;

        public Filter(short categoryBits = 0x0001, short maskBits = -1, short groupIndex = 0)
        {
            this.categoryBits = categoryBits;
            this.maskBits = maskBits;
            this.groupIndex = groupIndex;
        }

        public void set(Filter filter)
        {
            categoryBits = filter.categoryBits;
            maskBits = filter.maskBits;
            groupIndex = filter.groupIndex;
        }

        public static void ApplyFilter(Filter filter, Body body)
        {
            body.CollidesWith = (Category)filter.maskBits;
            body.CollisionCategories = (Category)filter.categoryBits;
            body.CollisionGroup = filter.groupIndex;
        }

        public static void ApplyFilter(Filter filter, Fixture fixture)
        {
            fixture.CollidesWith = (Category)filter.maskBits;
            fixture.CollisionCategories = (Category)filter.categoryBits;
            fixture.CollisionGroup = filter.groupIndex;
        }
    }
}
