using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;
using System.IO;
using static CH3.Graphics;
using CH3.Utils;
using CH3.GameObjects.DynamicObjects.Vehicles;
using CH3.Shaders;
using CH3.Lights;

namespace CH3
{
    public abstract class Drawable
    {
        protected VBO<Vector3> vertices;
        protected VBO<Vector3> normals;
        protected VBO<Vector2> texCoords;

        protected VBO<int> indices;

        protected Graphics graphics;

        public uint texture { get; protected set; }
        public Vector3 position { get; set; }
        //public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public float rotationX { get; set; }
        public float rotationY { get; set; }
        public float rotationZ { get; set; }

        public Vector3 offset;

        public OpenTK.Vector3 hext { get; private set; }

        private int modelId;

        public Drawable(Vector3 position, Vector3 scale, float rotationX, float rotationY, float rotationZ, Graphics graphics) {
            this.position = position;
            this.scale = scale;
            this.rotationX = rotationX;
            this.rotationY = rotationY;
            this.rotationZ = rotationZ;

            this.graphics = graphics;

            this.modelId = ModelIDGenerator.GetInstance().GetId();
        }



        protected void LoadModel(string modelFile, string textureFile, float texCoordsMultiplier, bool b_offset)
        {
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var fileStream = new FileStream(modelFile, FileMode.Open);

            LoadResult result = objLoader.Load(fileStream);

            fileStream.Close();

            this.setFaces(result.Groups, result.Vertices, result.Textures, result.Normals, texCoordsMultiplier, b_offset);

            texture = new OpenGL.Texture(textureFile).TextureID;

            Gl.BindTexture(TextureTarget.Texture2D, texture);
            Gl.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);

        }
        protected void LoadModel(string modelFile, string textureFile, float texCoordsMultiplier) {
            LoadModel(modelFile, textureFile, texCoordsMultiplier, true);
        }

        protected void LoadModel(Vector3[] vertices, Vector3[] normals, Vector2[] texCoords, int[] indices)
        {

            this.vertices = new VBO<Vector3>(vertices);
            this.texCoords = new VBO<Vector2>(texCoords);
            this.normals = new VBO<Vector3>(normals);

            this.indices = new VBO<int>(indices, BufferTarget.ElementArrayBuffer);

        }

        private void setFaces(IList<Group> groups, IList<Vertex> vs, IList<ObjLoader.Loader.Data.VertexData.Texture> tex, IList<Normal> ns, float texCoordsMultiplier, bool b_offset)
        {

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
                textures[j++] = new Vector2(uv.X * texCoordsMultiplier, uv.Y * texCoordsMultiplier);
            }

            j = 0;
            foreach (var n in ns)
            {
                normalArray[j++] = new Vector3(n.X, n.Y, n.Z);
            }

            int totalIndex = 0;
            int texCount = 0;
            int normalCount = 0;

