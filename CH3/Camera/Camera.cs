using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Camera
{
    abstract class Camera
    {
        public Vector3 target { get; set; }
        public Vector3 up { get; set; }
        public Vector3 position { get; set; }
        public Matrix4 viewMatrix
        {
            get
            {
                return Matrix4.LookAt(position, target, up);
            }
        }

    }
}
