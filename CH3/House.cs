using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class House : Building
    {
        public House(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, 10*scale, rotation, shader)
        {
            LoadModel("../../models/house.obj", "../../textures/house.png", 1);
        }
    }
}
