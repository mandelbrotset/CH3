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
        public FarmHouse(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, 1*scale, rotation, shader, normalShader, celShader, depthShader)
        {
            LoadModel("../../models/farmhouse.obj", "../../textures/farmhouse.jpg");
        }
    }
}
