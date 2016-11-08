using System;
using System.Collections.Generic;
using OpenGL;
using CH3.Camera;

namespace CH3
{
    public class GraphicsForward : Graphics
    {
        public World world { get; set; }

        public DirectionalLight light { get; private set; }
     
        private Window window;

        private PostProcessedImage contourTexture;
        private PostProcessedImage MSAAContourTexture;


        private PostProcessedImage multiSampledFinalImage;
        private PostProcessedImage finalImage;

        public GraphicsForward(World world, AAMode aaMode, ContourMode contourMode, Window window)
        {
            this.world = world; 
            this.aaMode = aaMode;
            this.contourMode = contourMode;

            this.window = window;

            initShaders();

            initLight();
            initCameras();
            
            initFinalImage();
            initMultiSampledFinalImage();
            initContourTexture();
            initMultiSampledContourTexture();

        }



        internal override void Render(RenderMode renderMode, AAMode aaMode, ContourMode contourMode) {

            /*
                int time = clock.ElapsedTime.AsMilliseconds();

                frame++;

                if (time - timebase > 1000)
                {
                    fps = frame * 1000.0 / (time - timebase);
                    timebase = time;
                    frame = 0;
                    Console.WriteLine(Gl.GetError());

                    Console.WriteLine(Math.Round(fps) + "FPS");
                    Console.WriteLine();
                    Console.WriteLine("Drawing " + world.visibleObjects.Count + " objects");

                }*/

            if (aaMode != this.aaMode) {
                this.aaMode = aaMode;
                if(aaMode != AAMode.FXAA && aaMode != AAMode.OFF) { 
                    initMultiSampledFinalImage();
                    initMultiSampledContourTexture();
                }
            }

            this.contourMode = contourMode;


            Gl.Enable(OpenGL.EnableCap.Blend);
            Gl.Enable(OpenGL.EnableCap.Multisample);
            Gl.BlendFunc(OpenGL.BlendingFactorSrc.SrcAlpha, OpenGL.BlendingFactorDest.OneMinusSrcAlpha);
            Gl.Enable(OpenGL.EnableCap.DepthTest);
            Gl.Enable(OpenGL.EnableCap.CullFace);
            Gl.CullFace(OpenGL.CullFaceMode.Back);
            Gl.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);




            renderContourTexture(/*(int)time*/0, world.visibleObjects);


            renderFinalImage(/*(int)time*/ 0, renderMode);


         //   renderHUD(time);
            window.SwapBuffers();
        }


        private void renderStaticObjects(int time, RenderMode renderMode) {
            renderObjects(time, renderMode, world.staticObjects, true);
        }


        private void renderDynamicObjects(int time, RenderMode renderMode) {
            renderObjects(time, renderMode, world.dynamicObjects, true);
        }

        private void renderVisibleObjects(int time, RenderMode renderMode)
        {
            renderObjects(time, renderMode, world.visibleObjects, true);
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
            Gl.Disable(OpenGL.EnableCap.CullFace);
            renderObject(time, RenderMode.BASIC, world.sky, true);
            Gl.Enable(OpenGL.EnableCap.CullFace);

        }


        private void renderObject(int time, RenderMode renderMode, GameObject obj, bool mipmap)
        {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

            BasicShaderProgram currentShader = celShader;
            if (renderMode == RenderMode.NORMAL)
                currentShader = normalShader;
            if (renderMode == RenderMode.BASIC)
                currentShader = basicShader;
            if (renderMode == RenderMode.DEPTH)
                currentShader = depthShader;
            if (renderMode == RenderMode.EDGE)
                currentShader = edgeShader;
            if (renderMode == RenderMode.SIMPLE_EDGE)
                currentShader = simpleEdgeShader;
            if (renderMode == RenderMode.MODEL)
                currentShader = modelShader;
            if (renderMode == RenderMode.FXAA)
                currentShader = fxAAShader;
            if (renderMode == RenderMode.GBuffer)
                currentShader = gbufferShader;

            currentShader.useProgram();
            currentShader.setProjectionMatrix(activeCamera.perspectiveMatrix);
            currentShader.setViewMatrix(activeCamera.viewMatrix);
            currentShader.setLightDirection(light.direction);
            currentShader.setTime(time);

            //Render drawable
            if (obj != null){
                    obj.Render(currentShader, mipmap, false, obj);
            }
            
        }


