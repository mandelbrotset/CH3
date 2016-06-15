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
    public class Soil : StaticObject
    {
        public Soil(Vector3 position, Vector3 scale, float rotation, float texScale, Graphics graphics) : base(position, 5 * scale, 0, 0, rotation, graphics)
        {
            LoadModel("../../models/grass.obj", "../../textures/grass.png", texScale);
        }

    }
}