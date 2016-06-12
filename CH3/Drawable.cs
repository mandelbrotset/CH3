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
        private VBO<Vector2> texCoords;
        private VBO<int> indices;



        public BasicShaderProgram shader { protected get; set; }
        public OpenGL.Texture texture { get; protected set; }
        public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public float rotationX { get; set; }
        public float rotationY { get; set; }
        public float rotationZ { get; set; }




        public void setFaces(IList<Group> groups, IList<Vertex> vs, IList<ObjLoader.Loader.Data.VertexData.Texture> tex) {

            Dictionary<string, int> elementMapping = new Dictionary<string, int>();
            List<Vector3> pos = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> inds = new List<int>();

            Vector3[] positions = new Vector3[vs.Count];
            Vector2[] textures = new Vector2[tex.Count];

            int j = 0;
            foreach (Vertex vertex in vs)
            {
                positions[j++] = new Vector3(vertex.X, vertex.Y, vertex.Z);
            }

            j = 0;
            foreach (var uv in tex)
            {
                textures[j++] = new Vector2(uv.X, uv.Y);
            }

            int totalIndex = 0;


            foreach (Group g in groups) {
                foreach (Face f in g.Faces) {

                    FaceVertex v1 = f[0];

                    
                    for (int i = 1; i < (f.Count-1); i++) {
                        FaceVertex v2 = f[i];
                        FaceVertex v3 = f[i + 1];

                        Vector3 pos1 = positions[v1.VertexIndex - 1];
                        Vector3 pos2 = positions[v2.VertexIndex - 1];
                        Vector3 pos3 = positions[v3.VertexIndex - 1];

                        Vector2 tex1 = textures[v1.TextureIndex - 1];
                        Vector2 tex2 = textures[v2.TextureIndex - 1];
                        Vector2 tex3 = textures[v3.TextureIndex - 1];

                        string str1 = pos1.ToString() + ":" + tex1.ToString();
                        string str2 = pos2.ToString() + ":" + tex2.ToString();
                        string str3 = pos3.ToString() + ":" + tex3.ToString();


                        int index = 0;
                        if (!elementMapping.TryGetValue(str1, out index))
                        {
                            pos.Add(pos1);
                            uvs.Add(tex1);
                            index = totalIndex++;
                            elementMapping.Add(str1, index);
                        }

                        inds.Add(index);


                        if (!elementMapping.TryGetValue(str2, out index))
                        {
                            pos.Add(pos2);
                            uvs.Add(tex2);
                            index = totalIndex++;
                            elementMapping.Add(str2, index);
                        }

                        inds.Add(index);


                        if (!elementMapping.TryGetValue(str3, out index))
                        {
                            pos.Add(pos3);
                            uvs.Add(tex3);
                            index = totalIndex++;
                            elementMapping.Add(str3, index);
                        }

                        inds.Add(index);

       

                    }
                }


            }



            vertices = new VBO<Vector3>(pos.ToArray());
            texCoords=  new VBO<Vector2>(uvs.ToArray());

            indices = new VBO<int>(inds.ToArray(), BufferTarget.ElementArrayBuffer);




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
              Gl.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            shader.useProgram();
            shader.setTime(time);
            shader.setProjectionMatrix(projectionMatrix);
            shader.setViewMatrix(viewMatrix);
            shader.setModelMatrix(( rotationX * rotationY* rotationZ) * scale * translation);

            Gl.BindBuffer(vertices);
            
            Gl.VertexAttribPointer(shader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);
            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(shader.vertexTexCoordIndex, 2, vertices.PointerType, false, 8, IntPtr.Zero);

            Gl.BindBuffer(indices);


            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

           Gl.BindTexture(TextureTarget.Texture2D, 0);

        }
    }
}