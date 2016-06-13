using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FreeGlut;
using CH3.Camera;

using OpenGL;

namespace CH3
{
    public class Game
    {
        public HUD HUD
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public World World
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        public Graphics Graphics
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        private List<GameObject> objects;
        private Window gameWindow;
        private DirectionalLight light;

        private CelShader celShader;
        private BasicShaderProgram basicShader;
        private NormalShader normalShader;
        private DepthShader depthShader;

        private PostProcessedImage normalTexture;
        private PostProcessedImage depthTexture;

        private Soil soil;

        private FPSCamera camera;
        private double fps;
        private int frame = 0, timebase = 0;

        private int renderMode;

        private bool[] activeKeys = new bool[255];
        VBO<float> verticesVbo;
        VBO<int> indicesVbo;

        public Game()
        {
            gameWindow = new Window();



            float[] vertices = { -Window.WIDTH/2, -Window.HEIGHT/2,
                              -Window.WIDTH/2, Window.HEIGHT/2,
                             Window.WIDTH/2, Window.HEIGHT/2,
                              Window.WIDTH/2, -Window.HEIGHT/2
            };

            float[] texCoords = { 0, 0,
                             0, 1,
                             1,1,
                              1, 0
            };

            int[] indices = { 0, 1, 2,
                                0, 2, 3
            };




            renderMode = Drawable.RENDER_MODE_CEL;

            if (!gameWindow.createWindow()) {
                Console.WriteLine("ERROR: Could not initialize GLUT");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            camera = new FPSCamera(new Vector3(-20, 0, 10), Vector3.Zero, activeKeys);
            CreateShaders();
            CreateObjects();
            CreateLight();



            SetGlutMethods();
            
           Console.WriteLine("INIT DEPTH TEXTURE: " + depthTex);
           bool success = initDepthTexture();
            Console.WriteLine(success);
            Console.WriteLine("AFTER DEPTH TEXTURE: " + depthTex);

            Console.WriteLine("INIT NORMAL TEXTURE: " + normalTex);
            success = initNormalTexture();
            Console.WriteLine(success);
            Console.WriteLine("AFTER NORMAL TEXTURE: " + normalTex);


            InitPostProcessedImage();


            verticesVbo = new VBO<float>(vertices);
            indicesVbo = new VBO<int>(indices, BufferTarget.ElementArrayBuffer);

        }

        private void CreateLight()
        {
            light = new DirectionalLight(new Vector3(0, 0, -1).Normalize());
        }

        private void CreateShaders()
        {
            basicShader = new BasicShaderProgram();
            basicShader.initShader();
            celShader = new CelShader();
            celShader.initShader();
            normalShader = new NormalShader();
            normalShader.initShader();
            depthShader = new DepthShader();
            depthShader.initShader();
            
        }


        private void CreateObjects()
        {


            objects = new List<GameObject>();
            objects.Add(new House(new Vector3(-120, -120, 0), new Vector3(1, 1, 1), 0f, basicShader, normalShader, celShader, depthShader));
            objects.Add(new FarmHouse(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0f, basicShader, normalShader, celShader, depthShader));


            CreateSoil();
        }

        private void InitPostProcessedImage() {

            normalTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, normalTex, new Vector3(0, 0,0), basicShader);
            depthTexture = new PostProcessedImage(Window.WIDTH, Window.HEIGHT, depthTex, new Vector3(0,0, 0), basicShader);

        }

        private void CreateSoil()
        {

            int soilSize = 10;
            int soils = 1;
            Vector3[] positions = new Vector3[soils * soils];
            
            for (int y = 0; y < soils; y++)
            {
                for (int x = 0; x < soils; x++)
                {
                    positions[y * soils + x] = new Vector3(x * soilSize, y * soilSize, 0);
                }
            }
            soil = new Soil(positions, new Vector3(1, 1, 1), 0f, basicShader, normalShader, celShader, depthShader);
        }
        private void SetGlutMethods()
        {
            Glut.glutIdleFunc(render);
            Glut.KeyboardCallback keyDownFunc = KeyDown;
            Glut.glutKeyboardFunc(keyDownFunc);
            Glut.KeyboardUpCallback keyUpFunc = KeyUp;
            Glut.glutKeyboardUpFunc(keyUpFunc);
            Glut.MouseCallback mouseFunc = MouseInput;
            Glut.glutMouseFunc(mouseFunc);
            Glut.MotionCallback motionFunc = camera.Motion;
            Glut.glutMotionFunc(motionFunc);
            Glut.glutTimerFunc(1, camera.MoveCamera, 0); 
        }

        public void MouseInput(int button, int state, int x, int y)
        {
            //Console.WriteLine($"{button} : {state} : {x} : {y}");
        }

        public void KeyUp(byte key, int x, int y)
        {
           activeKeys[key] = false;
        }

