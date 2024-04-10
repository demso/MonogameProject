using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Nez;
using Box2DLight.box2dlight.shaders;

namespace Box2DLight
{
    public class RayHandler : IDisposable
    {
        static float GAMMA_COR = 0.625f;

        static bool gammaCorrection = false;
        internal static float gammaCorrectionParameter = 1f;

        static int CIRCLE_APPROX_POINTS = 32;

        static float dynamicShadowColorReduction = 1;

        static int MAX_SHADOW_VERTICES = 64;

        internal static bool isDiffuse = false;

        public BlendFunc diffuseBlendFunc = new BlendFunc(Blend.One, Blend.Zero);

        public BlendFunc shadowBlendFunc = new BlendFunc(Blend.One, Blend.InverseDestinationAlpha);

        public BlendFunc simpleBlendFunc = new BlendFunc(Blend.SourceAlpha, Blend.One);

        Matrix combined = new Matrix();
        internal Color ambientLight = new Color();

        internal List<Light> lightList = new List<Light>();
        internal List<Light> disabledLights = new List<Light>();

        internal LightMap lightMap;
        internal Effect lightShader;
        internal Effect customLightShader = null;

        internal bool culling = true;
        internal bool shadows = true;
        bool blur = true;

        internal bool pseudo3d = false;
        bool shadowColorInterpolation = false;

        int blurNum = 1;

        bool customViewport = false;
        int viewportX = 0;
        int viewportY = 0;
        int viewportWidth = GraphicsDeviceManager.DefaultBackBufferWidth;
        int viewportHeight = GraphicsDeviceManager.DefaultBackBufferHeight;

        internal int lightRenderedLastFrame = 0;

        float x1, x2, y1, y2;

        internal World world;

        public RayHandler(World world) : this(world, null)
        {
            
        }

        public RayHandler(World world, RayHandlerOptions options) : this(world, Core.GraphicsDevice.DisplayMode.Width / 4, Core.GraphicsDevice.DisplayMode.Width / 4, options)
        {
            
        }

        public RayHandler(World world, int fboWidth, int fboHeight) : this(world, fboWidth, fboHeight, null)
        {
            this.world = world;
            lightShader = LightShader.createLightShader();
        }

        public RayHandler(World world, int fboWidth, int fboHeight, RayHandlerOptions options)
        {
            this.world = world;

            if (options != null)
            {
                isDiffuse = options.isDiffuse;
                gammaCorrection = options.gammaCorrection;
                pseudo3d = options.pseudo3d;
                shadowColorInterpolation = options.shadowColorInterpolation;
            }

            resizeFBO(fboWidth, fboHeight);
            lightShader = LightShader.createLightShader();
        }

        public void resizeFBO(int fboWidth, int fboHeight)
        {
            //if (lightMap != null)
            //{
            //    lightMap.dispose();
            //}
            lightMap = new LightMap(this, fboWidth, fboHeight);
        }

        //public void setCombinedMatrix(OrthographicCamera camera)
        //{
        //    this.setCombinedMatrix(
        //            camera.combined,
        //            camera.position.x,
        //            camera.position.y,
        //            camera.viewportWidth * camera.zoom,
        //            camera.viewportHeight * camera.zoom);
        //}

        public void setCombinedMatrix(Matrix combined)
        {
            this.combined = combined;

            float invWidth = combined.M11;
            //возможно неправильно выбраны переменные матрицы
            float halfViewPortWidth = 1f / invWidth;
            float x = -halfViewPortWidth * combined.M14;
            x1 = x - halfViewPortWidth;
            x2 = x + halfViewPortWidth;

            float invHeight = combined.M22;

            float halfViewPortHeight = 1f / invHeight;
            float y = -halfViewPortHeight * combined.M24;
            y1 = y - halfViewPortHeight;
            y2 = y + halfViewPortHeight;
        }

        //вызывать постоянно
        public void setCombinedMatrix(Matrix combined, float x, float y, float viewPortWidth, float viewPortHeight)
        {
            this.combined = combined;

            float halfViewPortWidth = viewPortWidth * 0.5f;
            x1 = x - halfViewPortWidth;
            x2 = x + halfViewPortWidth;

            float halfViewPortHeight = viewPortHeight * 0.5f;
            y1 = y - halfViewPortHeight;
            y2 = y + halfViewPortHeight;
        }

        public bool intersect(float x, float y, float radius)
        {
            return x1 < x + radius && x2 > x - radius &&
                    y1 < y + radius && y2 > y - radius;
        }

        public void updateAndRender()
        {
            update();
            render();
        }

        public void update()
        {
            foreach (Light light in lightList)
            {
                light.Update();
            }
        }

