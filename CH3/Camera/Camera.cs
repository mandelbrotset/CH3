using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Camera
{
    public abstract class Camera
    {
        public Vector3 target { get; set; }
        public Vector3 up { get; set; }
        public Vector3 position { get; set; }
        public Matrix4 perspectiveMatrix { get; protected set; }

        public Matrix4 viewMatrix
        {
            get
            {
                return Matrix4.LookAt(position, target, up);
            }
        }

        public OpenTK.Matrix4 viewMatrix_opentk
        {
            get
            {
                return OpenTK.Matrix4.LookAt(
                    new OpenTK.Vector3(position.x, position.y, position.z),
                    new OpenTK.Vector3(target.x, target.y, target.z),
                    new OpenTK.Vector3(up.x, up.y, up.z));
            }
        }

        protected bool captured;
        protected float z_far = 500000.0f;
        protected float z_near = 0.1f;
        protected float x_fov;
        protected float y_fov = 60.0f;

        public abstract void Update(Vector2 mouse);

        protected float CalcFovX(float fov_y, float width, float height) {
            float a;
            float y;

            if (fov_y < 1.0f) fov_y = 1.0f;
            if (fov_y > 179.0f) fov_y = 179.0f;

            y = height / (float)Math.Tan(fov_y / 360.0 * Math.PI);
            a = (float)Math.Atan(width / y);
            a = a * 360.0f / (float)Math.PI;
            
            return a;
        }

        protected float CalcFovY(float fov_x, float width, float height) {
            float a;
            float x;

            if (fov_x < 1.0f) fov_x = 1.0f;
            if (fov_x > 179.0f) fov_x = 179.0f;

            x = width / (float)Math.Tan(fov_x / 360.0 * Math.PI);
            a = (float)Math.Atan(height / x);
            a = a * 360.0f / (float)Math.PI;

            return a;
        }
    }
}
