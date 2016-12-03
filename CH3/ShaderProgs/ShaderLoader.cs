using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace CH3.Shaders
{
    public class ShaderLoader
    {
        public int vertex_shader_object, fragment_shader_object;

        public uint vertexPositionIndex { get; protected set; }
        public uint vertexTexCoordIndex { get; protected set; }
        public uint vertexNormalIndex   { get; protected set; }
        public uint program_id          { get; protected set; }

        public ShaderLoader() {
            uint tmp_program_id;
            using (StreamReader vs = new StreamReader("../../shaders/Tiled_Forward_Vert.c")) 
            using (StreamReader fs = new StreamReader("../../shaders/Tiled_Forward_Frag.c")) 
            CreateShaders(vs.ReadToEnd(), fs.ReadToEnd(), out vertex_shader_object, out fragment_shader_object, out tmp_program_id);
            
            this.program_id = tmp_program_id;

            vertexTexCoordIndex = (uint)GL.GetAttribLocation(program_id, "texCoord");
            vertexPositionIndex = (uint)GL.GetAttribLocation(program_id, "vertexPosition");
            vertexNormalIndex   = (uint)GL.GetAttribLocation(program_id, "vertexNormal");

        }

        public void UseProgram()
        {
            GL.UseProgram(program_id);
            GL.EnableVertexAttribArray(vertexPositionIndex);
            GL.EnableVertexAttribArray(vertexTexCoordIndex);
            GL.EnableVertexAttribArray(vertexNormalIndex);
        }

        void CreateShaders(string vs, string fs,
                    out int vertexObject, out int fragmentObject,
                    out uint program)
        {
            int status_code;
            string info;

            vertexObject = GL.CreateShader(ShaderType.VertexShader);
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
                throw new ApplicationException(info);

            // Compile fragment shader
            GL.ShaderSource(fragmentObject, fs);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out status_code);


            Console.WriteLine("Status: " + status_code);
            if (status_code != 1)
                throw new ApplicationException(info);

            program = (uint)GL.CreateProgram();
            GL.AttachShader((int)program, fragmentObject);
            GL.AttachShader((int)program, vertexObject);

            
            GL.BindAttribLocation(program, 0, "texCoord");
            GL.BindAttribLocation(program, 1, "vertexPosition");
            GL.BindAttribLocation(program, 2, "vertexNormal");
            

            GL.LinkProgram(program);
            GL.UseProgram(program);
        }
}

}
