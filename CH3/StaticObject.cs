using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class StaticObject : GameObject
    {
        public StaticObject(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, scale, 0,0, rotation, shader, celShader, normalShader, depthShader)
        {
        }

    }
}