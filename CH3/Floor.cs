using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using OpenGL;
using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;

namespace CH3
{
    public class Floor : GameObject
    {
        public Floor(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, 1000*scale, 0, 0, rotation, shader)
        {
            LoadModel("../../models/floor.obj", "../../textures/grass.png");
        }

        public new void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
           // ((FloorShaderProgram)shader).setResolution(new Vector2(Window.WIDTH, Window.HEIGHT));

            base.render(time, projectionMatrix, viewMatrix);
        }
    }
}