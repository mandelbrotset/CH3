using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Camera
{
    class Above : ThirdPerson
    {
        public float height { get; set; }
        public bool fixedRotation { get; set; }

        public Above() : base()
        {
            Input.SubscribeKeyDown(KeyDown);
            fixedRotation = true;
        }

        private void KeyDown(byte key, int x, int y)
        {
            switch(key)
            {
                case 32:
                    height += 5;
                    break;
                case 99:
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
