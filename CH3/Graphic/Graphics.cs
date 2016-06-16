using System;
using System.Collections.Generic;
using OpenGL;
using Tao.FreeGlut;

using CH3.Camera;

namespace CH3
{
    public class Graphics
    {



        public BasicShaderProgram basicShader { get; private set; }
        public CelShader celShader { get; private set; }
        public NormalShader normalShader { get; private set; }
        public DepthShader depthShader { get; private set; }
        public ModelShader modelShader { get; private set; }
        public EdgeShader edgeShader { get; private set; }
        public SimpleEdgeShader simpleEdgeShader { get; private set; }
        public FXAAShader fxAAShader { get; private set; }



        public World world { get; set; }


        public enum CameraMode
        {
            FPS, PLAYER
        }

        public enum RenderMode
        {
            BASIC, NORMAL, CEL, DEPTH, EDGE, SIMPLE_EDGE, MODEL, FXAA
        }

        public enum AAMode
        {
            OFF, MSAA_X2, MSAA_X4, MSAA_X8, FXAA
        }

        public enum ContourMode
        {
            OFF, ON, MSAA
        }




        public CameraMode cameraMode { get; set; }
        private AAMode aaMode;
        private ContourMode contourMode;


        public DirectionalLight light { get; private set; }



        public FPSCamera fpsCamera { get; private set; }
        public AboveCamera aboveCamera { get; private set; }


        private PostProcessedImage contourTexture;
        private PostProcessedImage MSAAContourTexture;


        private PostProcessedImage multiSampledFinalImage;
        private PostProcessedImage finalImage;






        private double fps;
        private int frame = 0, timebase = 0;






        public Graphics(World world, AAMode aaMode, ContourMode contourMode)
        {
            this.world = world;

            this.aaMode = aaMode;
            this.contourMode = contourMode;

            initShaders();

            initLight();
            initCameras();


            initFinalImage();
            initMultiSampledFinalImage();
            initContourTexture();
            initMultiSampledContourTexture();


        }



        public void Render(RenderMode renderMode, AAMode aaMode, ContourMode contourMode) {

            frame++;
            int time = Glut.glutGet(Glut.GLUT_ELAPSED_TIME);

            if (time - timebase > 1000)
            {
                fps = frame * 1000.0 / (time - timebase);
                timebase = time;
                frame = 0;
                Console.WriteLine(Gl.GetError());

                Console.WriteLine(Math.Round(fps) + "FPS");
                Console.WriteLine();


            }


            if (aaMode != this.aaMode) {
                this.aaMode = aaMode;
                if(aaMode != AAMode.FXAA && aaMode != AAMode.OFF) { 
                    initMultiSampledFinalImage();
                    initMultiSampledContourTexture();
                }
            }

            this.contourMode = contourMode;


            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.Multisample);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);
            Gl.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            renderContourTexture(time, world.allObjects);


            renderFinalImage(time, renderMode);

             //   renderSimpleContours(time, 0);
            
 


         //   renderHUD(time);

