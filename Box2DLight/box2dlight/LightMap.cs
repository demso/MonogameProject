
using System;
using System.Collections.Generic;
using Box2DLight;
using Box2DLight.box2dlight.shaders;
using Box2DLight.shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace Box2DLight
{
    class LightMap
    {
        private GraphicsDevice graphicsDevice;
        private Effect shadowShader;
        internal RenderTarget2D frameBuffer;
        private RenderTarget2D pingPongBuffer;
        private VertexBuffer lightMapMesh;

        private RayHandler rayHandler;
        private Effect withoutShadowShader;
        private Effect blurShader;
        private Effect diffuseShader;

        public SpriteBatch spriteBatch;

        internal bool lightMapDrawingDisabled;

        private int fboWidth, fboHeight;

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
            
            frameBuffer = new RenderTarget2D(graphicsDevice, fboWidth, fboHeight, false, SurfaceFormat.ColorSRgb, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            pingPongBuffer = new RenderTarget2D(graphicsDevice, fboWidth, fboHeight, false, SurfaceFormat.Color, DepthFormat.None);

            spriteBatch = new SpriteBatch(Core.GraphicsDevice);

            lightMapMesh = CreateLightMapMesh();

            CreateShaders();
        }

        public void Render()
        {
            bool needed = rayHandler.lightRenderedLastFrame > 0;

            if (lightMapDrawingDisabled)
                return;
            Core.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            spriteBatch.Draw(frameBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            // at last lights are rendered over scene
            if (rayHandler.shadows)
            {
                Color c = rayHandler.ambientLight;
                Effect shader = shadowShader;
                if (RayHandler.isDiffuse)
                {
                    shader = diffuseShader;
                    rayHandler.diffuseBlendFunc.Apply();
                    shader.Parameters["Ambient"].SetValue(new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f));
                }
                else
                {
                    rayHandler.shadowBlendFunc.Apply();
                    shader.Parameters["Ambient"].SetValue(new Vector4((float)c.R * (float)c.A / 255f, (float)c.G * (float)c.A / 255f, (float)c.B * (float)c.A / 255f, 1f - (float)c.A / 255f));
                }

                shader.CurrentTechnique.Passes[0].Apply();
                graphicsDevice.SetVertexBuffer(lightMapMesh);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
            }
            else if (needed)
            {
                rayHandler.simpleBlendFunc.Apply();
                withoutShadowShader.CurrentTechnique.Passes[0].Apply();

                graphicsDevice.SetVertexBuffer(lightMapMesh);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
            }

            graphicsDevice.BlendState = (BlendState.Opaque);
        }

        public void GaussianBlur(RenderTarget2D buffer, int blurNum)
        {
            //graphicsDevice.BlendState = (BlendState.Opaque);
            //for (int i = 0; i < blurNum; i++)
            //{
            //    buffer.SetData(0, 0, fboWidth, fboHeight, 0, 0);
            //    // horizontal
            //    pingPongBuffer.SetData(0, 0, fboWidth, fboHeight, 0, 0);
            //    {
            //        blurShader.CurrentTechnique.Passes[0].Apply();
            //        blurShader.Parameters["dir"].SetValue(new Vector2(1f, 0f));
            //        graphicsDevice.SetVertexBuffer(lightMapMesh);
            //        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            //    }

            //    pingPongBuffer.SetData(0, 0, fboWidth, fboHeight, 0, 0);
            //    // vertical
            //    buffer.SetData(0, 0, fboWidth, fboHeight, 0, 0);
            //    {
            //        blurShader.CurrentTechnique.Passes[0].Apply();
            //        blurShader.Parameters["dir"].SetValue(new Vector2(0f, 1f));
            //        graphicsDevice.SetVertexBuffer(lightMapMesh);
            //        graphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            //    }
            //    if (rayHandler.customViewport)
            //    {
            //        buffer.SetData(rayHandler.viewportX, rayHandler.viewportY, rayHandler.viewportWidth, rayHandler.viewportHeight, 0, 0);
            //    }
            //    else
            //    {
            //        buffer.SetData(0, 0, fboWidth, fboHeight, 0, 0);
            //    }
            //}

            //graphicsDevice.SetBlendState(BlendState.AlphaBlend);
        }

        void Dispose()
        {
            DisposeShaders();

            lightMapMesh.Dispose();

            frameBuffer.Dispose();
            pingPongBuffer.Dispose();
        }

        internal void CreateShaders()
        {
            DisposeShaders();

            //shadowShader = rayHandler.pseudo3d ? DynamicShadowShader.CreateShadowShader() : ShadowShader.CreateShadowShader();
            shadowShader = ShadowShader.CreateShadowShader();

            diffuseShader = DiffuseShader.CreateShadowShader();

            withoutShadowShader = WithoutShadowShader.CreateShadowShader();

            //blurShader = Gaussian.CreateBlurShader(fboWidth, fboHeight);
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
            //vertices[0].Position = new Vector3(0, 0, 0);
            //vertices[1].Position = new Vector3(0, 1, 0);
            //vertices[2].Position = new Vector3(1, 0, 0);
            //vertices[3].Position = new Vector3(1, 1, 0);
            vertices[0].Position = new Vector3(-1, -1, 0);
            vertices[1].Position = new Vector3(-1, 1, 0);
            vertices[2].Position = new Vector3(1, -1, 0);
            vertices[3].Position = new Vector3(1, 1, 0);

            // tex coords
            vertices[0].TextureCoordinate = new Vector2(0f, 0f);
            vertices[1].TextureCoordinate = new Vector2(1f, 0f);
            vertices[2].TextureCoordinate = new Vector2(0f, 1f);
            vertices[3].TextureCoordinate = new Vector2(1f, 1f);

            VertexBuffer tmpMesh = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            tmpMesh.SetData(vertices);
            return tmpMesh;
        }
    }
}

