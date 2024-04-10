using System;
using Box2DLight;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Transform = FarseerPhysics.Common.Transform;
using VertexPositionColorNormal = Microsoft.Xna.Framework.Graphics.VertexPositionColorNormal;

namespace Box2DLight
{
    public abstract class PositionalLight : Light
    {
        Color tmpColor = new Color();

        protected Vector2 tmpEnd = new Vector2();
        protected Vector2 start = new Vector2();

        protected Body body;
        protected float bodyOffsetX;
        protected float bodyOffsetY;
        protected float bodyAngleOffset;

        protected float[] sin;
        protected float[] cos;

        protected float[] endX;
        protected float[] endY;

        public PositionalLight(RayHandler rayHandler, int rays, Color color, float distance, float x, float y, float directionDegree) : base(rayHandler, rays, color, distance, directionDegree)
        {
            start.X = x;
            start.Y = y;

            lightMesh = new VertexBuffer(Core.GraphicsDevice, typeof(CustomVertex), lightVertexNum + 10, BufferUsage.WriteOnly);
            softShadowMesh = new VertexBuffer(Core.GraphicsDevice, typeof(CustomVertex), softShadowVertexNum + 10, BufferUsage.WriteOnly);
            SetMesh();
        }

        public struct CustomVertex(Vector2 position, Color color, float fog) : IVertexType
        {
            public Vector2 Position = position;
            public Color Color = color;
            public float Fog = fog;
            VertexDeclaration IVertexType.VertexDeclaration
            {
                get => VertexDeclaration;
            }

            public static readonly VertexDeclaration VertexDeclaration = new(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(4, VertexElementFormat.Single, VertexElementUsage.Fog, 0)
            );
        }

        public override void Update()
        {
            UpdateBody();

            if (Cull()) return;
            if (staticLight && !dirty) return;

            dirty = false;
            UpdateMesh();
        }

        public override void Render()
        {
            if (rayHandler.culling && culled) return;

            rayHandler.lightRenderedLastFrame++;
            Core.GraphicsDevice.SetVertexBuffer(lightMesh);
            Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, lightVertexNum);