        private void renderObjects(int time, RenderMode renderMode, List<GameObject> objects, bool mipmap)
        {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

            BasicShaderProgram currentShader = celShader;
            if (renderMode == RenderMode.NORMAL)
                currentShader = normalShader;
            if (renderMode == RenderMode.BASIC)
                currentShader = basicShader;
            if (renderMode == RenderMode.DEPTH)
                currentShader = depthShader;
            if (renderMode == RenderMode.EDGE)
                currentShader = edgeShader;
            if (renderMode == RenderMode.SIMPLE_EDGE)
                currentShader = simpleEdgeShader;
            if (renderMode == RenderMode.MODEL)
                currentShader = modelShader;
            if (renderMode == RenderMode.FXAA)
                currentShader = fxAAShader;
            if (renderMode == RenderMode.GBuffer)
                currentShader = gbufferShader;

            currentShader.useProgram();
            currentShader.setProjectionMatrix(activeCamera.perspectiveMatrix);
            currentShader.setViewMatrix(activeCamera.viewMatrix);
            currentShader.setLightDirection(light.direction);
            currentShader.setTime(time);

            /*
            if (renderMode == RenderMode.MODEL)
            {
                float id = ((float)modelId / 255);
                graphics.modelShader.setModelId(id);
            }
            */

            //Render drawable
            foreach (GameObject d in objects)
            {
                if (d != null) {
                    d.Render(currentShader, mipmap, false, d);
                }
            }

            //Render AABB
            primitiveShader.useProgram();
            primitiveShader.setProjectionMatrix(activeCamera.perspectiveMatrix);
            primitiveShader.setViewMatrix(activeCamera.viewMatrix);
            foreach (GameObject d in objects)
            {
                if (d != null && d.has_physics)
                {
                    if (d.body.IsActive)
                        primitiveShader.setColor(new Vector3(0, 1, 0));
                    else
                        primitiveShader.setColor(new Vector3(1, 0, 0));

                    OpenTK.Vector3 min, max;
                    d.body.GetAabb(out min, out max);
                    Utils.DrawBox.Render(primitiveShader, min, max);
                }
            }
            
        }


        private void renderContourTexture(int time, List<GameObject> objects)
        {
            if (aaMode != AAMode.OFF && aaMode != AAMode.FXAA && contourMode == ContourMode.MSAA)
            {
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, MSAAContourTexture.FBO);
                Gl.Enable(OpenGL.EnableCap.Multisample);
                Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);


                renderObjects(time, RenderMode.MODEL, objects, false);


