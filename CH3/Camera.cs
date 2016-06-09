using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using OpenGL;

namespace CH3
{
    class Camera
    {

        public Matrix4 viewMatrix {
            get {
                return Matrix4.LookAt(position, target, Vector3.Up);
            }

        }

        public Vector3 target
        {
            get; set;
        }

        public Vector3 position
        {
            get; set;
        }

        public Camera()
        {
            position = Vector3.Zero;
            target = Vector3.Zero;
        }

        public Camera(Vector3 position, Vector3 target)
        {
            this.position = position;
            this.target = target;
        }


    }

}
