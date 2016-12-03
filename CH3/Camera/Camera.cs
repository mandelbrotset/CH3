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
        public Matrix4 projection { get; protected set; }
        public OpenTK.Matrix4 projection_opentk { get; protected set; }

        public Matrix4 viewProjection { get; protected set; }
        public OpenTK.Matrix4 viewProjection_opentk { get; protected set; }

        public Matrix4 viewMatrix { get; protected set; }
        public OpenTK.Matrix4 viewMatrix_opentk { get; protected set; }

        protected bool captured;

        public float z_far = 4000.0f;
        public float z_near = 0.5f;


        protected float x_fov;
        protected float y_fov = 60.0f;


        public abstract void Update(Vector2 mouse);

        protected void UpdateMatrices()
        {
            viewMatrix = Matrix4.LookAt(position, target, up);
            viewMatrix_opentk = OpenTK.Matrix4.LookAt(
                    new OpenTK.Vector3(position.x, position.y, position.z),
                    new OpenTK.Vector3(target.x, target.y, target.z),
                    new OpenTK.Vector3(up.x, up.y, up.z));
            viewProjection = viewMatrix * projection;
            viewProjection_opentk = viewMatrix_opentk * projection_opentk;
        }

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