        public void prepareRender()
        {
            lightRenderedLastFrame = 0;

            Core.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            Core.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            bool useLightMap = shadows || blur;
            Viewport tempV = Core.GraphicsDevice.Viewport;
            if (useLightMap)
            {
                //lightMap.frameBuffer.Begin(); нужно еще поставить вьюпорт
                Core.GraphicsDevice.SetRenderTarget(lightMap.frameBuffer);
                Core.GraphicsDevice.Viewport = new Viewport(0, 0, lightMap.frameBuffer.Bounds.Width,
                    lightMap.frameBuffer.Bounds.Height);
                Core.GraphicsDevice.Clear(Color.Transparent);
            }

            simpleBlendFunc.Apply();

            Effect shader = customLightShader != null ? customLightShader : lightShader;

            shader.CurrentTechnique.Passes[0].Apply();
            shader.Parameters["WorldViewProjection"].SetValue(combined);
            if (customLightShader != null) updateLightShader();

            foreach (Light light in lightList)
            {
                if (customLightShader != null) updateLightShaderPerLight(light);
                light.Render();
            }

            if (useLightMap)
            {
                
                if (customViewport)
                {
                    Core.GraphicsDevice.SetRenderTarget(null);
                    Core.GraphicsDevice.Viewport = new Viewport(viewportX, viewportY, viewportWidth,
                        viewportHeight);
                }
                else
                {
                    Core.GraphicsDevice.SetRenderTarget(null);
                    Core.GraphicsDevice.Viewport = tempV;
                }
            }

            //if (useLightMap && pseudo3d)
            //{
            //    lightMap.shadowBuffer.Begin();
            //    Core.GraphicsDevice.Clear(Color.Transparent);

            //    foreach (Light light in lightList)
            //    {
            //        light.dynamicShadowRender();
            //    }

            //    if (customViewport)
            //    {
            //        lightMap.shadowBuffer.End(
            //                viewportX,
            //                viewportY,
            //                viewportWidth,
            //                viewportHeight);
            //    }
            //    else
            //    {
            //        lightMap.shadowBuffer.End();
            //    }
            //}

            bool needed = lightRenderedLastFrame > 0;
            if (needed && blur)
                lightMap.GaussianBlur(lightMap.frameBuffer, blurNum);
            //if (needed && blur && pseudo3d)
            //    lightMap.GaussianBlur(lightMap.shadowBuffer, blurNum);
        }

        public void render()
        {
            prepareRender();
            lightMap.Render();
        }

        public void renderOnly()
        {
            lightMap.Render();
        }

        protected void updateLightShader()
        {

        }

        protected void updateLightShaderPerLight(Light light)
        {

        }

        public bool pointAtLight(float x, float y)
        {
            foreach (Light light in lightList)
            {
                if (light.Contains(x, y)) return true;
            }
            return false;
        }

        public bool pointAtShadow(float x, float y)
        {
            foreach (Light light in lightList)
            {
                if (light.Contains(x, y)) return false;
            }
            return true;
        }

        //public void Dispose()
        //{
        //    removeAll();
        //    if (lightMap != null) lightMap.Dispose();
        //    if (lightShader != null) lightShader.Dispose();
        //}

        //public void removeAll()
        //{
        //    foreach (Light light in lightList)
        //    {
        //        light.dispose();
        //    }
        //    lightList.Clear();

        //    foreach (Light light in disabledLights)
        //    {
        //        light.dispose();
        //    }
        //    disabledLights.Clear();
        //}

        public void setLightShader(Effect customLightShader)
        {
            this.customLightShader = customLightShader;
        }

        public void setCulling(bool culling)
        {
            this.culling = culling;
        }

        public void setBlur(bool blur)
        {
            this.blur = blur;
        }

        public void setBlurNum(int blurNum)
        {
            this.blurNum = blurNum;
        }

        public void setShadows(bool shadows)
        {
            this.shadows = shadows;
        }

        public void setAmbientLight(float ambientLight)
        {
            this.ambientLight.A =(byte) Math.Round(MathHelper.Clamp(ambientLight*255, 0f, 255f));
        }

        public void setAmbientLight(float r, float g, float b, float a)
        {
            ambientLight = new Color(r, g, b, a);
        }

        public void setAmbientLight(Color ambientLightColor)
        {
            ambientLight = ambientLightColor;
        }

        public void setWorld(World world)
        {
            this.world = world;
        }

        public static bool getGammaCorrection()
        {
            return gammaCorrection;
        }

        public void applyGammaCorrection(bool gammaCorrectionWanted)
        {
            gammaCorrection = gammaCorrectionWanted;
            gammaCorrectionParameter = gammaCorrection ? GAMMA_COR : 1f;
            lightMap.CreateShaders();
        }

        public void setDiffuseLight(bool useDiffuse)
        {
            isDiffuse = useDiffuse;
            lightMap.CreateShaders();
        }

        public static bool isDiffuseLight()
        {
            return isDiffuse;
        }

        public static float getDynamicShadowColorReduction()
        {
            return dynamicShadowColorReduction;
        }

        public static void useDiffuseLight(bool useDiffuse)
        {
            isDiffuse = useDiffuse;
        }

        public static void setGammaCorrection(bool gammaCorrectionWanted)
        {
            gammaCorrection = gammaCorrectionWanted;
            gammaCorrectionParameter = gammaCorrection ? GAMMA_COR : 1f;
        }

        public void setLightMapRendering(bool isAutomatic)
        {
            lightMap.lightMapDrawingDisabled = !isAutomatic;
        }

        //public Texture2D getLightMapTexture()
        //{
        //    return lightMap.frameBuffer;
        //}

        public RenderTarget2D getLightMapBuffer()
        {
            return lightMap.frameBuffer;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}


