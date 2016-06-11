using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class BasicShaderProgram : ShaderProgram
    {
        public int colorLocation { get; protected set; }

        public uint vertexPositionIndex { get; protected set; }
        

        public BasicShaderProgram()
        {

            loadVertShader("../../shaders/basicVert.vert");
            loadFragShader("../../shaders/basicFrag.frag");
            loadProgram();

            
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
        }

        public new void useProgram() {
            base.useProgram();
            Gl.EnableVertexAttribArray(vertexPositionIndex);


        }


        public void setTime(int time)
        {
            program["time"].SetValue((float)time);
        }

        public void setProjectionMatrix(Matrix4 matrix)
        {
            program["projection_matrix"].SetValue(matrix);
            
        }

        public void setViewMatrix(Matrix4 matrix)
        {
            program["view_matrix"].SetValue(matrix);

        }

        public void setModelMatrix(Matrix4 matrix)
        {
            program["model_matrix"].SetValue(matrix);

        }









    }
}