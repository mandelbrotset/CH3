using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FreeGlut;

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


        private List<Teapot> teapots;
        private Window gameWindow;
        private BasicShaderProgram shader;
        private VBO<Vector3> floor;
        private VBO<int> floorElements;


        private Camera camera;
        private double fps;
        private int frame = 0, timebase = 0;



        public Game()
        {
            gameWindow = new Window();

            if (!gameWindow.createWindow()) {
                Console.WriteLine("ERROR: Could not initialize GLFW");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }


            Glut.glutIdleFunc(render);


            shader = new BasicShaderProgram();
            camera = new Camera(new Vector3(0, 0, 100), Vector3.Zero);
            teapots = new List<Teapot>();


            teapots.Add(new Teapot(new Vector3(-1, 0, 0), new Vector3(0.1, 0.1, 0.1), 0f, shader));
            teapots.Add(new Teapot(new Vector3(1, 0, 0), new Vector3(0.4, 0.4, 0.4), 0f, shader));

            floor = new VBO<Vector3>(new Vector3[] { new Vector3(1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(-1, -1, 0) });
            floorElements = new VBO<int>(new int[] { 3,0,1, 0, 1, 2, 0, 2, 1, 3}, BufferTarget.ElementArrayBuffer);


        }


        public void run(int fps)
        {
            Glut.glutMainLoop();
   
        }

        private void handleEvents()
        {


        }

        private void renderFloor(Vector3 position, Matrix4 projectionMatrix, Matrix4 viewMatrix) {
            shader.useProgram();

            shader.setProjectionMatrix(projectionMatrix);
            shader.setViewMatrix(viewMatrix);
            shader.setModelMatrix(Matrix4.CreateTranslation(position));

            Gl.BindBuffer(floor);
            Gl.VertexAttribPointer(shader.vertexPositionIndex, floor.Size, floor.PointerType, true, 12, IntPtr.Zero);
            Gl.BindBuffer(floorElements);
            Gl.DrawElements(BeginMode.Lines, floorElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

        }

        private void render()
        {

            frame++;
            int time = Glut.glutGet(Glut.GLUT_ELAPSED_TIME);

            if (time - timebase > 1000) {
                fps = frame * 1000.0 / (time - timebase);
                timebase = time;
                frame = 0;

                Console.WriteLine(fps + "FPS");

            }


            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            camera.target = Vector3.Lerp(camera.target, teapots.First().position, 0.1f);
            camera.position = Vector3.Lerp(camera.position, new Vector3(camera.target.x, camera.target.y - 10, 8), 0.004f);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Window.WIDTH / Window.HEIGHT), 0.1f, 1000f);
            Matrix4 viewMatrix = camera.viewMatrix;



            //Render floor
            for (int x = -10; x < 10; x += 2)
                for (int y = -10; y < 10; y += 2)
                    renderFloor(new Vector3(x, y, 0), projectionMatrix, viewMatrix);


            //Render teapots
            foreach (Teapot t in teapots) {

                t.position = t.position + new Vector3(Math.Sin(time*0.001)*0.005, Math.Sin(-time * 0.001)* Math.Cos(time * 0.001) * 0.005, 0);
                t.rotation += (float)((-0.001));
                if (t.rotation > 2*Math.PI)
                    t.rotation -= (float)(2*Math.PI);
                if (t.rotation < 0)
                    t.rotation += (float)(2 * Math.PI);



                t.render(time, projectionMatrix, viewMatrix);
            }


            Glut.glutSwapBuffers();

        }

        private void update()
        {
        }
    }
}