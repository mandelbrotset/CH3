using OpenTK;
using Gl = OpenGL.Gl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    public static class DrawBox
    {


        private static uint vertexbuffer = Gl.GenBuffer();
        private static uint VertexArrayID = Gl.GenVertexArray();

        public static void Render(BasicShaderProgram shader, Vector3 min, Vector3 max) {
            Vector3[] vertices = new Vector3[16];

            vertices[0] = new Vector3(min.X, min.Y, min.Z);
            vertices[1] = new Vector3(max.X, min.Y, min.Z);
            vertices[2] = new Vector3(max.X, min.Y, max.Z);
            vertices[3] = new Vector3(min.X, min.Y, max.Z);
            vertices[4] = new Vector3(min.X, max.Y, min.Z);
            vertices[5] = new Vector3(max.X, max.Y, min.Z);
            vertices[6] = new Vector3(max.X, max.Y, max.Z);
            vertices[7] = new Vector3(min.X, max.Y, max.Z);

            vertices[8] = vertices[0];
            vertices[9] = vertices[4];
            vertices[10] = vertices[1];
            vertices[11] = vertices[5];
            vertices[12] = vertices[2];
            vertices[13] = vertices[6];
            vertices[14] = vertices[3];
            vertices[15] = vertices[7];

            Gl.BindVertexArray(VertexArrayID);
            Gl.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, vertexbuffer);

            Gl.BufferData(OpenGL.BufferTarget.ArrayBuffer, vertices.Length * 3 * sizeof(float), vertices, OpenGL.BufferUsageHint.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            
            Gl.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, vertexbuffer);
            Gl.VertexAttribPointer(shader.vertexPositionIndex, 3, OpenGL.VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            Gl.DrawArrays(OpenGL.BeginMode.LineLoop, 0, 4);
            Gl.DrawArrays(OpenGL.BeginMode.LineLoop, 4, 4);
            Gl.DrawArrays(OpenGL.BeginMode.Lines, 8, 4);
            Gl.DrawArrays(OpenGL.BeginMode.Lines, 12, 4);

            Gl.DisableVertexAttribArray(0);
        }
    }
}
