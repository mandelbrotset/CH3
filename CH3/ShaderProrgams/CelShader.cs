using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class CelShader : BasicShaderProgram
    {


        public CelShader()
        {


        }

        public new void initShader()

        {

            Console.WriteLine("INIT CEL");
            loadVertShader("../../shaders/celVert.vert");
            loadFragShader("../../shaders/celFrag.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }

    }
}
