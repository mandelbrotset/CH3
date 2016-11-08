using System;
using System.Runtime.InteropServices;
using OpenGL;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using OpenTK;
using OpenTK.Graphics;

namespace CH3
{
    public class Window
    {
        public static readonly int WIDTH = 1280;//(int)SFML.Window.VideoMode.DesktopMode.Width;
        public static readonly int HEIGHT = 720;//(int)SFML.Window.VideoMode.DesktopMode.Height;
        public static readonly string TITLE = "CH3";
        public static readonly string VERSION = "V0.01a";
        private ContextSettings contextSettings;
        public SFML.Window.Window window { get; private set; }

       // public void destroyWindow()
        //{
            //Glfw.DestroyWindow(window);
        //}


        public bool createWindow()
        {
            contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            window = new SFML.Window.Window(new VideoMode((uint)WIDTH, (uint)HEIGHT), TITLE, Styles.None, contextSettings);
            //window = new SFML.Window.Window(new VideoMode((uint)1280, (uint)720), TITLE, Styles.None, contextSettings);


            Toolkit.Init();
            GraphicsContext context = new GraphicsContext(new ContextHandle(IntPtr.Zero), null);

            SetupEventHandlers();
            window.SetKeyRepeatEnabled(false);
            window.SetMouseCursorVisible(true);
            window.SetActive();
            window.RequestFocus();

            return true;
        }

        private void SetupEventHandlers()
        {
            window.Closed += new EventHandler(OnClosed);
            window.KeyPressed += new EventHandler<KeyEventArgs>(Input.OnKeyPressed);
            window.KeyReleased += new EventHandler<KeyEventArgs>(Input.OnKeyReleased);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(Input.OnMouseMoved);
        }

        public void HandleEvents()
        {
            window.DispatchEvents();
        }

        static void OnClosed(object sender, EventArgs e)
        {
            SFML.Window.Window window = (SFML.Window.Window)sender;
            window.Close();
            Environment.Exit(0);
        }

        public void SwapBuffers()
        {
            window.Display();
        }
    }
}