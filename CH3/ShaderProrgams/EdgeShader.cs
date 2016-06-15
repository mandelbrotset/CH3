﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class EdgeShader : BasicShaderProgram
    {


        public EdgeShader ()
        {


        }

        public new void initShader()
        {
            Console.WriteLine("INIT MODEL");

            loadVertShader("../../shaders/edgeVert.vert");
            loadFragShader("../../shaders/edgeFrag.frag");
            loadProgram();


            vertexTexCoordIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "texCoord");
            vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
            vertexNormalIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexNormal");
        }



    }
}
