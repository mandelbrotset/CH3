using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;

namespace CH3
{
    public abstract class Drawable
    {

        private VBO<Vector3> vertices;
        private VBO<int> faces;
        public BasicShaderProgram shader { protected get; set; }
        public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public float rotation { get; set; }



        public void setVertices(IList<Vertex> vs) {

            Vector3[] array = new Vector3[vs.Count];
            int i = 0;
            foreach (Vertex vertex in vs) {
                array[i++] = new Vector3(vertex.X, vertex.Y, vertex.Z);
            }

            vertices = new VBO<Vector3>(array);
            Console.Write("Setting vertices");
        }

        public void setFaces(IList<Group> groups) {

            List<int> list = new List<int>();

            foreach (Group g in groups) {
                foreach (Face f in g.Faces) {

                    FaceVertex v1 = f[0];

                    for (int i = 1; i < (f.Count-1); i++) {
                        FaceVertex v2 = f[i];
                        FaceVertex v3 = f[i + 1];


                        list.Add(v1.VertexIndex);
                        list.Add(v2.VertexIndex);
                        list.Add(v2.VertexIndex);

                        Console.WriteLine("Adding index: " + v2.VertexIndex);

                    }
                }


            }

            int[] array = list.ToArray();

            faces = new VBO<int>(array);
            Console.Write("Setting faces");


        }


        public void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix) {

            Matrix4 scale = Matrix4.CreateScaling(this.scale);
            Matrix4 rotation = Matrix4.CreateRotation(Vector3.UnitZ, this.rotation);
            Matrix4 translation = Matrix4.CreateTranslation(this.position);

            BasicShaderProgram shader = (BasicShaderProgram)this.shader;

            shader.useProgram();
            shader.setTime(time);
            shader.setProjectionMatrix(projectionMatrix);
            shader.setViewMatrix(viewMatrix);
            shader.setModelMatrix(rotation * scale * translation);

            Gl.BindBuffer(vertices);

            Gl.VertexAttribPointer(shader.vertexPositionIndex, vertices.Size, vertices.PointerType, false, 12, IntPtr.Zero);
            Gl.BindBuffer(faces);

            Gl.DrawElements(BeginMode.Triangles, faces.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}