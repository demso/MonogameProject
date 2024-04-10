using System;
using System.Collections.Generic;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Box2DLight
{
    public abstract class Light : IDisposable
    {
        protected static readonly Color DefaultColor = new Color(0.75f, 0.75f, 0.5f, 0.75f);
        protected static readonly float ZeroColorBits = Color.Transparent.PackedValue;
        protected static readonly float OneColorBits = Color.White.PackedValue;
        protected const int MinRays = 3;

        protected Color color = new Color();
        protected Vector2 tmpPosition = new Vector2();

        protected RayHandler rayHandler;

        protected bool active = true;
        protected bool soft = true;
        protected bool xray = false;
        protected bool staticLight = false;
        protected bool culled = false;
        protected bool dirty = true;
        protected bool ignoreBody = false;

        protected int rayNum;
        //protected int vertexNum;
        protected int lightVertexNum;
        protected int softShadowVertexNum;

        protected float distance;
        protected float direction;
        protected float colorF;
        protected float softShadowLength = 2.5f;

        protected VertexBuffer lightMesh;
        protected VertexBuffer softShadowMesh;

        //protected float[] segments;
        protected float[] mx;
        protected float[] my;
        protected float[] f;
        protected int m_index = 0;

        protected static readonly LightData tmpData = new LightData(0f);

        //protected float pseudo3dHeight = 0f;

        protected readonly List<VertexBuffer> dynamicShadowMeshes = new List<VertexBuffer>();
        protected readonly List<Fixture> affectedFixtures = new List<Fixture>();
        protected readonly List<Vector2> tmpVerts = new List<Vector2>();

        protected readonly List<int> ind = new List<int>();

        protected readonly Vector2 tmpStart = new Vector2();
        protected readonly Vector2 tmpEnd = new Vector2();
        protected readonly Vector2 tmpVec = new Vector2();
        protected readonly Vector2 center = new Vector2();

        public Light(RayHandler rayHandler, int rays, Color color, float distance, float directionDegree)
        {
            rayHandler.lightList.Add(this);
            this.rayHandler = rayHandler;
            SetRayNum(rays);
            SetColor(color);
            SetDistance(distance);
            SetSoftnessLength(distance * 0.1f);
            SetDirection(directionDegree);
        }

        public abstract void Update();

        public abstract void Render();

        //protected void DynamicShadowRender()
        //{
        //    foreach (VertexBuffer m in dynamicShadowMeshes)
        //    {
        //        //m.Render(rayHandler.lightShader, PrimitiveType.TriangleStrip);
        //        rayHandler.lightShader.CurrentTechnique.Passes[0].Apply();
        //        Core.GraphicsDevice.SetVertexBuffer(m);
        //        Core.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m.VertexCount);
        //    }
        //}

        public abstract void SetDistance(float dist);

        public abstract void SetDirection(float directionDegree);

        public abstract void AttachToBody(Body body);

        public abstract Body GetBody();

        public abstract void SetPosition(float x, float y);

        public abstract void SetPosition(Vector2 position);

        public abstract float GetX();

        public abstract float GetY();

        public virtual Vector2 GetPosition()
        {
            return tmpPosition;
        }

        public void SetColor(Color newColor)
        {
            if (newColor != null)
            {
                color = newColor;
            }
            else
            {
                color = DefaultColor;
            }
            colorF = color.PackedValue;
            if (staticLight) dirty = true;
        }

        public void SetColor(float r, float g, float b, float a)
        {
            color = new Color(r, g, b, a);
            colorF = color.PackedValue;
            if (staticLight) dirty = true;
        }

        public void Add(RayHandler rayHandler)
        {
            this.rayHandler = rayHandler;
            if (active)
            {
                rayHandler.lightList.Add(this);
            }
            else
            {
                rayHandler.disabledLights.Add(this);
            }
        }

        public void Remove()
        {
            Remove(true);
        }

        public void Remove(bool doDispose)
        {
            if (active)
            {
                rayHandler.lightList.Remove(this);
            }
            else
            {
                rayHandler.disabledLights.Remove(this);
            }
            rayHandler = null;
            if (doDispose) Dispose();
        }

        public void Dispose()
        {
            affectedFixtures.Clear();
            //lightMesh.Dispose();
            //softShadowMesh.Dispose();
            //foreach (Mesh mesh in dynamicShadowMeshes)
            //{
            //    mesh.Dispose();
            //}
            dynamicShadowMeshes.Clear();
        }

        public bool IsActive()
        {
            return active;
        }

        public void SetActive(bool active)
        {
            if (active == this.active)
                return;

            this.active = active;
            if (rayHandler == null)
                return;

            if (active)
            {
                rayHandler.lightList.Add(this);
                rayHandler.disabledLights.Remove(this);
            }
            else
            {
                rayHandler.disabledLights.Add(this);
                rayHandler.lightList.Remove(this);
            }
        }

        public bool IsXray()
        {
            return xray;
        }

        public void SetXray(bool xray)
        {
            this.xray = xray;
            if (staticLight) dirty = true;
        }

        public bool IsStaticLight()
        {
            return staticLight;
        }

        public void SetStaticLight(bool staticLight)
        {
            this.staticLight = staticLight;
            if (staticLight) dirty = true;
        }

        public bool IsSoft()
        {
            return soft;
        }

        public void SetSoft(bool soft)
        {
            this.soft = soft;
            if (staticLight) dirty = true;
        }

        public float GetSoftShadowLength()
        {
            return softShadowLength;
        }

        public void SetSoftnessLength(float softShadowLength)
        {
            this.softShadowLength = softShadowLength;
            if (staticLight) dirty = true;
        }

        public Color GetColor()
        {
            return color;
        }

        public float GetDistance()
        {
            return distance / RayHandler.gammaCorrectionParameter;
        }

        public float GetDirection()
        {
            return direction;
        }

        public virtual bool Contains(float x, float y)
        {
            return false;
        }

        public void SetIgnoreAttachedBody(bool flag)
        {
            ignoreBody = flag;
        }

        public bool GetIgnoreAttachedBody()
        {
            return ignoreBody;
        }

        //public void SetHeight(float height)
        //{
        //    pseudo3dHeight = height;
        //}

        protected virtual void SetRayNum(int rays)
        {
            if (rays < MinRays)
                rays = MinRays;

            rayNum = rays;
            //vertexNum = rays * 3;
            lightVertexNum = rays * 3;
            softShadowVertexNum = rays * 2;

            //segments = new float[vertexNum * 8];
            mx = new float[rays + 1];
            my = new float[rays + 1];
            f = new float[rays + 1];
        }

        public int GetRayNum()
        {
            return rayNum;
        }

        public static Category? GlobalCollidesWith = null;
        public static Category? GlobalCollisionCategories = null;
        public static short GlobalCollisionGroup;
        private Category? _collidesWith = null;
        private Category? _collisionCategories = null;
        private short _collisionGroup;
        public Category? CollidesWith
        {
            get => _collidesWith;

            set
            {
                if (_collidesWith == value)
                    return;

                _collidesWith = value;
            }
        }
        public Category? CollisionCategories
        {
            get => _collisionCategories;

            set
            {
                if (_collisionCategories == value)
                    return;

                _collisionCategories = value;
            }
        }
        public short CollisionGroup
        {
            set
            {
                if (_collisionGroup == value)
                    return;

                _collisionGroup = value;
            }
            get => _collisionGroup;
        }

        internal float ray (Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if ((GlobalCollidesWith != null || GlobalCollisionGroup != 0) && !GlobalContactFilter(fixture))
                return -1;

            if ((CollidesWith != null || CollisionGroup != 0) && !ContactFilter(fixture))
                return -1;

            if (ignoreBody && fixture.Body == GetBody())
                return -1;
            mx[m_index] = point.X;
            my[m_index] = point.Y;
            f[m_index] = fraction;
            return fraction;
        }

        bool ContactFilter(Fixture fixtureB)
        {
            if (CollisionGroup != 0 &&
                CollisionGroup == fixtureB.CollisionGroup)
                return CollisionGroup > 0;

            return ((this.CollidesWith & fixtureB.CollisionCategories) == Category.None) &
                   ((this.CollisionCategories & fixtureB.CollidesWith) == Category.None);
        }

        public void SetContactFilter(Category collisionCategories, Category collidesWith, short collideGroup)
        {
            CollisionCategories = collisionCategories;
            CollidesWith = collidesWith;
            CollisionGroup = collideGroup;
        }

        bool GlobalContactFilter(Fixture fixtureB)
        {
            if (GlobalCollisionGroup != 0 &&
                GlobalCollisionGroup == fixtureB.CollisionGroup)
                return GlobalCollisionGroup > 0;

            return ((GlobalCollidesWith & fixtureB.CollisionCategories) == Category.None) &
                   ((CollisionCategories & fixtureB.CollidesWith) == Category.None);
        }

        public static void SetGlobalContactFilter(Category collisionCategories, Category collidesWith, short collideGroup)
        {
            GlobalCollidesWith = collidesWith;
            GlobalCollisionGroup = collideGroup;
            GlobalCollisionCategories = collisionCategories;
        }

        //protected bool OnDynamicCallback(Fixture fixture)
        //{
        //    if ((globalFilterA != null) && !GlobalContactFilter(fixture))
        //    {
        //        return false;
        //    }

        //    if ((filterA != null) && !ContactFilter(fixture))
        //    {
        //        return false;
        //    }

        //    if (ignoreBody && fixture.Body == GetBody())
        //    {
        //        return false;
        //    }
        //    return !affectedFixtures.Contains(fixture);
        //}

        //internal readonly QueryCallback dynamicShadowCallback = new QueryCallback(fixture =>
        //{
        //    if (!OnDynamicCallback(fixture))
        //    {
        //        return true;
        //    }
        //    affectedFixtures.Add(fixture);
        //    if (fixture.UserData is LightData data)
        //    {
        //        data.ShadowsDropped++;
        //    }
        //    return true;
        //});
    }
}


