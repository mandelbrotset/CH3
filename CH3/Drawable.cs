﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;
using System.IO;


namespace CH3
{
    public abstract class Drawable
    {

        private VBO<Vector3> vertices;
        private VBO<Vector3> normals;
        private VBO<Vector2> texCoords;

        private VBO<int> indices;



        public BasicShaderProgram shader { protected get; set; }
        public OpenGL.Texture texture { get; protected set; }
        public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public float rotationX { get; set; }
        public float rotationY { get; set; }
        public float rotationZ { get; set; }



        protected void LoadModel(string modelFile, string textureFile) {


            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new FileStream(modelFile, FileMode.Open);

            LoadResult result = objLoader.Load(fileStream);

            fileStream.Close();

            this.setFaces(result.Groups, result.Vertices, result.Textures);

            texture = new OpenGL.Texture(textureFile);
        }

        private void setFaces(IList<Group> groups, IList<Vertex> vs, IList<ObjLoader.Loader.Data.VertexData.Texture> tex, IList<Normal> ns) {

            Dictionary<string, int> elementMapping = new Dictionary<string, int>();
            List<Vector3> pos = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> nss = new List<Vector3>();

            List<int> inds = new List<int>();

            Vector3[] positions = new Vector3[vs.Count];
            Vector3[] normalArray = new Vector3[ns.Count];
            Vector2[] textures = new Vector2[tex.Count];

            


            Console.WriteLine("Number of Textures:" + tex.Count);

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

            j = 0;
            foreach (var n in ns)
            {
                normalArray[j++] = new Vector3(n.X, n.Y, n.Z);
            }

            int totalIndex = 0;
            int texCount = 0;
            int normalCount = 0;

            foreach (Group g in groups) {
                foreach (Face f in g.Faces) {
                    texCount += f.Count;
                    normalCount += f.Count;
                    FaceVertex v1 = f[0];

                    
                    for (int i = 1; i < (f.Count-1); i++) {
                        FaceVertex v2 = f[i];
                        FaceVertex v3 = f[i + 1];

                        Vector3 pos1 = positions[v1.VertexIndex - 1];
                        Vector3 pos2 = positions[v2.VertexIndex - 1];
                        Vector3 pos3 = positions[v3.VertexIndex - 1];

                        Vector2 tex1;
                        Vector2 tex2;
                        Vector2 tex3;

                        Vector3 normal1;
                        Vector3 normal2;
                        Vector3 normal3;


                        if (v1.TextureIndex < 0)
                        {
                            if (texCount + v1.TextureIndex > (textures.Length - 1))
                            {
                                Console.WriteLine("Getting texture index: " + texCount);
                                Console.WriteLine("Getting v1 index: " + v1.TextureIndex);


                            }

                            tex1 = textures[texCount + v1.TextureIndex];
                            tex2 = textures[texCount + v2.TextureIndex];
                            tex3 = textures[texCount + v3.TextureIndex];

                        }
                        else if (tex.Count > 0)
                        {

                            tex1 = textures[v1.TextureIndex - 1];
                            tex2 = textures[v2.TextureIndex - 1];
                            tex3 = textures[v3.TextureIndex - 1];
                        }
                        else
                        {

                            tex1 = new Vector2(0, 0);
                            tex2 = new Vector2(0, 0);
                            tex3 = new Vector2(0, 0);
                        }


                        if (v1.NormalIndex < 0)
                        {
                  
                            normal1 = normalArray[normalCount + v1.NormalIndex];
                            normal2 = normalArray[normalCount + v2.NormalIndex];
                            normal3 = normalArray[normalCount + v3.NormalIndex];

                        }
                        else if (ns.Count > 0)
                        {

                            normal1 = normalArray[v1.NormalIndex - 1];
                            normal2 = normalArray[v2.NormalIndex - 1];
                            normal3 = normalArray[v3.NormalIndex - 1];
                        }
                        else
                        {

                            normal1 = new Vector3(0, 0, 0);
                            normal2 = new Vector3(0, 0, 0);
                            normal3 = new Vector3(0, 0, 0);
                        }




                        string str1 = pos1.ToString() + ":" + tex1.ToString() + ":" + normal1.ToString();
                        string str2 = pos2.ToString() + ":" + tex2.ToString() + ":" + normal2.ToString();
                        string str3 = pos3.ToString() + ":" + tex3.ToString() + ":" + normal3.ToString();


                        int index = 0;
                        if (!elementMapping.TryGetValue(str1, out index))
                        {
                            pos.Add(pos1);
                            uvs.Add(tex1);
                            nss.Add(normal1);
                            index = totalIndex++;
                            elementMapping.Add(str1, index);
                        }

                        inds.Add(index);


                        if (!elementMapping.TryGetValue(str2, out index))
                        {
                            pos.Add(pos2);
                            uvs.Add(tex2);
                            nss.Add(normal2);

                            index = totalIndex++;
                            elementMapping.Add(str2, index);
                        }

                        inds.Add(index);


                        if (!elementMapping.TryGetValue(str3, out index))
                        {
                            pos.Add(pos3);
                            uvs.Add(tex3);
                            nss.Add(normal3);

                            index = totalIndex++;
                            elementMapping.Add(str3, index);
                        }

                        inds.Add(index);

       

                    }
                }


            }



            vertices = new VBO<Vector3>(pos.ToArray());
            texCoords=  new VBO<Vector2>(uvs.ToArray());
            normals = new VBO<Vector3>(nss.ToArray());

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

            Gl.BindBuffer(normals);
            Gl.VertexAttribPointer(shader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);

            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(shader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);

            Gl.BindBuffer(indices);


            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

           Gl.BindTexture(TextureTarget.Texture2D, 0);

        }
    }
}