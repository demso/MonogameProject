
using System;
using System.Collections.Generic;
using Box2DLight;
using Box2DLight.box2dlight.shaders;
using Box2DLight.shaders;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Box2DLight
{
    public class LightMap
    {
        private GraphicsDevice graphicsDevice;
        private Effect shadowShader;
        public RenderTarget2D frameBuffer;
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
            
            frameBuffer = new RenderTarget2D(Core.GraphicsDevice, fboWidth, fboHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            pingPongBuffer = new RenderTarget2D(graphicsDevice, fboWidth, fboHeight, false, SurfaceFormat.ColorSRgb, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);

            spriteBatch = new SpriteBatch(Core.GraphicsDevice);

            lightMapMesh = CreateLightMapMesh();

            CreateShaders();
        }

        public void Render()
        {
            bool needed = rayHandler.lightRenderedLastFrame > 0;

            Color c = rayHandler.ambientLight;
            Effect shader = shadowShader;
            BlendFunc blFn = rayHandler.shadowBlendFunc;
            if (rayHandler.shadows)
            {
                if (RayHandler.isDiffuse)
                {
                    //Core.GraphicsDevice.BlendState = BlendState.Opaque;
                    //Core.GraphicsDevice.Clear(Color.Transparent);

                    //graphicsDevice.SetVertexBuffer(lightMapMesh);
                    //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);

                    Core.GraphicsDevice.SetRenderTarget(null);
                    Core.GraphicsDevice.Clear(Color.Transparent);

                    spriteBatch.Begin(blendState: BlendState.Opaque);
                    spriteBatch.Draw(rayHandler.renTar, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.End();

                    //Graphics.Instance.Batcher.Begin(BlendState.Opaque);
                    //Graphics.Instance.Batcher.Draw(rayHandler.renTar, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                    //Graphics.Instance.Batcher.End();

                    Rectangle rec = graphicsDevice.Viewport.Bounds.Clone();

                    shader = diffuseShader;
                    shader.CurrentTechnique.Passes[0].Apply();
                    blFn = rayHandler.diffuseBlendFunc;
                    shader.Parameters["Ambient"].SetValue(c.ToVector4());
                    shader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);

                    blFn.Apply();
                    graphicsDevice.SetVertexBuffer(lightMapMesh);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);

                    //Graphics.Instance.Batcher.Begin(blFn.Get());
                    //Graphics.Instance.Batcher.Draw(frameBuffer, rec, Color.White);
                    //Graphics.Instance.Batcher.End();

                    //spriteBatch.Begin(blendState: BlendState.Opaque);
                    //spriteBatch.Draw(rayHandler.renTar, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                    //spriteBatch.End();

                    //Rectangle rec = graphicsDevice.Viewport.Bounds.Clone();

                    //spriteBatch.Begin(blendState: blFn.Get());
                    //spriteBatch.Draw(frameBuffer, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                    //spriteBatch.End();
                }
                else
                {
                    shader.CurrentTechnique.Passes[0].Apply();
                    blFn = rayHandler.shadowBlendFunc;
                    shader.Parameters["Ambient"].SetValue(new Vector4((float)c.R * (float)c.A / 255f,
                        (float)c.G * (float)c.A / 255f, (float)c.B * (float)c.A / 255f, 1f - (float)c.A / 255f));

                    blFn.Apply();

                    graphicsDevice.SetVertexBuffer(lightMapMesh);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);

                    Core.GraphicsDevice.SetRenderTarget(null);

                    spriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp);
                    spriteBatch.Draw(rayHandler.renTar, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.End();

                    spriteBatch.Begin(blendState: blFn.Get(), samplerState: SamplerState.AnisotropicClamp);
                    spriteBatch.Draw(frameBuffer, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.End();
                }
            }
            else if (needed)
            {
                graphicsDevice.SetVertexBuffer(lightMapMesh);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
            }

            graphicsDevice.BlendState = (BlendState.Opaque);
        }

        public void GaussianBlur()
        {
            graphicsDevice.BlendState = (BlendState.AlphaBlend);
            rayHandler.simpleBlendFunc.Apply();
            Core.GraphicsDevice.SetRenderTarget(pingPongBuffer);
            Core.GraphicsDevice.Clear(Color.Transparent);
            //horizontal
            for (int i = 0; i < rayHandler.blurNum; i++)
            {
                Core.GraphicsDevice.SetRenderTarget(pingPongBuffer);
                //horizontal
                {
                    blurShader.CurrentTechnique.Passes[0].Apply();
                    blurShader.Parameters["dir"].SetValue(new Vector2(1f, 0f));
                    blurShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                    blurShader.Parameters["FBO_W"].SetValue(frameBuffer.Width);
                    blurShader.Parameters["FBO_H"].SetValue(frameBuffer.Height);
                    blurShader.Parameters["isDiffuse"].SetValue(RayHandler.isDiffuse);
                    graphicsDevice.SetVertexBuffer(lightMapMesh);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
                }

                Core.GraphicsDevice.SetRenderTarget(frameBuffer);
                spriteBatch.Begin();
                spriteBatch.Draw(pingPongBuffer, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                spriteBatch.End();

                // vertical
                {
                    blurShader.CurrentTechnique.Passes[0].Apply();
                    blurShader.Parameters["dir"].SetValue(new Vector2(0f, 1f));
                    blurShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                    blurShader.Parameters["FBO_W"].SetValue(frameBuffer.Width);
                    blurShader.Parameters["FBO_H"].SetValue(frameBuffer.Height);
                    blurShader.Parameters["isDiffuse"].SetValue(RayHandler.isDiffuse);
                    graphicsDevice.SetVertexBuffer(lightMapMesh);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
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

            //shadowShader = rayHandler.pseudo3d ? DynamicShadowShader.CreateShadowShader() : ShadowShader.CreateShadowShader();
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
    }
}

