using System;
using FarseerPhysics.Dynamics;

namespace Box2DLight
{
    public class LightData
    {
        public object userData = null;
        public float height;
        public bool shadow;
        int shadowsDropped = 0;

        public LightData(float h)
        {
            height = h;
        }

        public LightData(float h, bool shadow)
        {
            height = h;
            this.shadow = shadow;
        }

        public LightData(object data, float h, bool shadow)
        {
            height = h;
            userData = data;
            this.shadow = shadow;
        }

        public float GetLimit(float distance, float lightHeight, float lightRange)
        {
            float l = 0f;
            if (lightHeight > height)
            {
                l = distance * height / (lightHeight - height);
                float diff = lightRange - distance;
                if (l > diff)
                {
                    l = diff;
                }
            }
            else if (lightHeight == 0f)
            {
                l = lightRange;
            }
            else
            {
                l = lightRange - distance;
            }

            return l > 0 ? l : 0f;
        }

        public static object GetUserData(Fixture fixture)
        {
            // Fixture class is not available in C#, so this method cannot be translated directly
            // You need to provide the appropriate data structure or class for Fixture
            object data = fixture.UserData;
            if (data is LightData)
            {
                return ((LightData)data).userData;
            }
            else
            {
                return data;
            }
        }
    }
}

