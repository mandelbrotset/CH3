using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pencil.Gaming.Graphics;


namespace CH3
{
    public class BasicShaderProgram : ShaderProgram
    {
        public int colorLocation { get; private set; }

        public BasicShaderProgram()
        {

            loadFragShader("../../shaders/basicVert.vert");
            loadVertShader("../../ shaders / basicFrag.frag");
            loadProgram();

            colorLocation = GL.GetAttribLocation(program, "color");

            GL.EnableVertexAttribArray(colorLocation);
            GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, IntPtr.Zero);

        }
    }
}