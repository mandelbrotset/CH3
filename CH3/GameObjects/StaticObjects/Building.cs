using System;

using OpenGL;


namespace CH3
{
    public abstract class Building : StaticObject
    {
        public Building(Vector3 position, Vector3 scale, float rotationX, float rotationY, float rotationZ, Graphics graphics) : base(position, scale, rotationX, rotationY, rotationZ, graphics)
        {
        }

    }
}