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
        public Vector3 target { get; set; }
        public Vector3 position { get; set; }
        public Vector3 up { get; set; }
        public float yaw { get; set; }
        public float pitch { get; set; }
        public Vector3 translation { get; set; }

        public Matrix4 viewMatrix {
            get {
                return Matrix4.LookAt(position, target, up);
            }
        }

        public Camera()
        {
            position = Vector3.Zero;
            target = Vector3.Zero;
            translation = new Vector3(0.0f, 0.0f, 0.0f);
            pitch = 0;//(float)-Math.PI / 2;
            yaw = 0;
        }

        public Camera(Vector3 position, Vector3 target)
        {
            this.position = position;
            this.target = target;
            this.up = Vector3.Up;
        }

        public void Update(int i)
        {
            Matrix4 yawR = Matrix4.CreateRotationZ(yaw);
            Matrix4 pitchR = Matrix4.CreateRotationY(pitch);

            Matrix4 rotation = yawR * pitchR;

            //position:
            translation = translation * pitchR;
            translation = translation * yawR;
            this.position += translation;
            translation = new Vector3(0.0f, 0.0f, 0.0f);

            //target:
            Vector3 forward = new Vector3(1.0f, 0.0f, 0.0f);
            forward = forward * pitchR;
            forward =  forward * yawR;

            this.up = new Vector3(0, 0, 1);
            up *= pitchR;
            up *= yawR;
            this.target = this.position + forward;
        }
    }

}
