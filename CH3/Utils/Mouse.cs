using OpenGL;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    class Mouse
    {

        private bool captured;

        public void ToogleCaptured()
        {
            captured = !captured;
            Update();
        }

        public Vector2 Update()
        {
            if(captured) { 
                SFML.System.Vector2i center = new SFML.System.Vector2i(
                (int)SFML.Window.VideoMode.DesktopMode.Width / 2, (int)SFML.Window.VideoMode.DesktopMode.Height / 2);
                SFML.System.Vector2i delta = SFML.Window.Mouse.GetPosition() - center;
                SFML.Window.Mouse.SetPosition(center);
                return new Vector2((float)delta.X, (float)delta.Y);
            }
            return new Vector2(0, 0);
        }
    }
}
