using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class Ai
    {
    }

    public class AI : Player
    {
        public AI(Vector3 position, Vector3 scale, float rotation, ShaderProgram shader) : base(position, scale, rotation, shader)
        {
        }
    }
}