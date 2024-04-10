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

            lightMesh = new VertexBuffer(Core.GraphicsDevice, typeof(CustomVertex), vertexNum, BufferUsage.WriteOnly);
            softShadowMesh = new VertexBuffer(Core.GraphicsDevice, typeof(CustomVertex), rayNum * 2, BufferUsage.WriteOnly);
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
            //lightMesh.Render(rayHandler.lightShader, GraphicsDevice.GL_TRIANGLE_FAN, 0, vertexNum);

            if (soft && !xray && !rayHandler.pseudo3d)
            {
                Core.GraphicsDevice.SetVertexBuffer(softShadowMesh);
                Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, softShadowVertexNum);
                //softShadowMesh.Render(
                //    rayHandler.lightShader,
                //    GraphicsDevice.GL_TRIANGLE_STRIP,
                //    0,
                //    (vertexNum - 1) * 2);
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

        //protected void PrepareFixtureData()
        //{
        //    rayHandler.world.QueryAABB(
        //            dynamicShadowCallback,
        //            start.X - distance, start.Y - distance,
        //            start.X + distance, start.Y + distance);
        //}

        protected void SetMesh()
        {
            // ray starting point
            int size = 0;
            int tempIndex = 0;

            CustomVertex[] vertices = new CustomVertex[size * 8 * 3 + 1];
            //vertices[0] = new CustomVertex(start, color, 1);
            //three points for every triangle
            Vector2 tmpVec = new Vector2();
            for (int i = 0; i < rayNum * 3; i += 3)
            {
                vertices[i] = new CustomVertex(start, color, 1);
                tmpVec.X = mx[tempIndex];
                tmpVec.Y = my[tempIndex];
                vertices[i + 1] = new CustomVertex(tmpVec, color, 1 - f[tempIndex]);
                tempIndex++;
                tmpVec.X = mx[tempIndex];
                tmpVec.Y = my[tempIndex];
                vertices[i + 2] = new CustomVertex(tmpVec, color, 1 - f[tempIndex]);
                tempIndex++;
                size += 3;
            }

            //segments[size++] = start.X;
            //segments[size++] = start.Y;
            //segments[size++] = colorF;
            //segments[size++] = 1;
            //// rays ending points.
            //for (int i = 0; i < rayNum; i++)
            //{
            //    segments[size++] = mx[i];
            //    segments[size++] = my[i];
            //    segments[size++] = colorF;
            //    segments[size++] = 1 - f[i];
            //}
            lightMesh.SetData(vertices, 0, size);

            if (!soft || xray || rayHandler.pseudo3d) return;

            size = 0;
            // rays ending points.
            //for (int i = 0; i < rayNum; i++)
            //{
            //    segments[size++] = mx[i];
            //    segments[size++] = my[i];
            //    segments[size++] = colorF;
            //    float s = (1 - f[i]);
            //    segments[size++] = s;
            //    segments[size++] = mx[i] + s * softShadowLength * cos[i];
            //    segments[size++] = my[i] + s * softShadowLength * sin[i];
            //    segments[size++] = ZeroColorBits;
            //    segments[size++] = 0f;
            //}
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

        //protected void UpdateDynamicShadowMeshes()
        //{
        //    int meshInd = 0;
        //    float colBits = rayHandler.ambientLight.ToFloatBits();
        //    foreach (Fixture fixture in affectedFixtures)
        //    {
        //        LightData data = (LightData)fixture.UserData;
        //        if (data == null || fixture.IsSensor) continue;

        //        int size = 0;
        //        float l;

        //        Shape fixtureShape = fixture.Shape;
        //        Shape.Type type = fixtureShape.Type;
        //        Body body = fixture.Body;
        //        center.Set(body.WorldCenter);

        //        if (type == Shape.Type.Polygon || type == Shape.Type.Chain)
        //        {
        //            bool isPolygon = (type == Shape.Type.Polygon);
        //            ChainShape cShape = isPolygon ?
        //                    null : (ChainShape)fixtureShape;
        //            PolygonShape pShape = isPolygon ?
        //                    (PolygonShape)fixtureShape : null;
        //            int vertexCount = isPolygon ?
        //                    pShape.VertexCount : cShape.VertexCount;
        //            int minN = -1;
        //            int maxN = -1;
        //            int minDstN = -1;
        //            float minDst = float.PositiveInfinity;
        //            bool hasGasp = false;
        //            tmpVerts.Clear();
        //            for (int n = 0; n < vertexCount; n++)
        //            {
        //                if (isPolygon)
        //                {
        //                    pShape.GetVertex(n, tmpVec);
        //                }
        //                else
        //                {
        //                    cShape.GetVertex(n, tmpVec);
        //                }
        //                tmpVec.Set(body.GetWorldPoint(tmpVec));
        //                tmpVerts.Add(tmpVec.Cpy());
        //                tmpEnd.Set(tmpVec).Sub(start).Limit2(0.0001f).Add(tmpVec);
        //                if (fixture.TestPoint(tmpEnd))
        //                {
        //                    if (minN == -1) minN = n;
        //                    maxN = n;
        //                    hasGasp = true;
        //                    continue;
        //                }

        //                float currDist = tmpVec.Dst2(start);
        //                if (currDist < minDst)
        //                {
        //                    minDst = currDist;
        //                    minDstN = n;
        //                }
        //            }

        //            ind.Clear();
        //            if (!hasGasp)
        //            {
        //                tmpVec.Set(tmpVerts.Get(minDstN));
        //                for (int n = minDstN; n < vertexCount; n++)
        //                {
        //                    ind.Add(n);
        //                }
        //                for (int n = 0; n < minDstN; n++)
        //                {
        //                    ind.Add(n);
        //                }
        //                if (Intersector.PointLineSide(start, center, tmpVec) > 0)
        //                {
        //                    ind.Reverse();
        //                    ind.Insert(0, ind.Pop());
        //                }
        //            }
        //            else if (minN == 0 && maxN == vertexCount - 1)
        //            {
        //                for (int n = maxN - 1; n > minN; n--)
        //                {
        //                    ind.Add(n);
        //                }
        //            }
        //            else
        //            {
        //                for (int n = minN - 1; n > -1; n--)
        //                {
        //                    ind.Add(n);
        //                }
        //                for (int n = vertexCount - 1; n > maxN; n--)
        //                {
        //                    ind.Add(n);
        //                }
        //            }

        //            bool contained = false;
        //            foreach (int n in ind.ToArray())
        //            {
        //                tmpVec.Set(tmpVerts.Get(n));
        //                if (Contains(tmpVec.X, tmpVec.Y))
        //                {
        //                    contained = true;
        //                    break;
        //                }
        //            }

        //            if (!contained)
        //                continue;

        //            foreach (int n in ind.ToArray())
        //            {
        //                tmpVec.Set(tmpVerts.Get(n));

        //                float dst = tmpVec.Dst(start);
        //                l = data.GetLimit(dst, pseudo3dHeight, distance);
        //                tmpEnd.Set(tmpVec).Sub(start).SetLength(l).Add(tmpVec);

        //                float f1 = 1f - dst / distance;
        //                float f2 = 1f - (dst + l) / distance;

        //                tmpColor.Set(Color.Black);
        //                float startColBits = rayHandler.shadowColorInterpolation ?
        //                        tmpColor.Lerp(rayHandler.ambientLight, f1).ToFloatBits() :
        //                        oneColorBits;
        //                tmpColor.Set(Color.White);
        //                float endColBits = rayHandler.shadowColorInterpolation ?
        //                        tmpColor.Lerp(rayHandler.ambientLight, f2).ToFloatBits() :
        //                        colBits;

        //                segments[size++] = tmpVec.X;
        //                segments[size++] = tmpVec.Y;
        //                segments[size++] = startColBits;
        //                segments[size++] = f1;

        //                segments[size++] = tmpEnd.X;
        //                segments[size++] = tmpEnd.Y;
        //                segments[size++] = endColBits;
        //                segments[size++] = f2;
        //            }
        //        }
        //        else if (type == Shape.Type.Circle)
        //        {
        //            CircleShape shape = (CircleShape)fixtureShape;
        //            float r = shape.Radius;
        //            if (!Contains(tmpVec.Set(center).Add(r, r)) && !Contains(tmpVec.Set(center).Add(-r, -r))
        //                    && !Contains(tmpVec.Set(center).Add(r, -r)) && !Contains(tmpVec.Set(center).Add(-r, r)))
        //            {
        //                continue;
        //            }

        //            float dst = tmpVec.Set(center).Dst(start);
        //            float a = (float)Math.Acos(r / dst);
        //            l = data.GetLimit(dst, pseudo3dHeight, distance);
        //            float f1 = 1f - dst / distance;
        //            float f2 = 1f - (dst + l) / distance;
        //            tmpColor.Set(Color.Black);
        //            float startColBits = rayHandler.shadowColorInterpolation ?
        //                    tmpColor.Lerp(rayHandler.ambientLight, f1).ToFloatBits() :
        //                    oneColorBits;
        //            tmpColor.Set(Color.White);
        //            float endColBits = rayHandler.shadowColorInterpolation ?
        //                    tmpColor.Lerp(rayHandler.ambientLight, f2).ToFloatBits() :
        //                    colBits;

        //            tmpVec.Set(start).Sub(center).Clamp(r, r).RotateRad(a);
        //            tmpStart.Set(center).Add(tmpVec);

        //            float angle = (MathUtils.PI2 - 2f * a) /
        //                    RayHandler.CIRCLE_APPROX_POINTS;
        //            for (int k = 0; k < RayHandler.CIRCLE_APPROX_POINTS; k++)
        //            {
        //                tmpStart.Set(center).Add(tmpVec);
        //                segments[size++] = tmpStart.X;
        //                segments[size++] = tmpStart.Y;
        //                segments[size++] = startColBits;
        //                segments[size++] = f1;

        //                tmpEnd.Set(tmpStart).Sub(start).SetLength(l).Add(tmpStart);
        //                segments[size++] = tmpEnd.X;
        //                segments[size++] = tmpEnd.Y;
        //                segments[size++] = endColBits;
        //                segments[size++] = f2;

        //                tmpVec.RotateRad(angle);
        //            }
        //        }
        //        else if (type == Shape.Type.Edge)
        //        {
        //            EdgeShape shape = (EdgeShape)fixtureShape;

        //            shape.GetVertex1(tmpVec);
        //            tmpVec.Set(body.GetWorldPoint(tmpVec));
        //            if (!Contains(tmpVec))
        //            {
        //                continue;
        //            }
        //            float dst = tmpVec.Dst(start);
        //            l = data.GetLimit(dst, pseudo3dHeight, distance);
        //            float f1 = 1f - dst / distance;
        //            float f2 = 1f - (dst + l) / distance;
        //            tmpColor.Set(Color.Black);
        //            float startColBits = rayHandler.shadowColorInterpolation ?
        //                    tmpColor.Lerp(rayHandler.ambientLight, f1).ToFloatBits() :
        //                    oneColorBits;
        //            tmpColor.Set(Color.White);
        //            float endColBits = rayHandler.shadowColorInterpolation ?
        //                    tmpColor.Lerp(rayHandler.ambientLight, f2).ToFloatBits() :
        //                    colBits;

        //            segments[size++] = tmpVec.X;
        //            segments[size++] = tmpVec.Y;
        //            segments[size++] = startColBits;
        //            segments[size++] = f1;

        //            tmpEnd.Set(tmpVec).Sub(start).SetLength(l).Add(tmpVec);
        //            segments[size++] = tmpEnd.X;
        //            segments[size++] = tmpEnd.Y;
        //            segments[size++] = endColBits;
        //            segments[size++] = f2;

        //            shape.GetVertex2(tmpVec);
        //            tmpVec.Set(body.GetWorldPoint(tmpVec));
        //            if (!Contains(tmpVec))
        //            {
        //                continue;
        //            }
        //            dst = tmpVec.Dst(start);
        //            l = data.GetLimit(dst, pseudo3dHeight, distance);
        //            f1 = 1f - dst / distance;
        //            f2 = 1f - (dst + l) / distance;
        //            tmpColor.Set(Color.Black);
        //            startColBits = rayHandler.shadowColorInterpolation ?
        //                    tmpColor.Lerp(rayHandler.ambientLight, f1).ToFloatBits() :
        //                    oneColorBits;
        //            tmpColor.Set(Color.White);
        //            endColBits = rayHandler.shadowColorInterpolation ?
        //                    tmpColor.Lerp(rayHandler.ambientLight, f2).ToFloatBits() :
        //                    colBits;

        //            segments[size++] = tmpVec.X;
        //            segments[size++] = tmpVec.Y;
        //            segments[size++] = startColBits;
        //            segments[size++] = f1;

        //            tmpEnd.Set(tmpVec).Sub(start).SetLength(l).Add(tmpVec);
        //            segments[size++] = tmpEnd.X;
        //            segments[size++] = tmpEnd.Y;
        //            segments[size++] = endColBits;
        //            segments[size++] = f2;
        //        }

        //        Mesh mesh = null;
        //        if (meshInd >= dynamicShadowMeshes.Count)
        //        {
        //            mesh = new Mesh(
        //                    Mesh.VertexDataType.VertexArray, false, RayHandler.MAX_SHADOW_VERTICES, 0,
        //                    new VertexAttribute(VertexAttributes.Usage.Position, 2, "vertex_positions"),
        //                    new VertexAttribute(VertexAttributes.Usage.ColorPacked, 4, "quad_colors"),
        //                    new VertexAttribute(VertexAttributes.Usage.Generic, 1, "s"));
        //            dynamicShadowMeshes.Add(mesh);
        //        }
        //        else
        //        {
        //            mesh = dynamicShadowMeshes[meshInd];
        //        }
        //        mesh.SetVertices(segments, 0, size);
        //        meshInd++;
        //    }
        //    dynamicShadowMeshes.Truncate(meshInd);
        //}

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




