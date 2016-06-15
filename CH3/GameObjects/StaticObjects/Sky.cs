using System;

using OpenGL;


namespace CH3
{
    public class Sky : StaticObject
    {
        public Sky(Vector3 position, Vector3 scale, float rotationX, float rotationY, float rotationZ, Graphics graphics) : base(position, scale, rotationX, rotationY, rotationZ, graphics)
        {
            LoadModel("../../models/Sky-Sphere.obj", "../../textures/SKY_08.tif", 1);

        }

    }
}