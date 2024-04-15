
using System;
using System.Collections.Generic;
using System.Drawing;
using Box2DLight;
using Box2DLight.box2dlight.shaders;
using Box2DLight.shaders;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Color = Microsoft.Xna.Framework.Color;

namespace Box2DLight
{
    public class LightMap
    {
        private GraphicsDevice graphicsDevice;
        private Effect shadowShader;
        public RenderTarget2D frameBuffer;
        private RenderTarget2D pingPongBuffer;
        private VertexBuffer lightMapMesh;
        private VertexBuffer lightMapMesh2;


        private RayHandler rayHandler;
        private Effect withoutShadowShader;
        private Effect blurShader;
        private Effect diffuseShader;
        private Effect testEf;

        public SpriteBatch spriteBatch;

        internal bool lightMapDrawingDisabled;

        private int fboWidth, fboHeight;

        private Vector2[] _sampleHorOffsets;
        private Vector2[] _sampleVertOffsets;

        public LightMap(RayHandler rayHandler, int fboWidth, int fboHeight)
        {
            this.rayHandler = rayHandler;
            graphicsDevice = Core.GraphicsDevice;

            if (fboWidth <= 0)
                fboWidth = 1;
            if (fboHeight <= 0)
                fboHeight = 1;

            this.fboWidth = fboWidth;
            this.fboHeight = fboHeight;
            
            frameBuffer = new RenderTarget2D(Core.GraphicsDevice, fboWidth, fboHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            pingPongBuffer = new RenderTarget2D(graphicsDevice, fboWidth, fboHeight, false, SurfaceFormat.ColorSRgb, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);

            spriteBatch = new SpriteBatch(Core.GraphicsDevice);

            lightMapMesh = CreateLightMapMesh();

            CreateShaders();

            Vector2 futher = new Vector2(3.2307692308f / fboWidth, 3.2307692308f / fboHeight);
            Vector2 closer = new Vector2(1.3846153846f / fboWidth, 1.3846153846f / fboHeight);
            Vector2 f = futher * Vector2.UnitX;
            Vector2 c = closer * Vector2.UnitX;
            _sampleHorOffsets = new Vector2[5] { -f, -c, Vector2.Zero, c, f };
            f = futher * Vector2.UnitY;
            c = closer * Vector2.UnitY;
            _sampleVertOffsets = new Vector2[5] { -f, -c, Vector2.Zero, c, f };
        }

        public void Render()
        {
            bool needed = rayHandler.lightRenderedLastFrame > 0;

            Color c = rayHandler.ambientLight;
            Effect shader = shadowShader;
            BlendFunc blFn = rayHandler.shadowBlendFunc;

            Core.GraphicsDevice.SetRenderTarget(rayHandler.RenderHere);

            if (rayHandler.shadows)
            {
                if (RayHandler.isDiffuse)
                {
                    diffuseShader.Parameters["Ambient"].SetValue(c.ToVector4());
                    diffuseShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                    diffuseShader.CurrentTechnique.Passes[0].Apply();

                    rayHandler.diffuseBlendFunc.Apply();

                    graphicsDevice.SetVertexBuffer(lightMapMesh);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
                }
                else
                {
                    shadowShader.Parameters["Ambient"].SetValue(new Vector4((float)c.R * (float)c.A / 255f,
                        (float)c.G * (float)c.A / 255f, (float)c.B * (float)c.A / 255f, 1f - (float)c.A / 255f));
                    shadowShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                    shadowShader.CurrentTechnique.Passes[0].Apply();

                    rayHandler.shadowBlendFunc.Apply();

                    graphicsDevice.SetVertexBuffer(lightMapMesh);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
                }
            }
            else if (needed)
            {
                withoutShadowShader.CurrentTechnique.Passes[0].Apply();
                withoutShadowShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);

                graphicsDevice.SetVertexBuffer(lightMapMesh);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
            }

            graphicsDevice.BlendState = (BlendState.Opaque);
        }

        public void GaussianBlur()
        {
            Core.GraphicsDevice.SetRenderTarget(pingPongBuffer);
            Core.GraphicsDevice.Clear(Color.Transparent);
            
            blurShader.Parameters["isDiffuse"].SetValue(RayHandler.isDiffuse);

            for (int i = 0; i < rayHandler.blurNum; i++)
            {
                Core.GraphicsDevice.SetRenderTarget(pingPongBuffer);

                //vertical
                {
                    blurShader.Parameters["_sampleOffsets"].SetValue(_sampleVertOffsets);

                    spriteBatch.Begin(effect: blurShader);
                    spriteBatch.Draw(frameBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.End();
                }

                Core.GraphicsDevice.SetRenderTarget(frameBuffer);

                //horizontal
                {
                    blurShader.Parameters["_sampleOffsets"].SetValue(_sampleHorOffsets);

                    spriteBatch.Begin(effect: blurShader, blendState: BlendState.Opaque);
                    spriteBatch.Draw(pingPongBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.End();
                }
            }
        }

        public void Dispose()
        {
            DisposeShaders();

            lightMapMesh.Dispose();

            frameBuffer.Dispose();
            pingPongBuffer.Dispose();
        }

        internal void CreateShaders()
        {
            DisposeShaders();

            shadowShader = ShadowShader.CreateShadowShader();

            diffuseShader = DiffuseShader.CreateShadowShader();

            withoutShadowShader = WithoutShadowShader.CreateShadowShader();

            blurShader = Gaussian.CreateBlurShader();
        }

        private void DisposeShaders()
        {
            shadowShader?.Dispose();
            diffuseShader?.Dispose();
            withoutShadowShader?.Dispose();
            blurShader?.Dispose();
        }

        private VertexBuffer CreateLightMapMesh()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];
            // vertex coord
            vertices[0].Position = new Vector3(-1, -1, 0);
            vertices[1].Position = new Vector3(-1, 1, 0);
            vertices[2].Position = new Vector3(1, -1, 0);
            vertices[3].Position = new Vector3(1, 1, 0);

            // tex coords
            vertices[0].TextureCoordinate = new Vector2(0f, 1f);
            vertices[1].TextureCoordinate = new Vector2(0f, 0f);
            vertices[2].TextureCoordinate = new Vector2(1f, 1f);
            vertices[3].TextureCoordinate = new Vector2(1f, 0f);

            VertexBuffer tmpMesh = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            tmpMesh.SetData(vertices);
            return tmpMesh;
        }

        private VertexBuffer CreateLightMapMesh2()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];
            // vertex coord
            vertices[0].Position = new Vector3(-1f, -1f, 0);
            vertices[1].Position = new Vector3(-1f, 1f, 0);
            vertices[2].Position = new Vector3(1f, -1f, 0);
            vertices[3].Position = new Vector3(1f, 1f, 0);

            // tex coords
            vertices[0].TextureCoordinate = new Vector2(0f, 1f);
            vertices[1].TextureCoordinate = new Vector2(0f, 0f);
            vertices[2].TextureCoordinate = new Vector2(1f, 1f);
            vertices[3].TextureCoordinate = new Vector2(1f, 0f);

            VertexBuffer tmpMesh = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            tmpMesh.SetData(vertices);
            return tmpMesh;
        }
    }
}

