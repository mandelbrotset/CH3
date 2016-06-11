using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;
using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;

namespace CH3
{
    public abstract class GameObject : Drawable
    {


        


        public GameObject(Vector3 position, Vector3 scale, float rotationX, float rotationY, float rotationZ, BasicShaderProgram shader) {
            this.position = position;
            this.scale = scale;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;

            this.shader = shader;
        }

        
    }
}