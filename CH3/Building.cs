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
    public class Building : GameObject
    {
        public Building(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, 1.0f*scale, -(float)Math.PI/2, 0, rotation, shader)
        {

            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new FileStream("../../models/house.obj", FileMode.Open);

            LoadResult result = objLoader.Load(fileStream);

            fileStream.Close();

            this.setVertices(result.Vertices);
           
            this.setFaces(result.Groups, result.Textures);

            texture = new OpenGL.Texture("../../textures/house.png");

        }

        public new void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {


            base.render(time, projectionMatrix, viewMatrix);
        }
    }
}