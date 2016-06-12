using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    class Grass : GameObject
    {
        public Grass(Vector3 position, Vector3 scale, float rotationX, float rotationY, float rotationZ, BasicShaderProgram shader) : base(position, scale, -((float)Math.PI/2), rotationY, rotationZ, shader)
        {
            LoadModel("../../models/HighPolyGrass.obj", "../../textures/Green.png");
        }
    }
}
