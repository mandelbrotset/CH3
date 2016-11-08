using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using static SFML.Window.Keyboard;

namespace CH3.Camera
{
    public class FPSCamera : Camera
    {

        public Vector3 translation { get; set; }
        private float pitch, yaw;

        public FPSCamera(Vector3 position, Vector3 target)
        {
            this.position = position;
            this.target = target;
            this.up = Vector3.Up;
            pitch = yaw = 0;
            Input.SubscribeMouseMovement(MouseMovement);

            y_fov = CalcFovY(60.0f, 16.0f, 9.0f);
            x_fov = CalcFovX(y_fov, (float)Window.WIDTH, (float)Window.HEIGHT);
            perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(y_fov * (float)(Math.PI / 180.0),
                ((float)Window.WIDTH / (float)Window.HEIGHT), (float)z_near, (float)z_far);
        }

        private void handleMovement()
        {
            float speed = 2.0f;
            if (Input.IsKeyActive(Key.W))
            {
                translation += new Vector3(speed, 0.0f, 0.0f);
            }
            if (Input.IsKeyActive(Key.A))
            {
                translation += new Vector3(0.0f, speed, 0.0f);
            }
            if (Input.IsKeyActive(Key.S))
            {
                translation += new Vector3(-speed, 0.0f, 0.0f);
            }
            if (Input.IsKeyActive(Key.D))
            {
                translation += new Vector3(0.0f, -speed, 0.0f);
            }
            if (Input.IsKeyActive(Key.Space))
            {
                translation += new Vector3(0.0f, 0, speed);
            }
            if (Input.IsKeyActive(Key.C))
            {
                translation += new Vector3(0.0f, 0, -speed);
            }
        }

        private void MouseMovement(float pitch, float yaw)
        {


        }

        public override void Update(Vector2 mouse)
        {
            handleMovement();
            float sense = 0.005f;

            this.yaw -= mouse.x * sense;
            this.pitch += mouse.y * sense;

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
            forward = forward * yawR;

            this.up = new Vector3(0, 0, 1);
            up *= pitchR;
            up *= yawR;
            this.target = this.position + forward;
        }
    }
}
