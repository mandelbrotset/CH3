using System;
using System.Collections.Generic;
using OpenGL;
using CH3.Camera;
using CH3.GameObjects.DynamicObjects.Vehicles;
using CH3.Utils;

namespace CH3
{
    class GraphicsDeferred : Graphics
    {

        public uint colorTex    { get; private set; }
        public uint normalTex   { get; private set; }
        public uint specularTex { get; private set; }
        public uint depthTex    { get; private set; }
        public uint GBO         { get; private set; }

        public World world { get; set; }

        private Window window;

        private PostProcessedImage finalImage;

        private SFML.System.Clock clock;

        public GraphicsDeferred(World world, AAMode aaMode, ContourMode contourMode, Window window)
        {
            this.world = world;
            clock = new SFML.System.Clock();
            this.aaMode = aaMode;
            this.contourMode = contourMode;
            this.window = window;
            initShaders();
            
            initCameras();

            initFinalImage();
            initGBuffer();

        }

        private void renderHUD(int time)
        {

        }

        private void renderObject(int time, RenderMode renderMode, GameObject obj, bool mipmap)
        {

            Matrix4 projectionMatrix;
            Matrix4 viewMatrix = fpsCamera.viewMatrix;


            if (cameraMode == CameraMode.FPS)
            {
                viewMatrix = fpsCamera.viewMatrix;
                projectionMatrix = fpsCamera.projection;
            }
            else
            {
                viewMatrix = aboveCamera.viewMatrix;
                projectionMatrix = aboveCamera.projection;
            }

            //Render drawable

            if (obj != null)
            {
               // obj.Render(time, projectionMatrix, viewMatrix, renderMode, mipmap, obj);
            }

        }

        private void renderSky(int time)
        {
            Gl.Disable(OpenGL.EnableCap.CullFace);
            renderObject(time, RenderMode.BASIC, world.sky, true);
            Gl.Enable(OpenGL.EnableCap.CullFace);

        }

        private void renderGeometry(List<GameObject> objects)
        {
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.DrawFramebuffer, GBO);
            Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);

            Matrix4 projectionMatrix = activeCamera.projection;
            Matrix4 viewMatrix = activeCamera.viewMatrix;
            Matrix4 VPMatrix = viewMatrix * projectionMatrix, VMatrix = viewMatrix;
            
            gbufferShader.useProgram();
            
