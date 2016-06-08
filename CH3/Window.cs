using System;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;


namespace CH3
{
    public class Window 
    {
        public static readonly int WIDTH = 1280;
        public static readonly int HEIGHT = 720;
        public static readonly string TITLE = "CH3";
        public static readonly string VERSION = "V0.01a";



        public GlfwWindowPtr window
        {
            get; private set;

        }


        public bool isRunning()
        {
            if (!Glfw.WindowShouldClose(window))
            {

                return true;

            }
            destroyWindow();
            Glfw.Terminate();
            return false;
        }

        public void destroyWindow()
        {
            Glfw.DestroyWindow(window);

        }

        public bool createWindow()
        {

           
            if (!Glfw.Init())
            {

                return false;
            }

            Glfw.WindowHint(WindowHint.ContextVersionMajor, 2);
            Glfw.WindowHint(WindowHint.ContextVersionMinor, 0);


            window = Glfw.CreateWindow(WIDTH, HEIGHT, TITLE, GlfwMonitorPtr.Null, GlfwWindowPtr.Null);

            Glfw.MakeContextCurrent(window);

            Glfw.SwapInterval(1);

            return true;

        }
    }
}