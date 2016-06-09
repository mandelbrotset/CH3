using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class Player : DynamicObject
    {
        public Player(Vector3 position, Vector3 scale, float rotation, ShaderProgram shader) : base(position, scale, rotation, shader)
        {
        }

        public override void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            throw new NotImplementedException();
        }
    }
}