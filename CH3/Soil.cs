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
    public class Soil : GameObject
    {
        Vector3[] positions;
        public Soil(Vector3[] positions, Vector3 scale, float rotation, BasicShaderProgram shader) : base(positions[0], 5 * scale, 0, 0, rotation, shader)
        {
            this.positions = positions;
            LoadModel("../../models/grass.obj", "../../textures/grass.png");
        }

        public new void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            // ((FloorShaderProgram)shader).setResolution(new Vector2(Window.WIDTH, Window.HEIGHT));
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 11);
            //Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMa, 16);
            Gl.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);

            foreach (Vector3 pos in positions)
            {
                position = pos;
                base.render(time, projectionMatrix, viewMatrix);
            }
        }

    }
}