using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

using Gl = OpenGL.Gl;
using CH3.Shaders;

namespace CH3
{
    public static class DrawQuad
    {
        private static uint vertexbuffer = (uint)GL.GenBuffer();
        private static uint VertexArrayID = (uint)GL.GenVertexArray();

        public static void Render(BasicShaderProgram shader)
        {
            Vector2[] vertices = new Vector2[4];

            vertices[0] = new Vector2(-1.0f, -1.0f);
            vertices[1] = new Vector2(1.0f, -1.0f);
            vertices[2] = new Vector2(1.0f, 1.0f);
            vertices[3] = new Vector2(-1.0f, 1.0f);

            GL.BindVertexArray(VertexArrayID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, (int)vertexbuffer);

            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * 2 * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, (int)vertexbuffer);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            GL.DrawArrays(BeginMode.Quads, 0, 4);

            GL.DisableVertexAttribArray(0);
        }

        public static void Render(PrimitiveShader2 shader, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2[] vertices = new Vector2[4];

            vertices[0] = p0;
            vertices[1] = p1;
            vertices[2] = p2;
            vertices[3] = p3;
            
            GL.BindVertexArray(VertexArrayID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, (int)vertexbuffer);

            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * 2 * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexbuffer);
            GL.VertexAttribPointer(shader.vertexPositionIndex, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

            GL.DrawArrays(BeginMode.Quads, 0, 4);

           
            Vector4 color = new Vector4(0.1f, 0.1f, 0.1f, 0.5f);
            GL.Uniform4(GL.GetUniformLocation(shader.program_id, "color"), ref color);

            GL.DrawArrays(BeginMode.LineLoop, 0, 4);

            GL.DisableVertexAttribArray(0);
  
        }
    }
}
