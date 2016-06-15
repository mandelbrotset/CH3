using System;
using Tao.FreeGlut;
using OpenGL;

namespace CH3
{
    public class Window 
    {
        public static readonly int WIDTH = 1280;
        public static readonly int HEIGHT = 720;
        public static readonly string TITLE = "CH3";
        public static readonly string VERSION = "V0.01a";



        public int window
        {
            get; private set;

        }


        public void destroyWindow()
        {
            Glut.glutDestroyWindow(window);
        }

        public bool createWindow()
        {

            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH | Glut.GLUT_RGBA);
            Glut.glutInitWindowSize(WIDTH, HEIGHT);
            window = Glut.glutCreateWindow(TITLE);
            Glut.glutSetCursor(Glut.GLUT_CURSOR_NONE);

            return true;
        }


    }
}