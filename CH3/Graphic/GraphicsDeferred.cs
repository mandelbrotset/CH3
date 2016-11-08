using System;
using System.Collections.Generic;
using OpenGL;
using CH3.Camera;

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

        public DirectionalLight light { get; private set; }

        private Window window;

        private PostProcessedImage finalImage;

        private SFML.System.Clock clock;


        private double fps, timebase = 0;
        private int frame = 0;
        private DrawQuad quad;

        private int TMP = 0;

        public GraphicsDeferred(World world, AAMode aaMode, ContourMode contourMode, Window window)
        {
            this.world = world;
            clock = new SFML.System.Clock();
            this.aaMode = aaMode;
            this.contourMode = contourMode;
            this.window = window;

            quad = new DrawQuad();
            initShaders();

            initLight();
            initCameras();

            initFinalImage();
            initGBuffer();

        }

        private void renderHUD(int time)
        {

        }

        private void renderObject(int time, RenderMode renderMode, GameObject obj, bool mipmap)
        {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);


            Matrix4 projectionMatrix;
            Matrix4 viewMatrix = fpsCamera.viewMatrix;


            if (cameraMode == CameraMode.FPS)
            {
                viewMatrix = fpsCamera.viewMatrix;
                projectionMatrix = fpsCamera.perspectiveMatrix;
            }
            else
            {
                viewMatrix = aboveCamera.viewMatrix;
                projectionMatrix = aboveCamera.perspectiveMatrix;
            }

            //Render drawable

            if (obj != null)
            {
                //obj.Render(time, projectionMatrix, viewMatrix, light, renderMode, mipmap, false, obj);
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
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

           gbufferShader.useProgram();

          //  Gl.BindFramebuffer(OpenGL.FramebufferTarget.DrawFramebuffer, GBO);

           // Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Window.WIDTH / (float)Window.HEIGHT), 0.1f, 500000f);
            Matrix4 projectionMatrix;
            Matrix4 viewMatrix = fpsCamera.viewMatrix;

            if (cameraMode == CameraMode.FPS)
            {
                viewMatrix = fpsCamera.viewMatrix;
                projectionMatrix = fpsCamera.perspectiveMatrix;
            }
            else
            {
                viewMatrix = aboveCamera.viewMatrix;
                projectionMatrix = aboveCamera.perspectiveMatrix;
            }

            Matrix4 VPMatrix = projectionMatrix * viewMatrix;
            gbufferShader.setProjectionMatrix(projectionMatrix);
            gbufferShader.setViewMatrix(viewMatrix);
            gbufferShader.setNormal(projectionMatrix * viewMatrix);
            gbufferShader.setLightDirection(light.direction);
            
            foreach (GameObject d in objects)
            {
                if (d != null)
                {
                   // d.Render(0, projectionMatrix, viewMatrix, light, RenderMode.GBuffer, false, false);
                  d.Render(gbufferShader, VPMatrix,d.position);
                }
            }
        }



        private void renderFinalImage(int time, RenderMode renderMode)
        {
            uint fbo = finalImage.FBO;

            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, fbo);
            Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);

            renderGeometry(world.allObjects);

            if (aaMode == AAMode.FXAA)
            {
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

                finalImage.Render(RenderMode.FXAA);

            }
            else {
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.ReadFramebuffer, fbo);
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.DrawFramebuffer, 0);

                Gl.BlitFramebuffer(0, 0, Window.WIDTH, Window.HEIGHT, 0, 0, Window.WIDTH, Window.HEIGHT, OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit, OpenGL.BlitFramebufferFilter.Nearest);
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

            }
        }



        private void initLight()
        {
            light = new DirectionalLight(new Vector3(0, 1, -1).Normalize());
        }

        private bool initFinalImage()
        {
            uint modelTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2D, modelTex);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMagFilter, TextureParameter.Linear);

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
            Gl.TexImage2D(OpenGL.TextureTarget.Texture2D, 0, OpenGL.PixelInternalFormat.Rgb10A2, Window.WIDTH, Window.HEIGHT, 0, OpenGL.PixelFormat.Rgba, OpenGL.PixelType.UnsignedByte, IntPtr.Zero);
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
            //quad.render();
            renderFinalImage((int)0, renderMode);


            window.SwapBuffers(); // throw new NotImplementedException();
        }
    }
}