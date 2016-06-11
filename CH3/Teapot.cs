using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenGL;
using Tao.FreeGlut;
using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.Data.Elements;


namespace CH3
{
    class Teapot : GameObject
    {
        public Teapot(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, scale, 0,0, rotation, shader)
        {
        }

        public new void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {


            Matrix4 scale = Matrix4.CreateScaling(this.scale);
            Matrix4 rotation = Matrix4.CreateRotation(Vector3.UnitZ, this.rotationZ);
            Matrix4 translation = Matrix4.CreateTranslation(this.position);

            BasicShaderProgram shader = (BasicShaderProgram)this.shader;

            shader.useProgram();
            shader.setTime(time);
            shader.setProjectionMatrix(projectionMatrix);
            shader.setViewMatrix(viewMatrix);
            shader.setModelMatrix(rotation * scale * translation );

            Glut.glutWireTeapot(1);


        }


    }
}