            foreach (GameObject d in objects)
            {
                if (d != null)
                {
                    if (d is Car)
                    {
                        Car car = (Car)d;
                        OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(gbufferShader.program.ProgramID, "material_shininess"), 1.0f);
                        car.Render(gbufferShader, VPMatrix, VMatrix, d);
                        OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(gbufferShader.program.ProgramID, "material_shininess"), 0f);
                        car.wheel.Render(gbufferShader, car.vehicle.GetWheelTransformWS(0));
                        car.wheel.Render(gbufferShader, car.vehicle.GetWheelTransformWS(1));
                        car.wheel.Render(gbufferShader, car.vehicle.GetWheelTransformWS(2));
                        car.wheel.Render(gbufferShader, car.vehicle.GetWheelTransformWS(3));
                    } else {
                        OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(gbufferShader.program.ProgramID, "material_shininess"), 0f);
                        d.Render(gbufferShader, VPMatrix, VMatrix, d);
                    }
                }
            }

            Gl.BindFramebuffer(OpenGL.FramebufferTarget.ReadFramebuffer, GBO);
        }

        private void renderAABB(List<GameObject> objects)
        {
            primitiveShader.useProgram();
            primitiveShader.setProjectionMatrix(activeCamera.projection);
            primitiveShader.setViewMatrix(activeCamera.viewMatrix);
            foreach (GameObject d in objects)
            {
                if (d != null && d.has_physics)
                {
                    if (d is Car)
                        primitiveShader.setColor(new Vector4(1, 1, 0, 1));
                    else if (d.body.IsActive)
                        primitiveShader.setColor(new Vector4(0, 1, 0, 1));
                    else
                        primitiveShader.setColor(new Vector4(1, 0, 0, 1));

                    OpenTK.Vector3 min, max;
                    d.body.GetAabb(out min, out max);
                    Utils.DrawBox.Render(primitiveShader, min, max);
                }
            }
        }
        private void addFXAA()
        {
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);
            Gl.Clear(OpenGL.ClearBufferMask.DepthBufferBit);
            Gl.Enable(EnableCap.Texture2D);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, colorTex);
            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2D, normalTex);

            fxAAShader.useProgram();
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(fxAAShader.program.ProgramID, "tex"), 0);
            OpenTK.Graphics.OpenGL.GL.Uniform2(Gl.GetUniformLocation(fxAAShader.program.ProgramID, "screenSize"), (float)Window.WIDTH, (float)Window.HEIGHT);
            DrawQuad.Render(fxAAShader);
        }

        public OpenTK.Vector4 Mult(OpenTK.Vector4 v, OpenTK.Matrix4 m)
        {
            return new OpenTK.Vector4(
                m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z + m.M14 * v.W,
                m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z + m.M24 * v.W,
                m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z + m.M34 * v.W,
                m.M41 * v.X + m.M42 * v.Y + m.M43 * v.Z + m.M44 * v.W);
        }


        private void renderFinalImage(int time, RenderMode renderMode)
        {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

            renderGeometry(world.visibleObjects);

            /* BEGIN Lights */
            Gl.Disable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            //Gl.CullFace(CullFaceMode.Front);
            pointlightShader.useProgram();
            OpenTK.Graphics.OpenGL.GL.Uniform2(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "screenSize"), (float)Window.WIDTH, (float)Window.HEIGHT);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, colorTex);
            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2D, specularTex); // Specular
            Gl.ActiveTexture(TextureUnit.Texture2);
            Gl.BindTexture(TextureTarget.Texture2D, normalTex);
            Gl.ActiveTexture(TextureUnit.Texture3);
            Gl.BindTexture(TextureTarget.Texture2D, depthTex);
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "colorTexture"),     0);
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "specularTexture"),  1);
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "normalTexture"),    2);
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "depthTexture"),     3);
            OpenTK.Matrix4 invProj = OpenTK.Matrix4.Invert(activeCamera.projection_opentk);
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "inverseProjectionMatrix"), false, ref invProj);
            OpenTK.Matrix4 V_matrix = activeCamera.viewMatrix_opentk;
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "V_matrix"), false, ref V_matrix);


            OpenTK.Vector3 lightPosition = new OpenTK.Vector3(0, 0, 25);// Mult(new OpenTK.Vector4(0, 0, 100, 1), activeCamera.viewMatrix_opentk);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightPosition"), lightPosition);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightColor"), 0f,0f,1f);
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightRadius"), 100f);
            OpenTK.Matrix4 modelMatrix = OpenTK.Matrix4.CreateTranslation(lightPosition);
            OpenTK.Matrix4 MVP_matrix = modelMatrix * activeCamera.viewMatrix_opentk * activeCamera.projection_opentk;
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "MVP_matrix"), false, ref MVP_matrix);
            BackFaceBox.Render(pointlightShader, new OpenTK.Vector3(100, 100, 100));

            lightPosition = new OpenTK.Vector3(100, 0, 25);// Mult(new OpenTK.Vector4(0, 0, 100, 1), activeCamera.viewMatrix_opentk);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightPosition"), lightPosition);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightColor"), new OpenTK.Vector3(1,1,1));
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightRadius"), 100f);
            modelMatrix = OpenTK.Matrix4.CreateTranslation(lightPosition);
            MVP_matrix = modelMatrix * activeCamera.viewMatrix_opentk * activeCamera.projection_opentk;
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "MVP_matrix"), false, ref MVP_matrix);
            BackFaceBox.Render(pointlightShader, new OpenTK.Vector3(100, 100, 100));

            lightPosition = new OpenTK.Vector3(100, 100, 10);// Mult(new OpenTK.Vector4(0, 0, 100, 1), activeCamera.viewMatrix_opentk);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightPosition"), lightPosition);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightColor"), new OpenTK.Vector3(1, 1, 0));
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightRadius"), 100f);
            modelMatrix = OpenTK.Matrix4.CreateTranslation(lightPosition);
            MVP_matrix = modelMatrix * activeCamera.viewMatrix_opentk * activeCamera.projection_opentk;
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "MVP_matrix"), false, ref MVP_matrix);
            BackFaceBox.Render(pointlightShader, new OpenTK.Vector3(100, 100, 100));

            lightPosition = new OpenTK.Vector3(0, 100, 10);// Mult(new OpenTK.Vector4(0, 0, 100, 1), activeCamera.viewMatrix_opentk);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightPosition"), lightPosition);
            OpenTK.Graphics.OpenGL.GL.Uniform3(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightColor"), new OpenTK.Vector3(1, 0, 0));
            OpenTK.Graphics.OpenGL.GL.Uniform1(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "lightRadius"), 100f);
            modelMatrix = OpenTK.Matrix4.CreateTranslation(lightPosition);
            MVP_matrix = modelMatrix * activeCamera.viewMatrix_opentk * activeCamera.projection_opentk;
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(Gl.GetUniformLocation(pointlightShader.program.ProgramID, "MVP_matrix"), false, ref MVP_matrix);
            BackFaceBox.Render(pointlightShader, new OpenTK.Vector3(100, 100, 100));

            Gl.Enable(EnableCap.DepthTest);
            Gl.Disable(EnableCap.Blend);
            Gl.CullFace(CullFaceMode.Back);
            /* END Lights*/
            renderAABB(world.visibleObjects);
            addFXAA();

        }

        private bool initFinalImage()
        {
            uint modelTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2D, modelTex);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexImage2D(OpenGL.TextureTarget.Texture2D, 0, OpenGL.PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, 0, OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, IntPtr.Zero);

            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.FramebufferExt, modelFBO);


            Gl.FramebufferTexture2D(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.ColorAttachment0, OpenGL.TextureTarget.Texture2D, modelTex, 0);


            // The depth buffer
            uint depthrenderbuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(OpenGL.RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.RenderbufferStorage(OpenGL.RenderbufferTarget.Renderbuffer, OpenGL.RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.Framebuffer, OpenGL.FramebufferAttachment.DepthAttachment, OpenGL.RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.StencilAttachmentExt, OpenGL.RenderbufferTarget.RenderbufferExt, depthrenderbuffer);




            if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) != OpenGL.FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("FAILED");
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.finalImage = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

            return true;

        }

        private bool initGBuffer() {
            GBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, GBO);

            colorTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2D, colorTex);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexImage2D(OpenGL.TextureTarget.Texture2D, 0, OpenGL.PixelInternalFormat.Rgb10A2, Window.WIDTH, Window.HEIGHT, 0, OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, IntPtr.Zero);
            Gl.FramebufferTexture2D(OpenGL.FramebufferTarget.Framebuffer, OpenGL.FramebufferAttachment.ColorAttachment0, OpenGL.TextureTarget.Texture2D, colorTex, 0);

            normalTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2D, normalTex);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexImage2D(OpenGL.TextureTarget.Texture2D, 0, OpenGL.PixelInternalFormat.Rgb10A2, Window.WIDTH, Window.HEIGHT, 0, OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, IntPtr.Zero);
            Gl.FramebufferTexture2D(OpenGL.FramebufferTarget.Framebuffer, OpenGL.FramebufferAttachment.ColorAttachment1, OpenGL.TextureTarget.Texture2D, normalTex, 0);

            specularTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2D, specularTex);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexImage2D(OpenGL.TextureTarget.Texture2D, 0, OpenGL.PixelInternalFormat.R8, Window.WIDTH, Window.HEIGHT, 0, OpenGL.PixelFormat.Red, OpenGL.PixelType.UnsignedByte, IntPtr.Zero);
            Gl.FramebufferTexture2D(OpenGL.FramebufferTarget.Framebuffer, OpenGL.FramebufferAttachment.ColorAttachment2, OpenGL.TextureTarget.Texture2D, specularTex, 0);

            depthTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2D, depthTex);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureCompareMode, TextureParameter.None);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexImage2D(OpenGL.TextureTarget.Texture2D, 0, OpenGL.PixelInternalFormat.DepthComponent32, Window.WIDTH, Window.HEIGHT, 0, OpenGL.PixelFormat.DepthComponent, OpenGL.PixelType.UnsignedByte, IntPtr.Zero);
            Gl.FramebufferTexture2D(OpenGL.FramebufferTarget.Framebuffer, OpenGL.FramebufferAttachment.DepthAttachment, OpenGL.TextureTarget.Texture2D, depthTex, 0);

            OpenGL.DrawBuffersEnum[] drawbuffers = { OpenGL.DrawBuffersEnum.ColorAttachment0, OpenGL.DrawBuffersEnum.ColorAttachment1, OpenGL.DrawBuffersEnum.ColorAttachment2 };
            Gl.DrawBuffers(3, drawbuffers);

            if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) != OpenGL.FramebufferErrorCode.FramebufferComplete) {
                Console.WriteLine("FAILED");
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

                return false;
            }

            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);
            return true;

        }


        internal override void Render(RenderMode renderMode, AAMode aaMode, ContourMode contourMode)
        {

            Gl.Enable(OpenGL.EnableCap.Blend);
            Gl.BlendFunc(OpenGL.BlendingFactorSrc.SrcAlpha, OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
            Gl.Enable(OpenGL.EnableCap.DepthTest);
            Gl.Enable(OpenGL.EnableCap.CullFace);
            Gl.CullFace(OpenGL.CullFaceMode.Back);
            Gl.ClearColor(0.0f, 0.4f, 0.7f, 1.0f);
            Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);

            //
            

            
          //  Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);
            //Gl.ClearColor(1.0f, 1.0f, 0.0f, 1.0f);
           // Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);
         //   basicShader.useProgram();
           // quad.render();

            /*
            Gl.BindTexture(TextureTarget.Texture2D, colorTex);
            Gl.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb10A2, 0, 0, 512, 512, 0);
            */
            /*Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, colorTex);
            Gl.Uniform1i(Gl.GetUniformLocation(basicShader.program.ProgramID, "colorTex"), 0);
            */

           
            renderFinalImage((int)0, renderMode);


            window.SwapBuffers(); // throw new NotImplementedException();
        }
    }
}