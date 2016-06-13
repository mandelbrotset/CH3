﻿using System;
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

        public enum CameraMode
        {
            FPS, PLAYER
        }
        private CameraMode cameraMode = CameraMode.PLAYER;
        private List<GameObject> objects;
        private Window gameWindow;
        private DirectionalLight light;
        private Player player;
        private Soil soil;

        private Above aboveCamera;
        private FPSCamera fpsCamera;
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
            CreateLight();
            fpsCamera = new FPSCamera(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            aboveCamera = new Above();
            aboveCamera.height = 100;
            CreateObjects();
            aboveCamera.follow = player;
            Input.Init();
            Input.SubscribeKeyDown(KeyDown);
            SetGlutMethods();
            Glut.glutSetCursor(Glut.GLUT_CURSOR_NONE);
        }

        private void KeyDown(byte key, int x, int y)
        {
            if (key == 27) //esc
            {
                Environment.Exit(0);
            }
        }

        private void CreateLight()
        {
            light = new DirectionalLight(new Vector3(0, 0, -1).Normalize());
        }

        private void CreateObjects()
        {
            objects = new List<GameObject>();
            objects.Add(new House(new Vector3(-120, -120, 0), new Vector3(1, 1, 1), 0f, new BasicShaderProgram()));
            objects.Add(new FarmHouse(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0f, new BasicShaderProgram()));
            objects.Add(new Tower(new Vector3(150, 150, 0), new Vector3(1, 1, 1), 0f, new BasicShaderProgram()));
            player = new Player(new Vector3(10, 10, 0), new Vector3(0.1f, 0.1f, 0.1f), 0, new BasicShaderProgram());
            player.SetUpdateCamera(aboveCamera.UpdateCamera);
            objects.Add(player);
            //  objects.Add(new Grass(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0f, 0f, 0f, new BasicShaderProgram()));
            CreateSoil();
        }

        private void CreateSoil()
        {
            float scale = 10.0f;
            soil = new Soil(new Vector3(0,0,0), new Vector3(scale, scale, scale), 0f, new BasicShaderProgram(), scale);
        }
        private void SetGlutMethods()
        {
            Glut.glutIdleFunc(render);
            Glut.glutTimerFunc(1, GameLoop, 0);         
        }

        public void GameLoop(int i)
        {
            player.Move();
            Glut.glutTimerFunc(1, GameLoop, 0);
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
                Console.WriteLine(Gl.GetError());

                Console.WriteLine(fps + "FPS " + time);

            }

            Gl.Viewport(0, 0, Window.WIDTH, Window.HEIGHT);

            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            //camera.target = Vector3.Lerp(camera.target, objects.First().position, 0.1f);
         //   camera.position = Vector3.Lerp(camera.position, new Vector3(camera.target.x, camera.target.y - 10, 8), 0.004f);


            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(0.45f, ((float)Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT)), 0.1f, 1000f);
            Matrix4 viewMatrix;
            if (cameraMode == CameraMode.FPS)
            {
                viewMatrix = fpsCamera.viewMatrix;
            } else
            {
                viewMatrix = aboveCamera.viewMatrix;
            }
            

            //Render floor
            soil.render(time, projectionMatrix, viewMatrix, light);

            //Render teapots
            foreach (GameObject t in objects) {
                /*
                t.position = t.position + new Vector3(Math.Sin(time*0.001)*0.005, Math.Sin(-time * 0.001)* Math.Cos(time * 0.001) * 0.005, 0);
                t.rotationZ += (float)((-0.001));
                if (t.rotationZ > 2*Math.PI)
                    t.rotationZ -= (float)(2*Math.PI);
                if (t.rotationZ < 0)
                    t.rotationZ += (float)(2 * Math.PI);*/

                t.render(time, projectionMatrix, viewMatrix, light);
            }

            Glut.glutSwapBuffers();
        }

        private void update()
        {
        }

        
    }
}