        public void KeyDown(byte key, int x, int y)
        {
            activeKeys[key] = true;
            Console.WriteLine($"{key} : {x} : {y}");
            if (key == 27) //esc
            {
                Environment.Exit(0);
            }

            if (key == 110)
            {
                if (this.renderMode == Drawable.RENDER_MODE_CEL)
                {
                    this.renderMode = Drawable.RENDER_MODE_NORMAL;
                }
                else if (this.renderMode == Drawable.RENDER_MODE_NORMAL)
                {
                    this.renderMode = Drawable.RENDER_MODE_DEPTH;
                }
                else
                {
                    this.renderMode = Drawable.RENDER_MODE_CEL;        
                }
            }
        }

        public void run(int fps)
        {
            Glut.glutMainLoop();
   
        }

        private void handleEvents()
        {


        }

        private void render()
        {

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(CullFaceMode.Back);

            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            frame++;
            int time = Glut.glutGet(Glut.GLUT_ELAPSED_TIME);

            if (time - timebase > 1000) {
                fps = frame * 1000.0 / (time - timebase);
                timebase = time;
                frame = 0;
                Console.WriteLine(Gl.GetError());

                Console.WriteLine(fps + "FPS " + time);

            }

            normalTex = renderNormalTexture(time);
            depthTex = renderDepthTexture(time);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            renderScene(time, renderMode);

            normalTexture.render();
          //  depthTexture.render();

            Glut.glutSwapBuffers();

        }

        private void update()
        {
        }


        uint depthFB;
        uint depthTex;
        uint normalFB;
        uint normalTex;

        private bool initDepthTexture()
        {

            depthTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, depthTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, TextureParameter.CompareRefToTexture);

            Gl.DepthFunc(DepthFunction.Lequal);

            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);


            depthFB = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, depthFB);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTex, 0);

            Gl.DrawBuffer(DrawBufferMode.None);
            Gl.ReadBuffer(ReadBufferMode.None);

            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;

            return true;

        }

        private bool initNormalTexture()
        {

            normalTex = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, normalTex);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.Repeat);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);



            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Window.WIDTH, Window.HEIGHT, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


            normalFB = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, normalFB);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, normalTex, 0);



            // The depth buffer
            uint depthrenderbuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, Window.WIDTH, Window.HEIGHT);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);

            if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                return false;

            return true;
        }



        public uint renderNormalTexture(int time)
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, normalFB);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            renderScene(time, Drawable.RENDER_MODE_NORMAL);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return normalTex;

        }

        public uint renderDepthTexture(int time)
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, depthFB);

            Gl.DrawBuffer(DrawBufferMode.None);
            Gl.ReadBuffer(ReadBufferMode.None);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            renderScene(time, Drawable.RENDER_MODE_BASIC);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.DrawBuffer(DrawBufferMode.Back);
            Gl.ReadBuffer(ReadBufferMode.Back);

            return depthTex;
        }



        public void renderScene(int time, int renderMode) {
            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT)), 0.1f, 1000000f);
            Matrix4 viewMatrix = camera.viewMatrix;

            //Render floor
            soil.render(time, projectionMatrix, viewMatrix, light, renderMode);

            //Render teapots
            foreach (GameObject t in objects)
            {

                t.render(time, projectionMatrix, viewMatrix, light, renderMode);
            }

            //      soil.render(time, Matrix4.CreateOrthographic(100, 100, -1, 1), Matrix4.Identity, light, Drawable.RENDER_MODE_BASIC);

        }


        public void drawFullscreenTexture(uint tex) {
    /*        Gl.Viewport(3*Window.WIDTH / 4, 0, Window.WIDTH/4, Window.HEIGHT/4);


        //   Gl.Enable(EnableCap.Texture2D);
         //   Gl.Enable(EnableCap.DepthTest);
           // Gl.Enable(EnableCap.Blend);
         //   Gl.DepthMask(false);  // disable writes to Z-Buffer
          //  Gl.Disable(EnableCap.DepthTest);  // disable depth-testing

            //     Gl.BindTexture(TextureTarget.Texture2D, tex);


            texShader.useProgram();



            Gl.BindBuffer(verticesVbo);
            Gl.VertexAttribPointer(texShader.vertexPositionIndex, 2, verticesVbo.PointerType, false, 8, IntPtr.Zero);



        //    Gl.BindBuffer(texCoordsVbo);
         //   Gl.VertexAttribPointer(texShader.vertexTexCoordIndex, 2, texCoordsVbo.PointerType, true, 8, IntPtr.Zero);


            Gl.BindBuffer(indicesVbo);


            Gl.DrawElements(BeginMode.Triangles, indicesVbo.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);


          //  Gl.BindTexture(TextureTarget.Texture2D, 0);
          */
        }
    }
}