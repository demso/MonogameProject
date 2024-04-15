
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

        private const int BLUR_RADIUS = 7;
        private const float BLUR_AMOUNT = 2.0f;

        private RenderTarget2D _renderTarget1;
        private RenderTarget2D _renderTarget2;

        GaussianBlur _gaussianBlur;

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
            lightMapMesh2 = CreateLightMapMesh2();
            CreateShaders();

            _gaussianBlur = new GaussianBlur(Core.Instance);
            _gaussianBlur.ComputeKernel(BLUR_RADIUS, BLUR_AMOUNT);

            _renderTarget1 = new RenderTarget2D(Core.GraphicsDevice, fboWidth, fboHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);
            _renderTarget2 = new RenderTarget2D(Core.GraphicsDevice, fboWidth, fboHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);

            _gaussianBlur.ComputeOffsets(fboWidth, fboHeight);
        }

        public void Render()
        {

            //Core.GraphicsDevice.SetRenderTarget(null);
            //Core.GraphicsDevice.Clear(Color.Transparent);

            //spriteBatch.Begin(blendState: BlendState.Opaque);
            //spriteBatch.Draw(frameBuffer, Core.GraphicsDevice.Viewport.Bounds, Color.White);
            //spriteBatch.End();

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


                    shader = diffuseShader;

                    blFn = rayHandler.diffuseBlendFunc;
                    shader.Parameters["Ambient"].SetValue(c.ToVector4());
                    shader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                    shader.CurrentTechnique.Passes[0].Apply();

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

                    //spriteBatch.Begin();
                    //spriteBatch.Draw(frameBuffer, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                    //spriteBatch.End();
                }
                else
                {

                    blFn = rayHandler.shadowBlendFunc;
                    shader.Parameters["Ambient"].SetValue(new Vector4((float)c.R * (float)c.A / 255f,
                        (float)c.G * (float)c.A / 255f, (float)c.B * (float)c.A / 255f, 1f - (float)c.A / 255f));
                    shader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                    shader.CurrentTechnique.Passes[0].Apply();
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

        private Texture2D result;

        GaussianBlurEffect blurEffect;

        public void GaussianBlur()
        {
            Core.GraphicsDevice.SetRenderTarget(pingPongBuffer);
            Core.GraphicsDevice.Clear(Color.Transparent);;
            
            float FBO_W = fboWidth;
            float FBO_H = fboHeight;
            Vector2 futher = new Vector2(3.2307692308f / FBO_W, 3.2307692308f / FBO_H);
            Vector2 closer = new Vector2(1.3846153846f / FBO_W, 1.3846153846f / FBO_H);
            Vector2 f = futher * Vector2.UnitX;
            Vector2 c = closer * Vector2.UnitX;
            Vector2[] _sampleHorOffsets = new Vector2[5]{-f, -c, Vector2.Zero, c, f};
            f = futher * Vector2.UnitY;
            c = closer * Vector2.UnitY;
            Vector2[] _sampleVertOffsets = new Vector2[5]{-f, -c, Vector2.Zero, c, f};


            for (int i = 0; i < rayHandler.blurNum; i++)
            {
                Core.GraphicsDevice.SetRenderTarget(pingPongBuffer);
                //Core.GraphicsDevice.Clear(Color.Red);
                graphicsDevice.BlendState = (BlendState.AlphaBlend);

                //horizontal
                {
                    blurShader.Parameters["_sampleOffsets"].SetValue(_sampleVertOffsets);
                    blurShader.Parameters["RenderTargetTexture"].SetValue(frameBuffer);
                    //blurShader.Parameters["FBO_W"].SetValue(frameBuffer.Width);
                    //blurShader.Parameters["FBO_H"].SetValue(frameBuffer.Height);
                    //blurShader.Parameters["isDiffuse"].SetValue(RayHandler.isDiffuse);
                    //blurShader.CurrentTechnique.Passes[0].Apply();

                    //graphicsDevice.Textures[0] = frameBuffer;

                    //graphicsDevice.SetVertexBuffer(lightMapMesh);
                    //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);

                    spriteBatch.Begin(effect: blurShader);
                    spriteBatch.Draw(frameBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                    spriteBatch.End();
                }

                blurShader.Parameters["_sampleOffsets"].SetValue(_sampleHorOffsets);
               // blurShader.Parameters["RenderTargetTexture"].SetValue();

                Core.GraphicsDevice.SetRenderTarget(frameBuffer);

                spriteBatch.Begin(effect: blurShader, blendState: BlendState.Opaque);
                spriteBatch.Draw(pingPongBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                spriteBatch.End();

                //Core.GraphicsDevice.SetRenderTarget(null);
                //Core.GraphicsDevice.Clear(Color.Transparent);

                //spriteBatch.Begin();
                //spriteBatch.Draw(pingPongBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                //spriteBatch.End();


                //Core.GraphicsDevice.SetRenderTarget(null);
                //Core.GraphicsDevice.Clear(Color.Transparent);

                //spriteBatch.Begin(blendState: BlendState.AlphaBlend);
                //spriteBatch.Draw(pingPongBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                //spriteBatch.End();

                //Core.GraphicsDevice.SetRenderTarget(null);
                //Core.GraphicsDevice.Clear(Color.Transparent);

                //spriteBatch.Begin();
                //spriteBatch.Draw(frameBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                //spriteBatch.End();

                //Core.GraphicsDevice.SetRenderTarget(frameBuffer);
                //Core.GraphicsDevice.Clear(Color.Transparent);

                ////BasicEffect be = new BasicEffect(graphicsDevice);
                ////be.TextureEnabled = true;
                ////be.Texture = pingPongBuffer;
                ////be.CurrentTechnique.Passes[0].Apply();


                //graphicsDevice.Textures[0] = pingPongBuffer;

                //graphicsDevice.SetVertexBuffer(lightMapMesh);
                //graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);

                //spriteBatch.Begin(blendState: BlendState.Opaque, samplerState: new SamplerState() { Filter = TextureFilter.Point });
                //spriteBatch.Draw(pingPongBuffer, Core.GraphicsDevice.Viewport.Bounds, Color.White);
                //spriteBatch.End();
                //Graphics.Instance.Batcher.Begin(BlendState.Opaque);
                //Graphics.Instance.Batcher.Draw(pingPongBuffer, graphicsDevice.Viewport.Bounds, Color.White);
                //Graphics.Instance.Batcher.End();

                //Core.GraphicsDevice.SetRenderTarget(frameBuffer);
                //Core.GraphicsDevice.Clear(Color.Transparent);
                //graphicsDevice.BlendState = (BlendState.Opaque);
                //// vertical
                //{
                //    blurShader.Parameters["dir"].SetValue(new Vector2(0f, 1f));
                //    blurShader.Parameters["RenderTargetTexture"].SetValue(pingPongBuffer);
                //    blurShader.Parameters["FBO_W"].SetValue(pingPongBuffer.Width);
                //    blurShader.Parameters["FBO_H"].SetValue(pingPongBuffer.Height);
                //    blurShader.Parameters["isDiffuse"].SetValue(RayHandler.isDiffuse);
                //    blurShader.CurrentTechnique.Passes[0].Apply();
                //    graphicsDevice.SetVertexBuffer(lightMapMesh);
                //    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, lightMapMesh.VertexCount);
                //}
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
            testEf = Core.Content.Load<Effect>(@"Texting");

            blurEffect = Core.Content.LoadNezEffect<GaussianBlurEffect>();
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

