using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class NormalShader : BasicShaderProgram
    {

        public NormalShader ()
        {


        }

        public new void initShader()
        {
            Console.WriteLine("INIT CEL");

            loadVertShader("../../shaders/normalVert.vert");
            loadFragShader("../../shaders/normalFrag.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }




    }
}
