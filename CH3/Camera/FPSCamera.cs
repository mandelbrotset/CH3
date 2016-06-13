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
        
        public Vector3 translation { get; set; }
        private float pitch, yaw;
        private bool[] activeKeys;

        public FPSCamera(Vector3 position, Vector3 target)
        {
            activeKeys = new bool[255];
            this.position = position;
            this.target = target;
            this.up = Vector3.Up;
            Input.SubscribeMouseMovement(MouseMovement);
            Input.SubscribeKeyDown(KeyDown);
            Input.SubscribeKeyUp(KeyUp);
            Glut.glutTimerFunc(1, MoveCamera, 0);
        }

        public void MoveCamera(int i)
        {
            Update();
            handleMovement();
            Glut.glutTimerFunc(1, MoveCamera, 0);
        }

        private void KeyDown(byte key, int x, int y)
        {
            activeKeys[key] = true;
        }

        private void KeyUp(byte key, int x, int y)
        {
            activeKeys[key] = false;
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

        private void MouseMovement(float pitch, float yaw)
        {
            this.pitch = pitch;
            this.yaw = yaw;
        }

        private void Update()
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
