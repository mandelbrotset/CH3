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

        private List<GameObject> objects;
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
                Console.WriteLine("ERROR: Could not initialize GLUT");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            Glut.glutIdleFunc(render);

            shader = new BasicShaderProgram();
            camera = new Camera(new Vector3(0, 0, 100), Vector3.Zero);
            objects = new List<GameObject>();

       //     objects.Add(new Teapot(new Vector3(-1, 0, 0), new Vector3(0.1, 0.1, 0.1), 0f, shader));
            objects.Add(new Building(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0f, shader));

            floor = new VBO<Vector3>(new Vector3[] { new Vector3(1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(-1, -1, 0) });
            floorElements = new VBO<int>(new int[] { 3,0,1, 0, 1, 2, 0, 2, 1, 3}, BufferTarget.ElementArrayBuffer);

        }

        private void setCallbackMethods()
        {
            Glut.KeyboardCallback keyboardFunc = KeyboardInput;
            Glut.glutKeyboardFunc(keyboardFunc);
            Glut.MouseCallback mouseFunc = MouseInput;
            Glut.glutMouseFunc(mouseFunc);
        }

        public void MouseInput(int button, int state, int x, int y)
        {
            Console.WriteLine($"{button} : {state} : {x} : {y}");
        }

        public void KeyboardInput(byte key, int x, int y)
        {
            Console.WriteLine($"{key} : {x} : {y}");
            float speed = 3.0f;
            switch (key)
            {
                case 32://space
                    moveCamera(new Vector3(0.0f, 0.0f, speed));
                    break;
                case 99://c
                    moveCamera(new Vector3(0.0f, 0.0f, -speed));
                    break;
                case 97://a
                    moveCamera(new Vector3(speed, 0.0f, 0));
                    break;
                case 100://d
                    moveCamera(new Vector3(-speed, 0.0f, 0));
                    break;
                case 119://w
                    moveCamera(new Vector3(0, speed, 0));
                    break;
                case 115://s
                    moveCamera(new Vector3(0, -speed, 0));
                    break;
            }
        }

        private void moveCamera(Vector3 delta)
        {
            camera.position = camera.position + delta;
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

              //  Console.WriteLine(fps + "FPS");

            }


            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            camera.target = Vector3.Lerp(camera.target, objects.First().position, 0.1f);
         //   camera.position = Vector3.Lerp(camera.position, new Vector3(camera.target.x, camera.target.y - 10, 8), 0.004f);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Window.WIDTH / Window.HEIGHT), 0.1f, 1000f);
            Matrix4 viewMatrix = camera.viewMatrix;



            //Render floor
            for (int x = -10; x < 10; x += 2)
                for (int y = -10; y < 10; y += 2)
                    renderFloor(new Vector3(x, y, 0), projectionMatrix, viewMatrix);


            //Render teapots
            foreach (GameObject t in objects) {

             //   t.position = t.position + new Vector3(Math.Sin(time*0.001)*0.005, Math.Sin(-time * 0.001)* Math.Cos(time * 0.001) * 0.005, 0);
              //  t.rotation += (float)((-0.001));
               /* if (t.rotation > 2*Math.PI)
                    t.rotation -= (float)(2*Math.PI);
                if (t.rotation < 0)
                    t.rotation += (float)(2 * Math.PI);

    */


                t.render(time, projectionMatrix, viewMatrix);
            }


            Glut.glutSwapBuffers();

        }

        private void update()
        {
        }
    }
}