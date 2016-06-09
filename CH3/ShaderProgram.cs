using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;


namespace CH3
{
    public abstract class ShaderProgram
    {

        public OpenGL.ShaderProgram program { get; protected set; }
        public OpenGL.Shader fragShader { get; protected set; }
        public OpenGL.Shader vertShader { get; protected set; }

        private string vertexShaderPath;
        private string fragmentShaderPath;

        private bool fragChanged = false;
        private bool vertChanged = false;
        


        public void useProgram() {
            if (program != null) {

                if (fragChanged)
                {
                    fragChanged = !(reloadFragShader());
                    
                } else if(vertChanged) {
                    vertChanged = !reloadVertShader();
                }

                program.Use();
            }
        }

        public void unuseProgram() {
            program.Dispose();
        }

        private bool reloadFragShader() {

            string code = loadTextFile(fragmentShaderPath);
            if (code.Equals(""))
                return false;

            fragShader.Dispose();
            fragShader = new Shader(code, ShaderType.FragmentShader);


            reloadProgram();

            return true;
        }

        private bool reloadVertShader()
        {
            string code = loadTextFile(vertexShaderPath);
            if (code.Equals(""))
                return false;
            vertShader.Dispose();

            vertShader = new Shader(code, ShaderType.VertexShader);



            reloadProgram();

            return true;
        }

        protected bool loadFragShader(string path) {
            fragmentShaderPath = path;
            string code = loadTextFile(path);
            if (code.Equals(""))
                return false;

            fragShader = new Shader(code, ShaderType.FragmentShader);

            Console.WriteLine(fragShader.ShaderLog);

            createFragmentShaderWatcher(path);


            return true;

        }
        protected bool loadVertShader(string path) {
            vertexShaderPath = path;

            string code = loadTextFile(path);
            if (code.Equals(""))
                return false;

            vertShader = new Shader(code, ShaderType.VertexShader);

            Console.WriteLine(vertShader.ShaderLog);

            createFragmentShaderWatcher(path);

            return true;

        }

        protected bool loadProgram() {
            if (vertShader == null || fragShader == null)
                return false;


            program = new OpenGL.ShaderProgram(vertShader, fragShader);
            return true;

        }

        protected bool reloadProgram()
        {
            if (vertShader == null || fragShader == null)
                return false;

            program.Dispose();

            program = new OpenGL.ShaderProgram(vertShader, fragShader);
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
            catch (System.IO.IOException e)
            {
                return "";
            }

            return @string;

        }




        private void createVertexShaderWatcher(string path)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            string[] splitted = path.Split('/');
            int index = path.LastIndexOf('/');
            watcher.Path = path.Substring(0, index + 1);
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite;


            watcher.Filter = splitted[splitted.Length - 1];



            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnVertexShaderChanged);


            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }


        private void createFragmentShaderWatcher(string path)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            string[] splitted = path.Split('/');
            int index = path.LastIndexOf('/');
            watcher.Path = path.Substring(0, index+1);
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = splitted[splitted.Length - 1];


            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnFragmentShaderChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnVertexShaderChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            vertChanged = true;
        }

        // Define the event handlers.
        private void OnFragmentShaderChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            fragChanged = true;
        }
    }
}