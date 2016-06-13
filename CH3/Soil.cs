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
        public Soil(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, float texScale) : base(position, 5 * scale, 0, 0, rotation, shader)
        {
            LoadModel("../../models/grass.obj", "../../textures/grass.png", texScale);
        }

        public new void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix, DirectionalLight light)
        {
            // ((FloorShaderProgram)shader).setResolution(new Vector2(Window.WIDTH, Window.HEIGHT));
        //    Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.Repeat);
        //    Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.Repeat);
        //    Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
        //    Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 11);
            //Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMa, 16);
         //   Gl.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        //    Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);
            
            
            base.render(time, projectionMatrix, viewMatrix, light);
            
        }

    }
}