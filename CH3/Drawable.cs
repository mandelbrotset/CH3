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
        public float rotationX { get; set; }
        public float rotationY { get; set; }
        public float rotationZ { get; set; }




        public void setVertices(IList<Vertex> vs) {

            Vector3[] array = new Vector3[vs.Count];
            int i = 0;
            foreach (Vertex vertex in vs) {
                array[i++] = new Vector3(vertex.X, vertex.Y, vertex.Z);
            }



            vertices = new VBO<Vector3>(array);

        }

        public void setFaces(IList<Group> groups) {

            List<int> list = new List<int>();

            foreach (Group g in groups) {
                foreach (Face f in g.Faces) {

                    FaceVertex v1 = f[0];
                    
                    for (int i = 1; i < (f.Count-1); i++) {
                        FaceVertex v2 = f[i];
                        FaceVertex v3 = f[i + 1];

                        list.Add(v1.VertexIndex - 1);
                        list.Add(v2.VertexIndex - 1);
                        list.Add(v3.VertexIndex - 1);
    
                    }
                }


            }

            int[] array = list.ToArray();

            faces = new VBO<int>(array, BufferTarget.ElementArrayBuffer);


        }


        public void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix) {

            Matrix4 scale = Matrix4.CreateScaling(this.scale);
            Matrix4 rotationZ = Matrix4.CreateRotation(Vector3.UnitZ, this.rotationZ);
            Matrix4 rotationX = Matrix4.CreateRotation(Vector3.UnitX, this.rotationX);
            Matrix4 rotationY = Matrix4.CreateRotation(Vector3.UnitY, this.rotationY);


            Matrix4 translation = Matrix4.CreateTranslation(this.position);

            BasicShaderProgram shader = (BasicShaderProgram)this.shader;


            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);

            shader.useProgram();
            shader.setTime(time);
            shader.setProjectionMatrix(projectionMatrix);
            shader.setViewMatrix(viewMatrix);
            shader.setModelMatrix(( rotationX * rotationY* rotationZ) * scale * translation);

            Gl.BindBuffer(vertices);

            Gl.VertexAttribPointer(shader.vertexPositionIndex, vertices.Size, vertices.PointerType, false, 12, IntPtr.Zero);
            Gl.BindBuffer(faces);

            Gl.DrawElements(BeginMode.Triangles, faces.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}