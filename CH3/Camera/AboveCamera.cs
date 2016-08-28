using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;

namespace CH3.Camera
{
    public class AboveCamera : ThirdPersonCamera
    {
        public float height { get; set; }
        public bool fixedRotation { get; set; }

        public AboveCamera() : base()
        {
            Input.SubscribeKeyPressed(KeyDown);
            fixedRotation = true;
        }

        private void KeyDown(Key key)
        {
            switch(key)
            {
                case Key.Space:
                    height += 5;
                    break;
                case Key.C:
                    height -= 5;
                    break;
            }
        }

        public override void UpdateCamera()
        {
            position = new Vector3(follow.position.x, follow.position.y, height);
            target = follow.position;
            up = Vector3.Up;
            if (!fixedRotation)
            {
                up *= Matrix4.CreateRotationZ(-follow.rotationZ);
            }
        }
    }
}
