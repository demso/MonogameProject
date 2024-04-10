using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Box2DLight
{
    public class RayHandlerOptions
    {
        internal bool gammaCorrection = false;
        internal bool isDiffuse = false;

        internal bool pseudo3d = false;
        internal bool shadowColorInterpolation = false;

        public void setDiffuse(bool diffuse)
        {
            isDiffuse = diffuse;
        }

        public void setGammaCorrection(bool gammaCorrection)
        {
            this.gammaCorrection = gammaCorrection;
        }

        public void setPseudo3d(bool pseudo3d)
        {   
            setPseudo3d(pseudo3d, false);
        }

        public void setPseudo3d(bool pseudo3d, bool shadowColorInterpolation)
        {
            this.pseudo3d = pseudo3d;
            this.shadowColorInterpolation = shadowColorInterpolation;
        }
    }

}
