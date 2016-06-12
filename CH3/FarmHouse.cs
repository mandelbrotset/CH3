using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class FarmHouse : Building
    {
        public FarmHouse(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, 1*scale, rotation, shader)
        {
            LoadModel("../../models/farmhouse.obj", "../../textures/farmhouse.jpg");
        }
    }
}
