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
        private VBO<Vector2> texCoordinates;

        private VBO<int> faces;

        public BasicShaderProgram shader { protected get; set; }
        public OpenGL.Texture texture { get; protected set; }
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


        public void setFaces(IList<Group> groups, IList<ObjLoader.Loader.Data.VertexData.Texture> tex) {

            List<int> list = new List<int>();
            List<int> texs = new List<int>();
            Vector2[] uvs = new Vector2[vertices.Count];



            foreach (Group g in groups) {
                foreach (Face f in g.Faces) {

                    FaceVertex v1 = f[0];
                    
                    for (int i = 1; i < (f.Count-1); i++) {
                        FaceVertex v2 = f[i];
                        FaceVertex v3 = f[i + 1];

                        list.Add(v1.VertexIndex - 1);
                        list.Add(v2.VertexIndex - 1);
                        list.Add(v3.VertexIndex - 1);

                        if (tex != null && tex.Count > 0)
                        {
                            var tex1 = tex[v1.TextureIndex - 1];
                            var tex2 = tex[v2.TextureIndex - 1];
                            var tex3 = tex[v3.TextureIndex - 1];

                            uvs[v1.VertexIndex - 1] = new Vector2(tex1.X, tex1.Y);
                            uvs[v2.VertexIndex - 1] = new Vector2(tex2.X, tex2.Y);
                            uvs[v3.VertexIndex - 1] = new Vector2(tex3.X, tex3.Y);

                        }
                        else {
                            uvs[v1.VertexIndex - 1] = new Vector2(1.0, 1.0);
                            uvs[v2.VertexIndex - 1] = new Vector2(1.0, 1.0);
                            uvs[v3.VertexIndex - 1] = new Vector2(1.0, 1.0);
                        }

                    }
                }


            }

            int[] array = list.ToArray();

            texCoordinates = new VBO<Vector2>(uvs);

            faces = new VBO<int>(array, BufferTarget.ElementArrayBuffer);


        }


        public void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix) {

            Matrix4 scale = Matrix4.CreateScaling(this.scale);
            Matrix4 rotationZ = Matrix4.CreateRotation(Vector3.UnitZ, this.rotationZ);
            Matrix4 rotationX = Matrix4.CreateRotation(Vector3.UnitX, this.rotationX);
            Matrix4 rotationY = Matrix4.CreateRotation(Vector3.UnitY, this.rotationY);


            Matrix4 translation = Matrix4.CreateTranslation(this.position);

            BasicShaderProgram shader = (BasicShaderProgram)this.shader;
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Texture2D);
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);

            if(texture != null)
             Gl.BindTexture(texture);

            shader.useProgram();
            shader.setTime(time);
            shader.setProjectionMatrix(projectionMatrix);
            shader.setViewMatrix(viewMatrix);
            shader.setModelMatrix(( rotationX * rotationY* rotationZ) * scale * translation);



            Gl.BindBuffer(texCoordinates);
            Gl.VertexAttribPointer(shader.vertexTexCoordIndex, texCoordinates.Size, texCoordinates.PointerType, true, 8, IntPtr.Zero);

            Gl.BindBuffer(vertices);
            Gl.VertexAttribPointer(shader.vertexPositionIndex, vertices.Size, vertices.PointerType, false, 12, IntPtr.Zero);
            Gl.BindBuffer(faces);

            Gl.DrawElements(BeginMode.Triangles, faces.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}