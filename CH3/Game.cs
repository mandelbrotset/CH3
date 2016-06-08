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


        private Window gameWindow;
        private BasicShaderProgram shader;
        private VBO<Vector3> triangle, square;
        private VBO<int> triangleElements, squareElements;

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


            triangle = new VBO<Vector3>(new Vector3[] { new Vector3(0,1,0), new Vector3(-1,-1,0), new Vector3(1, -1,0) });
            square = new VBO<Vector3>(new Vector3[] { new Vector3(-1, 1, 0), new Vector3(1,1,0), new Vector3(1,-1, 0), new Vector3(-1,1,0) });


            triangleElements = new VBO<int>(new int[] { 0, 1, 2 }, BufferTarget.ElementArrayBuffer);
            squareElements = new VBO<int>(new int[] { 0, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);



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


            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.useProgram();


            shader.setProjectionMatrix(Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Window.WIDTH / Window.HEIGHT), 0.1f, 1000f));
            shader.setViewMatrix(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, -Vector3.Up));
            shader.setModelMatrix(Matrix4.CreateTranslation(new Vector3(1.0f, 1.0f, -1.0)));

            Gl.BindBuffer(triangle);

            Gl.VertexAttribPointer(shader.vertexPositionIndex, triangle.Size, triangle.PointerType, true, 12, IntPtr.Zero);

            Gl.BindBuffer(triangleElements);

            Gl.DrawElements(BeginMode.Triangles, triangleElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);


            Glut.glutSwapBuffers();

        }

        private void update()
        {
        }
    }
}