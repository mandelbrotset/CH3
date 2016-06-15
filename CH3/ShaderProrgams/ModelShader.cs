using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class ModelShader : BasicShaderProgram
    {


        public ModelShader ()
        {


        }

        public new void initShader()
        {
            Console.WriteLine("INIT MODEL");

            loadVertShader("../../shaders/modelVert.vert");
            loadFragShader("../../shaders/modelFrag.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }

        public void setModelId(float id)
        {
            program["model_id"].SetValue(id);
        }


    }
}
