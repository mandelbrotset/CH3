using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class GBufferShader : BasicShaderProgram
    {
        public GBufferShader() {}

        public new void initShader()
        {

            Console.WriteLine("INIT GBuffer");
            loadVertShader("../../shaders/gbuffer.vert");
            loadFragShader("../../shaders/gbuffer.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }

        public void setMVP(Matrix4 matrix)
        {
            program["MVP_matrix"].SetValue(matrix);
        }

        public void setNormal(Matrix4 matrix)
        {
            program["N_matrix"].SetValue(matrix);
        }

    }
}
