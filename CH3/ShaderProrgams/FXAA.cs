using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class FXAAShader : BasicShaderProgram
    {


        public FXAAShader ()
        {


        }

        public new void initShader()
        {

            loadVertShader("../../shaders/edgeVert.vert");
            loadFragShader("../../shaders/FXAA.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }



    }
}
