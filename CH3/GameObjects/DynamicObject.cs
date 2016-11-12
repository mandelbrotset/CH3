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

        public DynamicObject(Vector3 position, Vector3 scale, float rotationX, float rotationY, float rotationZ, Graphics graphics) : base(position, scale,rotationX,rotationY, rotationZ, graphics)
        {
            mass = 1;
        }

        public void SetUpdateCamera(UpdateCamera updCam)
        {
            updateCamera = updCam;
        }

        public void Tick()
        {
            //position += velocity;
            if (updateCamera != null)
            {
                updateCamera();
            }
        }

        public virtual void HandleInput(Vector2 mouse, float dt) { }
    }
}