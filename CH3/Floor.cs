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
        public Floor(Vector3 position, Vector3 scale, float rotation, FloorShaderProgram shader) : base(position, 1000*scale, 0, 0, rotation, shader)
        {

            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new FileStream("../../models/floor.obj", FileMode.Open);

            LoadResult result = objLoader.Load(fileStream);

            fileStream.Close();

            this.setVertices(result.Vertices);

            this.setFaces(result.Groups, result.Textures);
        }

        public new void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            ((FloorShaderProgram)shader).setResolution(new Vector2(Window.WIDTH, Window.HEIGHT));

            base.render(time, projectionMatrix, viewMatrix);
        }
    }
}