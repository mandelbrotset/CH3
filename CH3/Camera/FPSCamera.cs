using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using OpenGL;

namespace CH3.Camera
{
    class FPSCamera : Camera
    {
        public float yaw { get; set; }
        public float pitch { get; set; }
        public Vector3 translation { get; set; }
        private bool warped = false;
        private bool[] activeKeys;

        public void MoveCamera(int i)
        {
            handleMovement();
            Update(i);
            Glut.glutTimerFunc(1, MoveCamera, 0);
        }

        private void handleMovement()
        {
            float speed = 0.4f;
            if (activeKeys[119])//w
            {
                translation += new Vector3(speed, 0.0f, 0.0f);
            }
            if (activeKeys[97])//a
            {
                translation += new Vector3(0.0f, speed, 0.0f);
            }
            if (activeKeys[115])//s
            {
                translation += new Vector3(-speed, 0.0f, 0.0f);
            }
            if (activeKeys[100])//d
            {
                translation += new Vector3(0.0f, -speed, 0.0f);
            }
            if (activeKeys[32])
            {
                translation += new Vector3(0.0f, 0, speed);
            }
            if (activeKeys['c'])
            {
                translation += new Vector3(0.0f, 0, -speed);
            }
        }

        public FPSCamera(bool[] keys)
        {
            activeKeys = keys;
            position = Vector3.Zero;
            target = Vector3.Zero;
            translation = new Vector3(0.0f, 0.0f, 0.0f);
            pitch = 0;//(float)-Math.PI / 2;
            yaw = 0;
        }

        public FPSCamera(Vector3 position, Vector3 target, bool[] keys)
        {
            activeKeys = keys;
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

        public void Motion(int x, int y)
        {
            if (warped)
            {
                warped = false;
                return;
            }

            float dX = ((float)x - Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / 2);
            float dY = ((float)y - Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT) / 2);
            float sense = 0.001f;
            yaw -= dX * sense;
            pitch += dY * sense;
            warped = true;
            Glut.glutWarpPointer(Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / 2, Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT) / 2);

        }
    }

}
