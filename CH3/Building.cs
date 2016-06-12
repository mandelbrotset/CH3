using System;

using OpenGL;


namespace CH3
{
    public abstract class Building : GameObject
    {
        public Building(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, 1.0f*scale, -(float)Math.PI/2, 0, rotation, shader)
        {
        }

    }
}