using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public abstract class DynamicObject : GameObject
    {
        public Vector3 velocity { get; set; }
        public delegate void UpdateCamera();
        private UpdateCamera updateCamera;

        public DynamicObject(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, scale,0,0, rotation, shader, celShader, normalShader, depthShader)
        {
        }

        public void SetUpdateCamera(UpdateCamera updCam)
        {
            updateCamera = updCam;
        }

        public void Move()
        {
            position += velocity;
            if (updateCamera != null)
            {
                updateCamera();
            }
        }
    }
}