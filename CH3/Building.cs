using System;

using OpenGL;


namespace CH3
{
    public abstract class Building : GameObject
    {
        public Building(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, 1.0f*scale, -(float)Math.PI/2, 0, rotation, shader, celShader, normalShader, depthShader)
        {
        }

    }
}