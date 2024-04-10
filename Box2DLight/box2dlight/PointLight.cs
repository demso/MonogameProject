
using Box2DLight;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Box2dLight
{
    /// <summary>
    /// Light shaped as a circle with given radius
    /// 
    /// <p>Extends {@link PositionalLight}
    /// 
    /// @author kalle_h
    /// </summary>
    public class PointLight : PositionalLight
    {
        /// <summary>
        /// Creates light shaped as a circle with default radius (15f), color and
        /// position (0f, 0f)
        /// 
        /// @param rayHandler
        ///            not {@code null} instance of RayHandler
        /// @param rays
        ///            number of rays - more rays make light to look more realistic
        ///            but will decrease performance, can't be less than MIN_RAYS
        /// </summary>
        public PointLight(RayHandler rayHandler, int rays)
            : this(rayHandler, rays, Light.DefaultColor, 15f, 0f, 0f)
        {
        }

        /// <summary>
        /// Creates light shaped as a circle with given radius
        /// 
        /// @param rayHandler
        ///            not {@code null} instance of RayHandler
        /// @param rays
        ///            number of rays - more rays make light to look more realistic
        ///            but will decrease performance, can't be less than MIN_RAYS
        /// @param color
        ///            color, set to {@code null} to use the default color
        /// @param distance
        ///            distance of light, soft shadow length is set to distance * 0.1f
        /// @param x
        ///            horizontal position in world coordinates
        /// @param y
        ///            vertical position in world coordinates
        /// </summary>
        public PointLight(RayHandler rayHandler, int rays, Color color, float distance, float x, float y)
            : base(rayHandler, rays, color, distance, x, y, 0f)
        {
        }

        public override void Update()
        {
            UpdateBody();
            if (dirty) SetEndPoints();

            if (Cull()) return;
            if (staticLight && !dirty) return;

            dirty = false;
            UpdateMesh();
        }

        /// <summary>
        /// Sets light distance
        /// 
        /// <p>MIN value capped to 0.1f meter
        /// <p>Actual recalculations will be done only on {@link #update()} call
        /// </summary>
        public override void SetDistance(float dist)
        {
            dist *= RayHandler.gammaCorrectionParameter;
            this.distance = dist < 0.01f ? 0.01f : dist;
            dirty = true;
        }

        /// <summary>
        /// Updates light basing on it's distance and rayNum
        /// </summary>
        void SetEndPoints()
        {
            float angleNum = 360f / (rayNum - 1);
            for (int i = 0; i < rayNum; i++)
            {
                float angle = angleNum * i;
                sin[i] = (float) Math.Sin(MathHelper.ToRadians(angle));
                cos[i] = (float) Math.Cos(MathHelper.ToRadians(angle));
                endX[i] = distance * cos[i];
                endY[i] = distance * sin[i];
            }
        }

        /// <summary>
        /// Not applicable for this light type
        /// </summary>
        [Obsolete]
        public override void SetDirection(float directionDegree)
        {
        }
    }
}

