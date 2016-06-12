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

        public override void UpdateCamera()
        {
            position = new Vector3(follow.position.x, follow.position.y, height);
            target = follow.position;
            up = Vector3.Up;
            if (!fixedRotation)
            {
                up *= Matrix4.CreateRotationZ(follow.rotationZ);
            }
        }
    }
}
