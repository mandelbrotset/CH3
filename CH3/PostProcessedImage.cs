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
    public class PostProcessedImage : GameObject
    {

        private int width;
        private int height;

        public PostProcessedImage(int width, int height, uint texture, Vector3 position, BasicShaderProgram shader) : base(position, new Vector3(1,1,1), 0, 0, 0, shader, null, null, null)
        {
            this.texture = texture;//new OpenGL.Texture("../../textures/sunrise.jpg");
            this.width = width;
            this.height = height;

            float halfWidth = width / 2;
            float halfHeight = height / 2;

            //LoadModel("../../models/grass.obj", "../../textures/grass.png");

            Vector3[] vs = { new Vector3(-halfWidth, halfHeight, 0), new Vector3(-halfWidth, -halfHeight, 0), new Vector3(halfWidth, halfHeight, 0), new Vector3(halfWidth, -halfHeight, 0) };
            Vector3[] ns = { new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) };
            Vector2[] tex = { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0) };

            int[] indices = { 1, 2, 0, 1, 3, 2 };
              LoadModel(vs, ns, tex, indices);
           // LoadModel("../../models/grass.obj", "../../textures/grass.png");

        }

        public void render()
        {

            /* Matrix4 translation = Matrix4.CreateTranslation(this.position);
             Matrix4 scale = Matrix4.CreateScaling(new Vector3(50, 50, 0));
             Matrix4 projectionMatrix = Matrix4.CreateOrthographic(100, 100, -1, 1);
             Matrix4 viewMatrix = Matrix4.Identity;


             shader.useProgram();
             shader.setProjectionMatrix(projectionMatrix);
             shader.setViewMatrix(viewMatrix);
             shader.setModelMatrix(translation*scale);

             Gl.BindBuffer(vertices);
             Gl.VertexAttribPointer(shader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);

             Gl.BindBuffer(normals);
             Gl.VertexAttribPointer(shader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);

             Gl.Enable(EnableCap.Texture2D);
             Gl.BindTexture(TextureTarget.Texture2D, texture.TextureID);

             Gl.BindBuffer(texCoords);
             Gl.VertexAttribPointer(shader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);

             Gl.BindBuffer(indices);

             Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

             Gl.BindTexture(TextureTarget.Texture2D, 0);*/
            // 
            Matrix4 ortho = Matrix4.CreateOrthographic(Window.WIDTH, Window.HEIGHT, -1, 1);

            base.render(0, ortho, Matrix4.Identity, new DirectionalLight(Vector3.Zero), Drawable.RENDER_MODE_BASIC);
        }

    }
}