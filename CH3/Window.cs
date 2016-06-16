using System;
using OpenGL;
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


        public GlfwWindowPtr window { get; private set; }


       

        public void destroyWindow()
        {
            Glfw.DestroyWindow(window);
        }

        public bool createWindow()
        {

            if(!Glfw.Init())
            {
                return false;
            }
            
            window = Glfw.CreateWindow(Window.WIDTH, Window.HEIGHT, TITLE + " - " + VERSION, GlfwMonitorPtr.Null, GlfwWindowPtr.Null);

            Glfw.MakeContextCurrent(window);
            
            return true;
        }


    }
}