using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;


namespace CH3
{
    public class Game
    {
        



        public HUD HUD
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public World World
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public Graphics Graphics
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }


        private Window gameWindow;
        private BasicShaderProgram shader;


        public Game()
        {
            gameWindow = new Window();

            if (!gameWindow.createWindow()) {
                Console.WriteLine("ERROR: Could not initialize GLFW");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }


            Glfw.SetErrorCallback((GlfwError error, String msg) => {
                
                Console.WriteLine("ERROR: " + msg);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            });
            Glfw.SetWindowSizeCallback(gameWindow.window, (GlfwWindowPtr window, int width, int height) => {
                GL.Viewport(0, 0, width, height);
                
            });


            shader = new BasicShaderProgram();



        }


        public unsafe void run(int fps)
        {

            int frameCounter = 0;
            double beforeTime = Glfw.GetTime();

            int vertexBuffer;
            GL.GenBuffers(1, out vertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

          //  GL.BufferData(BufferTarget.ArrayBuffer, 1, data, BufferUsageHint.StaticDraw);

                
            while (gameWindow.isRunning())
            {
                double currentTime = Glfw.GetTime();

                handleEvents();
                update();
                render();
                frameCounter++;

                double afterTime = Glfw.GetTime();




                if (afterTime - beforeTime >= 1) {
                    Console.WriteLine(frameCounter + "FPS");
                    frameCounter = 0;
                    beforeTime = Glfw.GetTime();
                }



            }

        }

        private void handleEvents()
        {
            Glfw.PollEvents();

            if (Glfw.GetKey(gameWindow.window, Key.Escape))
                Glfw.SetWindowShouldClose(gameWindow.window, true);
            
        }

        private void render()
        {

            int width;
            int height;
            float ratio;
            double time = Glfw.GetTime();

            Glfw.GetFramebufferSize(gameWindow.window, out width, out height);

            ratio = width / (float)height;

            GL.Viewport(0, 0, width, height);


            float alpha = 1.0f;
            float red = (float)Math.Sin(Glfw.GetTime());
            float green = 0.5f;
            float blue = 0.5f;

            GL.ClearColor(red, green, blue, alpha);
                
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float[] matrix = new float[16];


            shader.useProgram();


            Glfw.SwapBuffers(gameWindow.window);

        }

        private void update()
        {
        }
    }
}