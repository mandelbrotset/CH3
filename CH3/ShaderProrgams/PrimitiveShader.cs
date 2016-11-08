using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class PrimitiveShader : BasicShaderProgram
    {
        public PrimitiveShader() {}

        public new void initShader()
        {

            Console.WriteLine("INIT Primitive");
            loadVertShader("../../shaders/primitiveVert.vert");
            loadFragShader("../../shaders/primitiveFrag.frag");
            loadProgram();

            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }


        public void setColor(Vector3 color)
        {
            program["color"].SetValue(color);
        }
    }
}
