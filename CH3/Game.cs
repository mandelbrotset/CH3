﻿using System;
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

        private Floor floor;

        private Camera camera;
        private double fps;
        private int frame = 0, timebase = 0;

        private bool[] activeKeys = new bool[255];
        private bool warped = false;

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

            camera = new Camera(new Vector3(-20, 0, 10), Vector3.Zero);
            objects = new List<GameObject>();

            objects.Add(new Building(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0f, new BasicShaderProgram()));
            floor = new Floor(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0f, new FloorShaderProgram());

            SetCallbackMethods();
        }

        

        private void SetCallbackMethods()
        {
            Glut.KeyboardCallback keyDownFunc = KeyDown;
            Glut.glutKeyboardFunc(keyDownFunc);
            Glut.KeyboardUpCallback keyUpFunc = KeyUp;
            Glut.glutKeyboardUpFunc(keyUpFunc);
            Glut.MouseCallback mouseFunc = MouseInput;
            Glut.glutMouseFunc(mouseFunc);
            Glut.MotionCallback motionFunc = Motion;
            Glut.glutMotionFunc(motionFunc);
            Glut.glutTimerFunc(1, MoveCamera, 0); 
        }

        public void MoveCamera(int i)
        {
            handleMovement();
            camera.Update(i);
            Glut.glutTimerFunc(1, MoveCamera, 0);
        }

        private void handleMovement()
        {
            float speed = 0.4f;
            if (activeKeys[119])//w
            {
                camera.translation += new Vector3(speed, 0.0f, 0.0f);
            }
            if (activeKeys[97])//a
            {
                camera.translation += new Vector3(0.0f, speed, 0.0f);
            }
            if (activeKeys[115])//s
            {
                camera.translation += new Vector3(-speed, 0.0f, 0.0f);
            }
            if (activeKeys[100])//d
            {
                camera.translation += new Vector3(0.0f, -speed, 0.0f);
            }
            if (activeKeys[32])
            {
                camera.translation += new Vector3(0.0f, 0, speed);
            }
            if (activeKeys['c'])
            {
                camera.translation += new Vector3(0.0f, 0, -speed);
            }
        }

        public void MouseInput(int button, int state, int x, int y)
        {
            Console.WriteLine($"{button} : {state} : {x} : {y}");
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
        }

        private void Motion(int x, int y)
        {
            if (warped )
        {
                warped = false;
                return;
            }
            
            float dX = ((float)x - Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / 2);
            float dY = ((float)y - Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT) / 2);
            float sense = 0.001f;
            camera.yaw -= dX * sense;
            camera.pitch += dY * sense;
            warped = true;
            Glut.glutWarpPointer(Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / 2, Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT) / 2);
            
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
            
            //camera.target = Vector3.Lerp(camera.target, objects.First().position, 0.1f);
         //   camera.position = Vector3.Lerp(camera.position, new Vector3(camera.target.x, camera.target.y - 10, 8), 0.004f);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT)), 0.1f, 1000f);
            Matrix4 viewMatrix = camera.viewMatrix;



            //Render floor
            floor.render(time, projectionMatrix, viewMatrix);

            //Render teapots
            foreach (GameObject t in objects) {

                t.position = t.position + new Vector3(Math.Sin(time*0.001)*0.005, Math.Sin(-time * 0.001)* Math.Cos(time * 0.001) * 0.005, 0);
                t.rotationZ += (float)((-0.001));
                if (t.rotationZ > 2*Math.PI)
                    t.rotationZ -= (float)(2*Math.PI);
                if (t.rotationZ < 0)
                    t.rotationZ += (float)(2 * Math.PI);

    


                t.render(time, projectionMatrix, viewMatrix);
            }


            Glut.glutSwapBuffers();

        }

        private void update()
        {
        }
    }
}