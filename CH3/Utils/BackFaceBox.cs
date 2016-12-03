using OpenTK;
using Gl = OpenGL.Gl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    public static class BackFaceBox
    {
        private static uint vertexbuffer = Gl.GenBuffer();
        private static uint VertexArrayID = Gl.GenVertexArray();

        public static void Render(BasicShaderProgram shader, Vector3 hext)
        {
            Vector3[] vertices = new Vector3[24];

            vertices[3] = new Vector3(-hext.X, -hext.Y, -hext.Z);
            vertices[2] = new Vector3(hext.X, -hext.Y, -hext.Z);
            vertices[1] = new Vector3(hext.X, -hext.Y, hext.Z);
            vertices[0] = new Vector3(-hext.X, -hext.Y, hext.Z);

            vertices[4] = new Vector3(-hext.X, hext.Y, -hext.Z);
            vertices[5] = new Vector3(hext.X, hext.Y, -hext.Z);
            vertices[6] = new Vector3(hext.X, hext.Y, hext.Z);
            vertices[7] = new Vector3(-hext.X, hext.Y, hext.Z);

            vertices[8] = vertices[2];
            vertices[9] = vertices[5];
            vertices[10] = vertices[4];
            vertices[11] = vertices[3];

            vertices[12] = vertices[0];
            vertices[13] = vertices[7];
            vertices[14] = vertices[6];
            vertices[15] = vertices[1];

            vertices[16] = vertices[0];
            vertices[17] = vertices[3];
            vertices[18] = vertices[4];
            vertices[19] = vertices[7];

            vertices[20] = vertices[6];
            vertices[21] = vertices[5];
            vertices[22] = vertices[2];
            vertices[23] = vertices[1];

            Gl.BindVertexArray(VertexArrayID);
            Gl.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, vertexbuffer);

            Gl.BufferData(OpenGL.BufferTarget.ArrayBuffer, vertices.Length * 3 * sizeof(float), vertices, OpenGL.BufferUsageHint.StaticDraw);

            Gl.EnableVertexAttribArray(0);

            Gl.BindBuffer(OpenGL.BufferTarget.ArrayBuffer, vertexbuffer);
            Gl.VertexAttribPointer(shader.vertexPositionIndex, 3, OpenGL.VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            Gl.DrawArrays(OpenGL.BeginMode.Quads, 0, 4);
            Gl.DrawArrays(OpenGL.BeginMode.Quads, 4, 4);
            Gl.DrawArrays(OpenGL.BeginMode.Quads, 8, 4);
            Gl.DrawArrays(OpenGL.BeginMode.Quads, 12, 4);
            Gl.DrawArrays(OpenGL.BeginMode.Quads, 16, 4);
            Gl.DrawArrays(OpenGL.BeginMode.Quads, 20, 4);


            Gl.DisableVertexAttribArray(0);
        }
    }
}
