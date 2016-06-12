using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class Tower : Building
    {
        public Tower(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, 2*scale, rotation, shader)
        {
            LoadModel("../../models/klcc_lores.obj", "../../textures/klccbake.jpg");

        }
    }
}
