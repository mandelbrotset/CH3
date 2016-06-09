using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public abstract class GameObject : Drawable
    {

        public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public float rotation { get; set; }

        public ShaderProgram shader { protected get; set; }

        public GameObject(Vector3 position, Vector3 scale, float rotation, ShaderProgram shader) {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;

            this.shader = shader;
        }

        public abstract void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix);
    }
}