            foreach (Group g in groups)
            {
                foreach (Face f in g.Faces)
                {
                    texCount += f.Count;
                    normalCount += f.Count;
                    FaceVertex v1 = f[0];


                    for (int i = 1; i < (f.Count - 1); i++)
                    {
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

            
            texCoords = new VBO<Vector2>(uvs.ToArray());
            normals = new VBO<Vector3>(nss.ToArray());

            indices = new VBO<int>(inds.ToArray(), BufferTarget.ElementArrayBuffer);

            Vector3 v_max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 v_min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

           
            foreach (Vector3 v in pos) {
                if (v.x < v_min.x) v_min.x = v.x;
                if (v.y < v_min.y) v_min.y = v.y;
                if (v.z < v_min.z) v_min.z = v.z;
                if (v.x > v_max.x) v_max.x = v.x;
                if (v.y > v_max.y) v_max.y = v.y;
                if (v.z > v_max.z) v_max.z = v.z;
            }
            
            offset = (v_max + v_min) * 0.5f;
            for (int i=0;i<pos.Count;++i)
            {
                if (b_offset)
                    pos[i] -= offset;
                pos[i] *= scale;
            }
            offset *= scale;
            if (b_offset)
            {
                // Flip y/z
                offset = new Vector3(offset.x, offset.z, offset.y);
            } else
            {
                offset = new Vector3(0, 0, 0);
            }

            vertices = new VBO<Vector3>(pos.ToArray());

            hext = new OpenTK.Vector3((v_max.x - v_min.x) * 0.5f, (v_max.y - v_min.y) * 0.5f, (v_max.z - v_min.z) * 0.5f) * 
                new OpenTK.Vector3(scale.x, scale.y, scale.z);

        }


        public void Render(ShaderLoader shader, OpenTK.Matrix4 VPMatrix, OpenTK.Matrix4 ModelMatrix) {

            OpenTK.Matrix4 modelViewProjectionMatrix = ModelMatrix * VPMatrix ;

            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program_id, "modelViewProjectionMatrix"), false, ref modelViewProjectionMatrix);
            OpenTK.Matrix4 m = OpenTK.Matrix4.Transpose(OpenTK.Matrix4.Invert(OpenTK.Matrix4.Mult(ModelMatrix, graphics.activeCamera.viewMatrix_opentk)));
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program_id, "normalMatrix"), false, ref m);

            OpenTK.Matrix4 m2 = OpenTK.Matrix4.Mult(ModelMatrix, graphics.activeCamera.viewMatrix_opentk);
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program_id, "modelviewMatrix"), false, ref m2);




            Gl.BindBuffer(vertices);
            Gl.VertexAttribPointer(shader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);
            Gl.BindBuffer(normals);
            Gl.VertexAttribPointer(shader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);
            Gl.Enable(EnableCap.Texture2D);
            Gl.BindTexture(TextureTarget.Texture2D, texture);
            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(shader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);
            Gl.BindBuffer(indices);
            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(TextureTarget.Texture2D, 0);
        }

 

        public void Render(BasicShaderProgram shader, bool mipmap, bool multiSampledTexture, GameObject obj) {
            if (obj.has_physics)
            {
                OpenTK.Matrix4 modelMatrix;
                obj.body.GetWorldTransform(out modelMatrix);
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program.ProgramID, "modelViewProjectionMatrix"), false, ref modelMatrix);

                OpenTK.Matrix4 m = OpenTK.Matrix4.Transpose(OpenTK.Matrix4.Invert(OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk)));
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program.ProgramID, "normalMatrix"), false, ref m);
            } else  {
                Matrix4 rotationZ = Matrix4.CreateRotation(Vector3.UnitZ, this.rotationZ);
                Matrix4 rotationX = Matrix4.CreateRotation(Vector3.UnitX, this.rotationX);
                Matrix4 rotationY = Matrix4.CreateRotation(Vector3.UnitY, this.rotationY);

                Matrix4 modelMatrix =  (rotationX * rotationY * rotationZ) * Matrix4.CreateTranslation(obj.position + offset);
                shader.setModelMatrix(modelMatrix);

                Matrix4 normalMatrix = modelMatrix * graphics.activeCamera.viewMatrix;
                normalMatrix = normalMatrix.Inverse();
                normalMatrix = normalMatrix.Transpose();
                shader.setRotationMatrix(normalMatrix);

                //shader.setRotationMatrix(rotationX * rotationY * rotationZ);
                /*
                OpenTK.Matrix4 rotation =
                    OpenTK.Matrix4.CreateRotationX(this.rotationX) *
                    OpenTK.Matrix4.CreateRotationZ(this.rotationZ) *
                    OpenTK.Matrix4.CreateRotationY(this.rotationY);


                OpenTK.Matrix4 modelMatrix =
                    rotation * OpenTK.Matrix4.CreateTranslation(obj.position.x + obj.offset.x, obj.position.y + obj.offset.y, obj.position.z + obj.offset.z);


                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program.ProgramID, "model_matrix"), false, ref modelMatrix);

                OpenTK.Matrix4 m = OpenTK.Matrix4.Transpose(OpenTK.Matrix4.Invert(OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk)));
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program.ProgramID, "rotation_matrix"), false, ref m);
                */
                
            }

            Gl.BindBuffer(vertices);
            Gl.VertexAttribPointer(shader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);

            Gl.BindBuffer(normals);
            Gl.VertexAttribPointer(shader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);

            Gl.Enable(EnableCap.Texture2D);

            Gl.BindTexture(TextureTarget.Texture2D, texture);

            if (mipmap)
            {
                Gl.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                //  Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.LinearMipMapNearest);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);
            }

            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(shader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);

            

            Gl.BindBuffer(indices);

            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

 
            Gl.BindTexture(TextureTarget.Texture2D, 0);

        }


        public void Render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix, DirectionalLight light, RenderMode renderMode, bool mipmap, bool multiSampledTexture, Vector3 position)
        {
            
            Matrix4 scale = Matrix4.CreateScaling(this.scale);
            Matrix4 rotationZ = Matrix4.CreateRotation(Vector3.UnitZ, this.rotationZ);
            Matrix4 rotationX = Matrix4.CreateRotation(Vector3.UnitX, this.rotationX);
            Matrix4 rotationY = Matrix4.CreateRotation(Vector3.UnitY, this.rotationY);

            BasicShaderProgram currentShader = graphics.celShader;
            if (renderMode == RenderMode.NORMAL)
                currentShader = graphics.normalShader;
            if (renderMode == RenderMode.BASIC)
                currentShader = graphics.basicShader;
            if (renderMode == RenderMode.DEPTH)
                currentShader = graphics.depthShader;
            if (renderMode == RenderMode.EDGE)
                currentShader = graphics.edgeShader;
            if (renderMode == RenderMode.SIMPLE_EDGE)
                currentShader = graphics.simpleEdgeShader;
            if (renderMode == RenderMode.MODEL)
                currentShader = graphics.modelShader;
            if (renderMode == RenderMode.FXAA)
                currentShader = graphics.fxAAShader;
            if (renderMode == RenderMode.GBuffer)
                currentShader = graphics.gbufferShader;

            currentShader.useProgram();
            currentShader.setProjectionMatrix(projectionMatrix);
            currentShader.setRotationMatrix(rotationX * rotationY * rotationZ);
            currentShader.setViewMatrix(viewMatrix);
            currentShader.setModelMatrix(Matrix4.Identity);

            if (renderMode == RenderMode.MODEL)
            {
                float id = ((float)modelId / 255);
                graphics.modelShader.setModelId(id);
            }
            else if (renderMode == RenderMode.FXAA)
            {
                OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(currentShader.program.ProgramID, "tex"), 0);
                OpenTK.Graphics.OpenGL.GL.Uniform2(Gl.GetUniformLocation(currentShader.program.ProgramID, "screenSize"), (float)Window.WIDTH, (float)Window.HEIGHT);
            }

                Gl.BindBuffer(vertices);
            Gl.VertexAttribPointer(currentShader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);

            Gl.BindBuffer(normals);
            Gl.VertexAttribPointer(currentShader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);

            currentShader.setLightDirection(new Vector3(light.direction.X, light.direction.Y, light.direction.Z));
            currentShader.setTime(time);


            Gl.Enable(EnableCap.Texture2D);

            Gl.BindTexture(TextureTarget.Texture2D, texture);

            if (mipmap)
            {
                Gl.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                //  Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.LinearMipMapNearest);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);
            }

            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(currentShader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);



            Gl.BindBuffer(indices);

            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);


            Gl.BindTexture(TextureTarget.Texture2D, 0);

        }



        public void Render(GBufferShader shader, OpenTK.Matrix4 modelMatrix)
        {
            OpenTK.Matrix4 MVP_matrix = modelMatrix * graphics.activeCamera.viewMatrix_opentk * graphics.activeCamera.projection_opentk;

            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program.ProgramID, "modelViewProjectionMatrix"), false, ref MVP_matrix);

            OpenTK.Matrix4 m = OpenTK.Matrix4.Transpose(OpenTK.Matrix4.Invert(OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk)));
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(shader.program.ProgramID, "normalMatrix"), false, ref m);

            Gl.BindBuffer(vertices);
            Gl.VertexAttribPointer(shader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);
            Gl.BindBuffer(normals);
            Gl.VertexAttribPointer(shader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);
            Gl.Enable(EnableCap.Texture2D);
            Gl.BindTexture(TextureTarget.Texture2D, texture);
            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(shader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);
            Gl.BindBuffer(indices);
            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindTexture(TextureTarget.Texture2D, 0);
        }



        public void Render(GBufferShader currentShader, Matrix4 VPMatrix, Matrix4 VMatrix, GameObject obj)
        {
            if (obj.has_physics)
            {
                OpenTK.Matrix4 modelMatrix;
                    obj.body.GetWorldTransform(out modelMatrix);
                OpenTK.Matrix4 MVP_matrix = modelMatrix * graphics.activeCamera.viewMatrix_opentk*  graphics.activeCamera.projection_opentk;

                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program.ProgramID, "modelViewProjectionMatrix"), false, ref MVP_matrix);

                OpenTK.Matrix4 m = OpenTK.Matrix4.Transpose(OpenTK.Matrix4.Invert(OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk)));
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program.ProgramID, "normalMatrix"), false, ref m);
                
            } else {
                Matrix4 rotationZ = Matrix4.CreateRotation(Vector3.UnitZ, this.rotationZ);
                Matrix4 rotationX = Matrix4.CreateRotation(Vector3.UnitX, this.rotationX);
                Matrix4 rotationY = Matrix4.CreateRotation(Vector3.UnitY, this.rotationY);

                Matrix4 translation = Matrix4.CreateTranslation(position);

                Matrix4 modelMatrix = (rotationX * rotationY * rotationZ)  * translation;

                currentShader.setRotationMatrix(rotationX * rotationY * rotationZ);

                currentShader.setNormal(modelMatrix * VMatrix);
                currentShader.setMVP(modelMatrix * VPMatrix);
            }



            Gl.BindBuffer(vertices);
            Gl.VertexAttribPointer(currentShader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);

            Gl.BindBuffer(normals);
            Gl.VertexAttribPointer(currentShader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);
           
            Gl.ActiveTexture(TextureUnit.Texture0);
           
            Gl.BindTexture(TextureTarget.Texture2D, texture);
           
            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(currentShader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);
            
            Gl.BindBuffer(indices);

            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }


        public void Render(ShaderLoader currentShader, Matrix4 VPMatrix, Matrix4 VMatrix, GameObject obj)
        {
            if (obj.has_physics)
            {
                OpenTK.Matrix4 modelMatrix;
                obj.body.GetWorldTransform(out modelMatrix);
                OpenTK.Matrix4 MVP_matrix = modelMatrix * graphics.activeCamera.viewMatrix_opentk * graphics.activeCamera.projection_opentk;

                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program_id, "modelViewProjectionMatrix"), false, ref MVP_matrix);

                OpenTK.Matrix4 m = OpenTK.Matrix4.Transpose(OpenTK.Matrix4.Invert(OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk)));
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program_id, "normalMatrix"), false, ref m);

                OpenTK.Matrix4 m2 = OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk);
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program_id, "modelviewMatrix"), false, ref m2);


            }
            else {
                // FEL
                /*
                Matrix4 rotationZ = Matrix4.CreateRotation(Vector3.UnitZ, this.rotationZ);
                Matrix4 rotationX = Matrix4.CreateRotation(Vector3.UnitX, this.rotationX);
                Matrix4 rotationY = Matrix4.CreateRotation(Vector3.UnitY, this.rotationY);

                Matrix4 translation = Matrix4.CreateTranslation(position);

                Matrix4 modelMatrix = (rotationX * rotationY * rotationZ) * translation;
                */
                /*
                currentShader.setRotationMatrix(rotationX * rotationY * rotationZ);

                currentShader.setNormal(modelMatrix * VMatrix);
                currentShader.setMVP(modelMatrix * VPMatrix);
                */


                OpenTK.Matrix4 modelMatrix = OpenTK.Matrix4.CreateTranslation(position.x, position.y, position.z);
                OpenTK.Matrix4 MVP_matrix = modelMatrix * graphics.activeCamera.viewMatrix_opentk * graphics.activeCamera.projection_opentk;

                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program_id, "modelViewProjectionMatrix"), false, ref MVP_matrix);

                OpenTK.Matrix4 m = OpenTK.Matrix4.Transpose(OpenTK.Matrix4.Invert(OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk)));
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program_id, "normalMatrix"), false, ref m);

                OpenTK.Matrix4 m2 = OpenTK.Matrix4.Mult(modelMatrix, graphics.activeCamera.viewMatrix_opentk);
                OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(currentShader.program_id, "modelviewMatrix"), false, ref m2);
            }

            Gl.BindBuffer(vertices);
            Gl.VertexAttribPointer(currentShader.vertexPositionIndex, 3, vertices.PointerType, false, 12, IntPtr.Zero);

            Gl.BindBuffer(normals);
            Gl.VertexAttribPointer(currentShader.vertexNormalIndex, 3, vertices.PointerType, true, 12, IntPtr.Zero);

            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.BindTexture(TextureTarget.Texture2D, texture);

            Gl.BindBuffer(texCoords);
            Gl.VertexAttribPointer(currentShader.vertexTexCoordIndex, 2, vertices.PointerType, true, 8, IntPtr.Zero);

            Gl.BindBuffer(indices);

            Gl.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}