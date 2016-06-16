using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using OpenGL;
using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;
using static CH3.Graphics;

namespace CH3
{
    public class PostProcessedImage : Drawable
    {

        public uint FBO { get; private set; }


        private int width;
        private int height;



        public PostProcessedImage(int width, int height, uint texture, uint fbo, Vector3 position, Graphics graphics) : base(position, new Vector3(1,1,1), 0, 0, 0, graphics)
        {
            this.texture = texture;
            this.FBO = fbo;
            this.width = width;
            this.height = height;

            float halfWidth = width / 2;
            float halfHeight = height / 2;

            Vector3[] vs = { new Vector3(-halfWidth, halfHeight, 0), new Vector3(-halfWidth, -halfHeight, 0), new Vector3(halfWidth, halfHeight, 0), new Vector3(halfWidth, -halfHeight, 0) };
            Vector3[] ns = { new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) };
            Vector2[] tex = { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0) };

            int[] indices = { 1, 2, 0, 1, 3, 2 };
              LoadModel(vs, ns, tex, indices);


        }

        public void Render(RenderMode renderMode)
        {
            Matrix4 ortho = Matrix4.CreateOrthographic(Window.WIDTH, Window.HEIGHT, -1, 1);
            base.Render(0, ortho, Matrix4.Identity, new DirectionalLight(Vector3.Zero), renderMode, false, false);
        }

    }
}