                Gl.BindFramebuffer(OpenGL.FramebufferTarget.ReadFramebuffer, this.MSAAContourTexture.FBO);
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.DrawFramebuffer, this.contourTexture.FBO);

                Gl.BlitFramebuffer(0, 0, Window.WIDTH, Window.HEIGHT, 0, 0, Window.WIDTH, Window.HEIGHT, OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit, OpenGL.BlitFramebufferFilter.Nearest);


            }
            else if(contourMode == ContourMode.ON)
            {
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, contourTexture.FBO);
                Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);
                renderObjects(time, RenderMode.MODEL, objects, false);

            }


            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

        }


        
        private void renderFinalImage(int time, RenderMode renderMode)
        {
            uint fbo = finalImage.FBO;
            if (aaMode != AAMode.OFF && aaMode != AAMode.FXAA) { 
                fbo = multiSampledFinalImage.FBO;
                Gl.Enable(OpenGL.EnableCap.Multisample);

            }
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, fbo);
            Gl.Clear(OpenGL.ClearBufferMask.ColorBufferBit | OpenGL.ClearBufferMask.DepthBufferBit);

            renderSky(time);
            //renderStaticObjects(time, renderMode);
            //renderDynamicObjects(time, renderMode);
            renderVisibleObjects(time, renderMode);

            renderSimpleContours(time);

            if (aaMode == AAMode.FXAA) {
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

                finalImage.Render(RenderMode.FXAA);

            } else { 
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




            if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) != OpenGL.FramebufferErrorCode.FramebufferComplete) {
                Console.WriteLine("FAILED");
                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.finalImage = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

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

            Gl.Enable(OpenGL.EnableCap.Multisample);

            uint modelTex = Gl.GenTexture();

            uint modelMultiSampledTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2DMultisample, modelMultiSampledTex);

            Gl.TexImage2DMultisample(OpenGL.TextureTargetMultisample.Texture2DMultisample, numberOfSamples, OpenGL.PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, true);

            // Create and bind the FBO
            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.FramebufferExt, modelFBO);

            // Create color render buffer
            uint colorBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(OpenGL.RenderbufferTarget.RenderbufferExt, colorBuffer);
            Gl.RenderbufferStorageMultisample(OpenGL.RenderbufferTarget.RenderbufferExt, numberOfSamples, OpenGL.RenderbufferStorage.Rgba8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.ColorAttachment0Ext, OpenGL.RenderbufferTarget.RenderbufferExt, colorBuffer);



            uint depthBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(OpenGL.RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.RenderbufferStorageMultisample(OpenGL.RenderbufferTarget.RenderbufferExt, numberOfSamples, OpenGL.RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.DepthAttachmentExt, OpenGL.RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.StencilAttachmentExt, OpenGL.RenderbufferTarget.RenderbufferExt, depthBuffer);


            // Bind Texture assuming we have created a texture
            Gl.FramebufferTexture2D(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.ColorAttachment0Ext, OpenGL.TextureTarget.Texture2D, modelTex, 0);


            if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) != OpenGL.FramebufferErrorCode.FramebufferComplete)
            {

                Console.WriteLine("FAILED TO CREATE FRAME BUFFER");

                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferUndefined)
                {
                    Console.WriteLine("UNDEFINED");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferUnsupported)
                {
                    Console.WriteLine("UNSUPPORTED");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteReadBuffer)
                {
                    Console.WriteLine("INCOMPLETE READ BUFFER");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteMultisample)
                {
                    Console.WriteLine("INCOMPLETE MULTISAMPLE");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteMissingAttachment)
                {
                    Console.WriteLine("INCOMPLETE MISSING ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteAttachment)
                {
                    Console.WriteLine("INCOMPLETE ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteDrawBuffer)
                {
                    Console.WriteLine("INCOMPLETE DRAW BUFFER");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteLayerCount)
                {
                    Console.WriteLine("INCOMPLETE LAYER  COUNT");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteLayerTargets)
                {
                    Console.WriteLine("INCOMPLETE LAYER TARGETS");
                }



                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

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

            Gl.Enable(OpenGL.EnableCap.Multisample);

            uint modelTex = Gl.GenTexture();

            uint modelMultiSampledTex = Gl.GenTexture();
            Gl.BindTexture(OpenGL.TextureTarget.Texture2DMultisample, modelMultiSampledTex);

            Gl.TexImage2DMultisample(OpenGL.TextureTargetMultisample.Texture2DMultisample, numberOfSamples, OpenGL.PixelInternalFormat.Rgba, Window.WIDTH, Window.HEIGHT, true);

            // Create and bind the FBO
            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(OpenGL.FramebufferTarget.FramebufferExt, modelFBO);

            // Create color render buffer
            uint colorBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(OpenGL.RenderbufferTarget.RenderbufferExt, colorBuffer);
            Gl.RenderbufferStorageMultisample(OpenGL.RenderbufferTarget.RenderbufferExt, numberOfSamples, OpenGL.RenderbufferStorage.Rgba8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.ColorAttachment0Ext, OpenGL.RenderbufferTarget.RenderbufferExt, colorBuffer);



            uint depthBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(OpenGL.RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.RenderbufferStorageMultisample(OpenGL.RenderbufferTarget.RenderbufferExt, numberOfSamples, OpenGL.RenderbufferStorage.Depth24Stencil8, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.DepthAttachmentExt, OpenGL.RenderbufferTarget.RenderbufferExt, depthBuffer);
            Gl.FramebufferRenderbuffer(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.StencilAttachmentExt, OpenGL.RenderbufferTarget.RenderbufferExt, depthBuffer);


            // Bind Texture assuming we have created a texture
            Gl.FramebufferTexture2D(OpenGL.FramebufferTarget.FramebufferExt, OpenGL.FramebufferAttachment.ColorAttachment0Ext, OpenGL.TextureTarget.Texture2D, modelTex, 0);


            if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) != OpenGL.FramebufferErrorCode.FramebufferComplete)
            {

                Console.WriteLine("FAILED TO CREATE FRAME BUFFER");

                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferUndefined)
                {
                    Console.WriteLine("UNDEFINED");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferUnsupported)
                {
                    Console.WriteLine("UNSUPPORTED");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteReadBuffer)
                {
                    Console.WriteLine("INCOMPLETE READ BUFFER");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteMultisample)
                {
                    Console.WriteLine("INCOMPLETE MULTISAMPLE");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteMissingAttachment)
                {
                    Console.WriteLine("INCOMPLETE MISSING ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteAttachment)
                {
                    Console.WriteLine("INCOMPLETE ATTACHMENT");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteDrawBuffer)
                {
                    Console.WriteLine("INCOMPLETE DRAW BUFFER");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteLayerCount)
                {
                    Console.WriteLine("INCOMPLETE LAYER  COUNT");
                }
                if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) == OpenGL.FramebufferErrorCode.FramebufferIncompleteLayerTargets)
                {
                    Console.WriteLine("INCOMPLETE LAYER TARGETS");
                }



                Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);

                return false;
            }

            this.MSAAContourTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);

            return true;


        }


        private bool initContourTexture()
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


            Gl.BindFramebuffer(OpenGL.FramebufferTarget.Framebuffer, 0);


            if (Gl.CheckFramebufferStatus(OpenGL.FramebufferTarget.Framebuffer) != OpenGL.FramebufferErrorCode.FramebufferComplete)
                return false;

            this.contourTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);
            return true;
              

        }






    }
}