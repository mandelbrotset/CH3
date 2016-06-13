using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class DepthShader : BasicShaderProgram
    {


        public DepthShader()
        {


        }

        public new void initShader()

        {

            Console.WriteLine("INIT Depth");
            loadVertShader("../../shaders/depthVert.vert");
            loadFragShader("../../shaders/depthFrag.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }

    }
}
