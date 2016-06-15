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



        public CameraMode cameraMode { get; set; }
        public DirectionalLight light { get; private set; }



        public FPSCamera fpsCamera { get; private set; }
        public AboveCamera aboveCamera { get; private set; }


        private PostProcessedImage depthTexture;

        private PostProcessedImage normalTexture;

        private PostProcessedImage modelTexture;

        private PostProcessedImage dynamicObjectTexture;



        private double fps;
        private int frame = 0, timebase = 0;






        public Graphics(World world)
        {
            this.world = world;

            initShaders();

            initLight();
            initCameras();

            

            initDepthTexture();
            initNormalTexture();
            initModelTexture();
            initDynamicObjectTexture();


        }



        public  void Render(RenderMode renderMode) {

            frame++;
            int time = Glut.glutGet(Glut.GLUT_ELAPSED_TIME);

            if (time - timebase > 1000)
            {
                fps = frame * 1000.0 / (time - timebase);
                timebase = time;
                frame = 0;
                Console.WriteLine(Gl.GetError());

                Console.WriteLine(fps + "FPS " + time);

            }

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);
            Gl.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            renderModelTexture(time, world.allObjects);
            renderStaticObjects(time, renderMode);


            renderDynamicObjects(time, renderMode);
            renderSimpleContours(time);


            renderHUD(time);

            Glut.glutSwapBuffers();

        }


        private void renderStaticObjects(int time, RenderMode renderMode) {
            renderObjects(time, renderMode, world.staticObjects, true);
        }


        private void renderDynamicObjects(int time, RenderMode renderMode) {
            renderObjects(time, renderMode, world.dynamicObjects, true);
        }

        private void renderSimpleContours(int time) {
            this.modelTexture.Render(RenderMode.SIMPLE_EDGE);

        }
        private void renderContours(int time) {
            this.normalTexture.Render(RenderMode.EDGE);

        }

        private void renderHUD(int time) {

        }

        private void renderObjects(int time, RenderMode renderMode, List<GameObject> objects, bool mipmap)
        {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Window.WIDTH / (float)Window.HEIGHT), 0.1f, 1000f);
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
                if(d != null)
                    d.Render(time, projectionMatrix, viewMatrix, light, renderMode, mipmap);
            }

        }

        private void renderDynamicObjectTexture(int time, List<GameObject> objects)
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, dynamicObjectTexture.FBO);

            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            renderDynamicObjects(time, RenderMode.CEL);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


        }



        private void renderNormalTexture(int time, List<GameObject> objects)
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, normalTexture.FBO);

            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            renderObjects(time, RenderMode.NORMAL, objects, false);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


        }

        private void renderModelTexture(int time, List<GameObject> objects)
        {

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, modelTexture.FBO);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            renderObjects(time, RenderMode.MODEL, objects, false);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }

        private void renderDepthTexture(int time, List<GameObject> objects)
        {

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, depthTexture.FBO);

            Gl.DrawBuffer(DrawBufferMode.None);
            Gl.ReadBuffer(ReadBufferMode.None);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            renderObjects(time, RenderMode.BASIC, objects, false);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.DrawBuffer(DrawBufferMode.Back);
            Gl.ReadBuffer(ReadBufferMode.Back);

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


        private bool initDepthTexture()
        {

            uint depthTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, depthTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, TextureParameter.CompareRefToTexture);

            Gl.DepthFunc(DepthFunction.Lequal);

            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);


            uint depthFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, depthFBO);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTex, 0);

            Gl.DrawBuffer(DrawBufferMode.None);
            Gl.ReadBuffer(ReadBufferMode.None);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;



            this.depthTexture =  new PostProcessedImage(Window.WIDTH, Window.HEIGHT, depthTex, depthFBO, new Vector3(0, 0, 0), this);

            return true;

        }

        private bool initDynamicObjectTexture()
        {

            uint dynamicObjectTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, dynamicObjectTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, TextureParameter.CompareRefToTexture);

            Gl.DepthFunc(DepthFunction.Lequal);

            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);


            uint dynamicObjectFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, dynamicObjectFBO);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, dynamicObjectFBO, 0);

            Gl.DrawBuffer(DrawBufferMode.None);
            Gl.ReadBuffer(ReadBufferMode.None);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;



            this.dynamicObjectTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, dynamicObjectTex, dynamicObjectFBO, new Vector3(0, 0, 0), this);

            return true;

        }

        private bool initNormalTexture()
        {

            uint normalTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, normalTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);



            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


            uint normalFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, normalFBO);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, normalTex, 0);



            // The depth buffer
            uint depthrenderbuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;

            this.normalTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, normalTex, normalFBO, new Vector3(0, 0, 0), this);


            return true;
        }

        private bool initModelTexture()
        {

            uint modelTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, modelTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);



            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


            uint modelFBO = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, modelFBO);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, modelTex, 0);



            // The depth buffer
            uint depthrenderbuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;

            this.modelTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, modelTex, modelFBO, new Vector3(0, 0, 0), this);

            return true;
        }






    }
}