            if (soft && !xray && !rayHandler.pseudo3d)
            {
                Core.GraphicsDevice.SetVertexBuffer(softShadowMesh);
                Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, softShadowVertexNum);
            }
        }

        public override void AttachToBody(Body body)
        {
            AttachToBody(body, 0f, 0f, 0f);
        }

        public void AttachToBody(Body body, float offsetX, float offsetY)
        {
            AttachToBody(body, offsetX, offsetY, 0f);
        }

        public void AttachToBody(Body body, float offsetX, float offsetY, float degrees)
        {
            this.body = body;
            bodyOffsetX = offsetX;
            bodyOffsetY = offsetY;
            bodyAngleOffset = degrees;
            if (staticLight) dirty = true;
        }

        public override Vector2 GetPosition()
        {
            tmpPosition.X = start.X;
            tmpPosition.Y = start.Y;
            return tmpPosition;
        }

        public override Body GetBody()
        {
            return body;
        }

        public override float GetX()
        {
            return start.X;
        }

        public override float GetY()
        {
            return start.Y;
        }

        public override void SetPosition(float x, float y)
        {
            start.X = x;
            start.Y = y;
            if (staticLight) dirty = true;
        }

        public override void SetPosition(Vector2 position)
        {
            start.X = position.X;
            start.Y = position.Y;
            if (staticLight) dirty = true;
        }

        public bool Contains(Vector2 pos)
        {
            return Contains(pos.X, pos.Y);
        }

        public override bool Contains(float x, float y)
        {
            // fast fail
            float x_d = start.X - x;
            float y_d = start.Y - y;
            float dst2 = x_d * x_d + y_d * y_d;
            if (distance * distance <= dst2) return false;

            // actual check
            bool oddNodes = false;
            float x2 = mx[rayNum] = start.X;
            float y2 = my[rayNum] = start.Y;
            float x1, y1;
            for (int i = 0; i <= rayNum; x2 = x1, y2 = y1, ++i)
            {
                x1 = mx[i];
                y1 = my[i];
                if (((y1 < y) && (y2 >= y)) || (y1 >= y) && (y2 < y))
                {
                    if ((y - y1) / (y2 - y1) * (x2 - x1) < (x - x1)) oddNodes = !oddNodes;
                }
            }
            return oddNodes;
        }

        protected override void SetRayNum(int rays)
        {
            base.SetRayNum(rays);

            sin = new float[rays];
            cos = new float[rays];
            endX = new float[rays];
            endY = new float[rays];
        }

        protected bool Cull()
        {
            culled = rayHandler.culling && !rayHandler.intersect(
                        start.X, start.Y, distance + softShadowLength);
            return culled;
        }

        protected void UpdateBody()
        {
            if (body == null || staticLight) return;

            Vector2 vec = body.DisplayPosition;
            Transform tr = new Transform();
            body.GetTransform(out tr);
            float angle = tr.Q.GetAngle();
            float cos = tr.Q.C;
            float sin = tr.Q.S;
            float dX = bodyOffsetX * cos - bodyOffsetY * sin;
            float dY = bodyOffsetX * sin + bodyOffsetY * cos;
            start.X = vec.X + dX;
            start.Y = vec.Y + dY;
            SetDirection(bodyAngleOffset + (float) (angle * (180f / Math.PI))); // rads to degrees
        }

        protected void UpdateMesh()
        {
            for (int i = 0; i < rayNum; i++)
            {
                m_index = i;
                f[i] = 1f;
                tmpEnd.X = endX[i] + start.X;
                mx[i] = tmpEnd.X;
                tmpEnd.Y = endY[i] + start.Y;
                my[i] = tmpEnd.Y;
                if (rayHandler.world != null && !xray && !rayHandler.pseudo3d)
                {
                    rayHandler.world.RayCast(ray, start, tmpEnd);
                }
            }
            SetMesh();
        }


        protected void SetMesh()
        {
            // ray starting point
            int size = 0;
            int tempIndex = 0;

            CustomVertex[] vertices = new CustomVertex[lightVertexNum];
            Vector2 tmpVec = new Vector2();
            for (int i = 0; i < rayNum; i += 1)
            {
                vertices[size] = new CustomVertex(start, color, 1);
                size++;
                tmpVec.X = mx[tempIndex];
                tmpVec.Y = my[tempIndex];
                vertices[size] = new CustomVertex(tmpVec, color, 1 - f[tempIndex]);
                size++;
                tempIndex++;
                tmpVec.X = mx[tempIndex];
                tmpVec.Y = my[tempIndex];
                vertices[size] = new CustomVertex(tmpVec, color, 1 - f[tempIndex]);
                size++;
            }

            lightMesh.SetData(vertices, 0, size);

            if (!soft || xray || rayHandler.pseudo3d) return;

            vertices = new CustomVertex[softShadowVertexNum];
            size = 0;
            tempIndex = 0;
            for (int i = 0; i < rayNum; i++)
            {
                tmpVec.X = mx[i];
                tmpVec.Y = my[i];
                float s = 1 - f[i];
                vertices[size++] = new CustomVertex(tmpVec, color, s);
                tmpVec.X = mx[i] + s * softShadowLength * cos[i];
                tmpVec.Y = my[i] + s * softShadowLength * sin[i];
                vertices[size++] = new CustomVertex(tmpVec, Color.Transparent, 0);
            }
            softShadowMesh.SetData(vertices, 0, size);
        }

        public float GetBodyOffsetX()
        {
            return bodyOffsetX;
        }

        public float GetBodyOffsetY()
        {
            return bodyOffsetY;
        }

        public float GetBodyAngleOffset()
        {
            return bodyAngleOffset;
        }

        public void SetBodyOffsetX(float bodyOffsetX)
        {
            this.bodyOffsetX = bodyOffsetX;
        }

        public void SetBodyOffsetY(float bodyOffsetY)
        {
            this.bodyOffsetY = bodyOffsetY;
        }

        public void SetBodyAngleOffset(float bodyAngleOffset)
        {
            this.bodyAngleOffset = bodyAngleOffset;
        }
    }
}




