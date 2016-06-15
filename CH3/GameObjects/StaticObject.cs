using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class StaticObject : GameObject
    {
        public StaticObject(Vector3 position, Vector3 scale, float rotationX, float rotationY, float rotationZ, Graphics graphics) : base(position, scale, rotationX, rotationY, rotationZ, graphics)
        {
        }

    }
}