using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    class DrawQuad
    {
        private uint VAO;
        public DrawQuad()
        {
            uint buffer = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);

            var points = new float[]
            {
                -1.0f, -1.0f, 1.0f, -1.0f,  1.0f, 1.0f,
                -1.0f, -1.0f, 1.0f,  1.0f, -1.0f, 1.0f
            };

            Gl.BufferData(BufferTarget.ArrayBuffer, points.Length * sizeof(float), points, BufferUsageHint.StaticDraw);

            Gl.BindVertexArray(VAO);
            Gl.VertexAttribPointer(0,2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            Gl.EnableVertexAttribArray(0);
        }

        public void render()
        {
 
            Gl.BindVertexArray(VAO);
            Gl.DrawArrays(BeginMode.Triangles, 0, 6);
        }
    }
}
