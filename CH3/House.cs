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
        public House(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, 10*scale, rotation, shader, normalShader, celShader, depthShader)
        {
            LoadModel("../../models/house.obj", "../../textures/house.png", 1);
        }
    }
}
