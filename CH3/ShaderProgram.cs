using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pencil.Gaming.Graphics;

namespace CH3
{
    public abstract class ShaderProgram
    {

        public uint program { get; protected set; }
        public uint fragShader { get; protected set; }
        public uint vertShader { get; protected set; }

        public void useProgram() {
            if (program != 0) {
                GL.UseProgram(program);
            }
        }

        protected unsafe bool loadFragShader(string path) {

            fragShader = GL.CreateShader(ShaderType.FragmentShader);


            string code = loadTextFile(path);
            if (code.Equals(""))
                return false;
            string[] array = { code };

            GL.ShaderSource(fragShader, 1, array, null);

            return true;

        }
        protected unsafe bool loadVertShader(string path) {

            vertShader = GL.CreateShader(ShaderType.FragmentShader);

            string code = loadTextFile(path);
            if (code.Equals(""))
                return false;
            string[] array = { code };

            GL.ShaderSource(vertShader, 1, array, null);

            return true;

        }

        protected bool loadProgram() {
            if (fragShader == 0 || vertShader == 0)
                return false;

            program = GL.CreateProgram();
            GL.AttachShader(program, vertShader);
            GL.AttachShader(program, fragShader);
            GL.LinkProgram(program);

            return true;

        }

        private string loadTextFile(string path) {
            string @string = "";
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(path);

                @string = file.ReadToEnd();

                file.Close();

                Console.WriteLine("\n\nLOADED SHADER:\n\n\t" + @string.Replace("\n", "\n\t"));


            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            return @string;

        }
    }
}