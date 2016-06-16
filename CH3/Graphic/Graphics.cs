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



        public World world { get; set; }


        public enum CameraMode
        {
            FPS, PLAYER
        }

        public enum RenderMode
        {
            BASIC, NORMAL, CEL, DEPTH, EDGE, SIMPLE_EDGE, MODEL
        }

        public enum MSAAMode
        {
            OFF, MSAA_X2, MSAA_X4, MSAA_X8
        }

        public enum ContourMode
        {
            OFF, ON, MSAA
        }




        public CameraMode cameraMode { get; set; }
        private MSAAMode msaaMode;
        private ContourMode contourMode;


        public DirectionalLight light { get; private set; }



        public FPSCamera fpsCamera { get; private set; }
        public AboveCamera aboveCamera { get; private set; }


        private PostProcessedImage modelTexture;
        private PostProcessedImage modelMSTexture;

        private PostProcessedImage multiSampledFinalImage;
        private PostProcessedImage finalImage;






        private double fps;
        private int frame = 0, timebase = 0;






        public Graphics(World world, MSAAMode msaaMode, ContourMode contourMode)
        {
            this.world = world;

            this.msaaMode = msaaMode;
            this.contourMode = contourMode;

            initShaders();

            initLight();
            initCameras();


            initFinalImage();
            initMultiSampledFinalImage();
            initModelTexture();
            initMultiSampledModelTexture();


        }



        public void Render(RenderMode renderMode, MSAAMode msaaMode, ContourMode contourMode) {

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


            if (msaaMode != this.msaaMode) {
                this.msaaMode = msaaMode;
                initMultiSampledFinalImage();
                initMultiSampledModelTexture();
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


            renderModelTexture(time, world.allObjects);

            renderFinalImage(time, renderMode);
            
 
            renderSimpleContours(time);


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

            if(contourMode != ContourMode.OFF)
                this.modelTexture.Render(RenderMode.SIMPLE_EDGE);

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


        private void renderModelTexture(int time, List<GameObject> objects)
        {
            if (msaaMode != MSAAMode.OFF && contourMode == ContourMode.MSAA)
            {
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, modelMSTexture.FBO);
                Gl.Enable(EnableCap.Multisample);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


                renderObjects(time, RenderMode.MODEL, objects, false);


                Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.modelMSTexture.FBO);
                Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.modelTexture.FBO);

                Gl.BlitFramebuffer(0, 0, Window.WIDTH, Window.HEIGHT, 0, 0, Window.WIDTH, Window.HEIGHT, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);


            }
            else if(contourMode == ContourMode.ON)
            {
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, modelTexture.FBO);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                renderObjects(time, RenderMode.MODEL, objects, false);

            }


            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }


        
        private void renderFinalImage(int time, RenderMode renderMode)
        {
            uint fbo = finalImage.FBO;
            if (msaaMode != MSAAMode.OFF)
                fbo = multiSampledFinalImage.FBO;
            
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, multiSampledFinalImage.FBO);
            Gl.Enable(EnableCap.Multisample);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            renderSky(time);
            renderStaticObjects(time, renderMode);
            renderDynamicObjects(time, renderMode);


            Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.multiSampledFinalImage.FBO);
            Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            Gl.BlitFramebuffer(0, 0, Window.WIDTH, Window.HEIGHT, 0, 0, Window.WIDTH, Window.HEIGHT, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
            

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

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

            if (msaaMode == MSAAMode.MSAA_X2)
            {
                multiSampling = true;
                numberOfSamples = 2;
            }
            else if (msaaMode == MSAAMode.MSAA_X4)
            {
                multiSampling = true;
                numberOfSamples = 4;
            }
            else if (msaaMode == MSAAMode.MSAA_X8)
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

        private bool initMultiSampledModelTexture()
        {


            if (this.modelMSTexture != null)
            {
                uint[] list = { this.modelMSTexture.FBO };
                Gl.DeleteFramebuffers(1, list);
            }

            bool multiSampling = false;
            int numberOfSamples = 0;

            if (msaaMode == MSAAMode.MSAA_X2)
            {
                multiSampling = true;
                numberOfSamples = 2;
            }
            else if (msaaMode == MSAAMode.MSAA_X4)
            {
                multiSampling = true;
                numberOfSamples = 4;
            }
            else if (msaaMode == MSAAMode.MSAA_X8)
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

            this.modelMSTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);

            return true;


        }

        private bool initModelTexture()
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

            this.modelTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);
            return true;
              

        }






    }
}