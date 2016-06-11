using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class Human : Player
    {
        public Human(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, scale, rotation, shader)
        {
        }
    }
}