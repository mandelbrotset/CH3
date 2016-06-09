using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public abstract class DynamicObject : GameObject
    {
        public DynamicObject(Vector3 position, Vector3 scale, float rotation, ShaderProgram shader) : base(position, scale, rotation, shader)
        {
        }
    }
}