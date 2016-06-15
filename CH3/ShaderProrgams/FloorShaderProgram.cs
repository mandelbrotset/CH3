using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class FloorShaderProgram : BasicShaderProgram
    {

        public FloorShaderProgram()
        {

            loadVertShader("../../shaders/floorVert.vert");
            loadFragShader("../../shaders/floorFrag.frag");
            loadProgram();

            
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            

        }


        public void setResolution(Vector2 res)
        {

            program["resolution"].SetValue(res);

        }



    }
}