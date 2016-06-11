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
            pitch = (float)-Math.PI / 2;
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
            Matrix4 yawR = new Matrix4(
                new Vector4(Math.Cos(yaw), 0, Math.Sin(yaw), 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(-Math.Sin(yaw), 0, Math.Cos(yaw), 0),
                new Vector4(0, 0, 0, 1)
                );

            Matrix4 pitchR = new Matrix4(
               new Vector4(1, 0, 0, 0),
               new Vector4(0, Math.Cos(pitch), -Math.Sin(pitch), 0),
               new Vector4(0, Math.Sin(pitch), Math.Cos(pitch), 0),
               new Vector4(0, 0, 0, 1)
               );

            Matrix4 rotation = yawR *pitchR;

            //position:
            translation = translation * rotation;
            this.position += translation;
            translation = new Vector3(0.0f, 0.0f, 0.0f);

            //target:
            Vector3 temp = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 forward =  temp * rotation;
            this.up = rotation * Vector3.Up;
            this.target = this.position + forward;
            
            Console.WriteLine($"position: {position} target: {target} yaw: {yaw} pitch: {pitch} translation: {translation}");
        }
    }

}
