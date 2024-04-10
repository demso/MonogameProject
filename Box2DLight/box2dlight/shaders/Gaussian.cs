using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Box2DLight.box2dlight.shaders
{
    internal class Gaussian
    {
        static public Effect CreateBlurShader(float w, float h)
        {
            string gamma = "";
            if (RayHandler.getGammaCorrection())
                gamma = "sqrt";

            string vertexShader =
                "attribute vec4 vertex_positions;\n" //
                + "attribute vec4 quad_colors;\n" //
                + "attribute float s;\n"
                + "uniform mat4 u_projTrans;\n" //
                + "varying vec4 v_color;\n" //				
                + "void main()\n" //
                + "{\n" //
                + "   v_color = s * quad_colors;\n" //				
                + "   gl_Position =  u_projTrans * vertex_positions;\n" //
                + "}\n";
            string fragmentShader = "#ifdef GL_ES\n" //
                                    + "precision lowp float;\n" //
                                    + "#define MED mediump\n"
                                    + "#else\n"
                                    + "#define MED \n"
                                    + "#endif\n" //
                                    + "varying vec4 v_color;\n" //
                                    + "void main()\n"//
                                    + "{\n" //
                                    + "  gl_FragColor = " + gamma + "(v_color);\n" //
                                    + "}";

            //ShaderProgram.pedantic = false;
            //ShaderProgram lightShader = new ShaderProgram(vertexShader,
            //    fragmentShader);
            //if (!lightShader.isCompiled())
            //{
            //    lightShader = new ShaderProgram("#version 330 core\n" + vertexShader,
            //        "#version 330 core\n" + fragmentShader);
            //    if (!lightShader.isCompiled())
            //    {
            //        Gdx.app.log("ERROR", lightShader.getLog());
            //    }
            //}

            return null;
        }
    }
}