            Glut.glutSwapBuffers();

        }


        private void renderStaticObjects(int time, RenderMode renderMode) {
            renderObjects(time, renderMode, world.staticObjects, true);
        }


        private void renderDynamicObjects(int time, RenderMode renderMode) {
            renderObjects(time, renderMode, world.dynamicObjects, true);
        }

        private void renderSimpleContours(int time) {

            if (contourMode == ContourMode.OFF)
                return;
            else {
                this.contourTexture.Render(RenderMode.SIMPLE_EDGE);

            }

        }





        private void renderHUD(int time) {

        }

        private void renderSky(int time) {
            Gl.Disable(EnableCap.CullFace);
            renderObject(time, RenderMode.BASIC, world.sky, true);
            Gl.Enable(EnableCap.CullFace);

        }


        private void renderObject(int time, RenderMode renderMode, GameObject obj, bool mipmap)
        {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Window.WIDTH / (float)Window.HEIGHT), 0.1f, 500000f);
            Matrix4 viewMatrix = fpsCamera.viewMatrix;

            if (cameraMode == CameraMode.FPS)
            {
                viewMatrix = fpsCamera.viewMatrix;
            }
            else
            {
                viewMatrix = aboveCamera.viewMatrix;
            }

            //Render drawable

                if (obj != null)
                {
                    obj.Render(time, projectionMatrix, viewMatrix, light, renderMode, mipmap, false);
                }
            
        }


        private void renderObjects(int time, RenderMode renderMode, List<GameObject> objects, bool mipmap)
        {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Window.WIDTH / (float)Window.HEIGHT), 0.1f, 500000f);
            Matrix4 viewMatrix = fpsCamera.viewMatrix;

            if (cameraMode == CameraMode.FPS)
            {
                viewMatrix = fpsCamera.viewMatrix;
            }
            else
            {
               viewMatrix = aboveCamera.viewMatrix;
            }

            //Render drawable
            foreach (Drawable d in objects)
            {
                if (d != null) {
                    d.Render(time, projectionMatrix, viewMatrix, light, renderMode, mipmap, false);

                }
            }

        }


        private void renderContourTexture(int time, List<GameObject> objects)
        {
            if (aaMode != AAMode.OFF && aaMode != AAMode.FXAA && contourMode == ContourMode.MSAA)
            {
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, MSAAContourTexture.FBO);
                Gl.Enable(EnableCap.Multisample);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


                renderObjects(time, RenderMode.MODEL, objects, false);


                Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.MSAAContourTexture.FBO);
                Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.contourTexture.FBO);

                Gl.BlitFramebuffer(0, 0, Window.WIDTH, Window.HEIGHT, 0, 0, Window.WIDTH, Window.HEIGHT, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);


            }
            else if(contourMode == ContourMode.ON)
            {
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, contourTexture.FBO);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                renderObjects(time, RenderMode.MODEL, objects, false);

            }


            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }


        
        private void renderFinalImage(int time, RenderMode renderMode)
        {
            uint fbo = finalImage.FBO;
            if (aaMode != AAMode.OFF && aaMode != AAMode.FXAA) { 
                fbo = multiSampledFinalImage.FBO;
                Gl.Enable(EnableCap.Multisample);

            }
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            renderSky(time);
            renderStaticObjects(time, renderMode);
            renderDynamicObjects(time, renderMode);

            renderSimpleContours(time);



            if (aaMode == AAMode.FXAA) {
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                finalImage.Render(RenderMode.FXAA);

            } else { 
                Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, fbo);
                Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

                Gl.BlitFramebuffer(0, 0, Window.WIDTH, Window.HEIGHT, 0, 0, Window.WIDTH, Window.HEIGHT, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            }


        }



        private void initLight()
        {
            light = new DirectionalLight(new Vector3(0, 1, -1).Normalize());

        }

        private void initShaders()
        {

            basicShader = new BasicShaderProgram();
            basicShader.initShader();

            celShader = new CelShader();
            celShader.initShader();

            normalShader = new NormalShader();
            normalShader.initShader();

            depthShader = new DepthShader();
            depthShader.initShader();

            modelShader = new ModelShader();
            modelShader.initShader();

            edgeShader = new EdgeShader();
            edgeShader.initShader();

            simpleEdgeShader = new SimpleEdgeShader();
            simpleEdgeShader.initShader();

            fxAAShader = new FXAAShader();
            fxAAShader.initShader();

        }

        private void initCameras()
        {
            cameraMode = CameraMode.FPS;


            fpsCamera = new FPSCamera(new Vector3(0, 0, 10), new Vector3(0, 0, 0));
            aboveCamera = new AboveCamera();
            aboveCamera.height = 100;

        }

        private bool initFinalImage()
        {

            
            uint modelTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, modelTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);



            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);
            Gl.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, modelTex, 0);


            // The depth buffer
            uint depthrenderbuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthrenderbuffer);




            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
                Console.WriteLine("FAILED");
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.finalImage = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return true;

        }

        private bool initMultiSampledFinalImage()
        {

            if (this.multiSampledFinalImage != null) {
                uint[] list = { this.multiSampledFinalImage.FBO };
                Gl.DeleteFramebuffers(1, list);
            }

            bool multiSampling = false;
            int numberOfSamples = 0;

            if (aaMode == AAMode.MSAA_X2)
            {
                multiSampling = true;
                numberOfSamples = 2;
            }
            else if (aaMode == AAMode.MSAA_X4)
            {
                multiSampling = true;
                numberOfSamples = 4;
            }
            else if (aaMode == AAMode.MSAA_X8)
            {
                multiSampling = true;
                numberOfSamples = 8;
            }

            if (!multiSampling)
                return false;

            // Create multisample texture

            Gl.Enable(EnableCap.Multisample);

            uint modelTex = Gl.GenTexture();

            uint modelMultiSampledTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2DMultisample, modelMultiSampledTex);

            Gl.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, numberOfSamples, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, true);

            // Create and bind the FBO
            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);

            // Create color render buffer
            uint colorBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, colorBuffer);
            Gl.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Rgba8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, RenderbufferTarget.RenderbufferExt, colorBuffer);



            uint depthBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);


            // Bind Texture assuming we have created a texture
            Gl.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, modelTex, 0);


            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {

                Console.WriteLine("FAILED TO CREATE FRAME BUFFER");

                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUndefined)
                {
                    Console.WriteLine("UNDEFINED");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUnsupported)
                {
                    Console.WriteLine("UNSUPPORTED");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteReadBuffer)
                {
                    Console.WriteLine("INCOMPLETE READ BUFFER");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMultisample)
                {
                    Console.WriteLine("INCOMPLETE MULTISAMPLE");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMissingAttachment)
                {
                    Console.WriteLine("INCOMPLETE MISSING ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteAttachment)
                {
                    Console.WriteLine("INCOMPLETE ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteDrawBuffer)
                {
                    Console.WriteLine("INCOMPLETE DRAW BUFFER");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerCount)
                {
                    Console.WriteLine("INCOMPLETE LAYER  COUNT");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerTargets)
                {
                    Console.WriteLine("INCOMPLETE LAYER TARGETS");
                }



                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.multiSampledFinalImage = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);

            return true;

        }

        private bool initMultiSampledContourTexture()
        {


            if (this.MSAAContourTexture != null)
            {
                uint[] list = { this.MSAAContourTexture.FBO };
                Gl.DeleteFramebuffers(1, list);
            }

            bool multiSampling = false;
            int numberOfSamples = 0;

            if (aaMode == AAMode.MSAA_X2)
            {
                multiSampling = true;
                numberOfSamples = 2;
            }
            else if (aaMode == AAMode.MSAA_X4)
            {
                multiSampling = true;
                numberOfSamples = 4;
            }
            else if (aaMode ==AAMode.MSAA_X8)
            {
                multiSampling = true;
                numberOfSamples = 8;
            }

            if (!multiSampling)
                return false;

            // Create multisample texture

            Gl.Enable(EnableCap.Multisample);

            uint modelTex = Gl.GenTexture();

            uint modelMultiSampledTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2DMultisample, modelMultiSampledTex);

            Gl.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, numberOfSamples, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, true);

            // Create and bind the FBO
            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);

            // Create color render buffer
            uint colorBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, colorBuffer);
            Gl.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Rgba8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, RenderbufferTarget.RenderbufferExt, colorBuffer);



            uint depthBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, numberOfSamples, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);


            // Bind Texture assuming we have created a texture
            Gl.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, modelTex, 0);


            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {

                Console.WriteLine("FAILED TO CREATE FRAME BUFFER");

                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUndefined)
                {
                    Console.WriteLine("UNDEFINED");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferUnsupported)
                {
                    Console.WriteLine("UNSUPPORTED");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteReadBuffer)
                {
                    Console.WriteLine("INCOMPLETE READ BUFFER");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMultisample)
                {
                    Console.WriteLine("INCOMPLETE MULTISAMPLE");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteMissingAttachment)
                {
                    Console.WriteLine("INCOMPLETE MISSING ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteAttachment)
                {
                    Console.WriteLine("INCOMPLETE ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteDrawBuffer)
                {
                    Console.WriteLine("INCOMPLETE DRAW BUFFER");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerCount)
                {
                    Console.WriteLine("INCOMPLETE LAYER  COUNT");
                }
                if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferIncompleteLayerTargets)
                {
                    Console.WriteLine("INCOMPLETE LAYER TARGETS");
                }



                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.MSAAContourTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);

            return true;


        }


        private bool initContourTexture()
        {


            uint modelTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, modelTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);



            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.FramebufferExt, modelFBO);
            Gl.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, modelTex, 0);


            // The depth buffer
            uint depthrenderbuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.StencilAttachmentExt, RenderbufferTarget.RenderbufferExt, depthrenderbuffer);


            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;

            this.contourTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);
            return true;
              

        }






    }
}