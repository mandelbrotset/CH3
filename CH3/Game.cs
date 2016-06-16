﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FreeGlut;
using CH3.Camera;

using OpenGL;
using static CH3.Graphics;

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
        private World world;
        private Map.Map map;
        private Graphics graphics;
        private Sky sky;
        private Soil soil;
        private Player player;


        private bool[] activeKeys = new bool[255];
        private RenderMode renderMode;
        private MSAAMode msaaMode;
        private ContourMode contourMode;



        public Game()
        {
            renderMode = RenderMode.CEL;
            gameWindow = new Window();
            msaaMode = MSAAMode.MSAA_X2;
            contourMode = ContourMode.ON;

            if (!gameWindow.createWindow())
            {
                Console.WriteLine("ERROR: Could not initialize GLUT");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            map = new Map.Map();
            world = new World(map);
            graphics = new Graphics(world, msaaMode, contourMode);

            Input.Init();
            Input.SubscribeKeyDown(KeyDown);

            CreateObjects();
            graphics.aboveCamera.follow = player;

            SetGlutMethods();



        }

        private void KeyDown(byte key, int x, int y)
        {

            if (key == 27) //esc
            {
                Environment.Exit(0);
            }
            if (key == 110) //N - Toggle cel, normal and depth shading
            {
                if (renderMode == RenderMode.CEL)
                    renderMode = RenderMode.NORMAL;
                else if (renderMode == RenderMode.NORMAL)
                    renderMode = RenderMode.MODEL;
                else
                    renderMode = RenderMode.CEL;

                Console.WriteLine("SWITCHING TO RENDER MODE: " + renderMode.ToString());

            }
            if (key == 109) //M - Toggle multisampling
            {
                if (msaaMode == MSAAMode.OFF)
                    msaaMode = MSAAMode.MSAA_X2;
                else if (msaaMode == MSAAMode.MSAA_X2)
                    msaaMode = MSAAMode.MSAA_X4;
                else if (msaaMode == MSAAMode.MSAA_X4)
                    msaaMode = MSAAMode.MSAA_X8;
                else
                    msaaMode = MSAAMode.OFF;

                Console.WriteLine("SWITCHING TO MSAA: " + msaaMode.ToString());

            }
            if (key == 98) //B - Toggle multisampled contours
            {
                if (contourMode == ContourMode.OFF)
                    contourMode = ContourMode.ON;
                else if (contourMode == ContourMode.ON)
                    contourMode = ContourMode.MSAA;
                else 
                    contourMode = ContourMode.OFF;


                Console.WriteLine("SWITCHING TO CONTOUR MODE: " + contourMode.ToString());

            }
        }


        private void CreateObjects()
        {
            world.staticObjects.Add(null);

            CreateSky();
            CreateSoil();
       
            StaticObject house = new House(new Vector3(-120, -120, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse = new FarmHouse(new Vector3(0, 0, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse1 = new FarmHouse(new Vector3(0, 100, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse2 = new FarmHouse(new Vector3(0, 200, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse3 = new FarmHouse(new Vector3(0, -100, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse4 = new FarmHouse(new Vector3(0, -200, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse5 = new FarmHouse(new Vector3(100, -100, 0), new Vector3(1, 1, 1), 0f, graphics);

            player = new Player(new Vector3(50, 50, 0), new Vector3(0.3f, 0.3f, 0.3f), 0, graphics);

            // player.SetUpdateCamera(graphics.aboveCamera.UpdateCamera);
            world.AddObject(house);
            world.AddObject(farmHouse);
            world.AddObject(farmHouse1);
            world.AddObject(farmHouse2);
            world.AddObject(farmHouse3);
            world.AddObject(farmHouse4);
            world.AddObject(farmHouse5);



            world.AddObject(player);
            
        }

        private void CreateSky()
        {
            
            sky = new Sky(new Vector3(0, 0, -1000), new Vector3(0.01, 0.01, 0.01), (float)-Math.PI/2, 0f, 0f, graphics);
            world.sky = sky;
        }

        private void CreateSoil()
        {
            float scale = 10.0f;
            soil = new Soil(new Vector3(0, 0, 0), new Vector3(scale, scale, scale), 0f, scale, graphics);
            world.AddObject(soil);

        }
        private void SetGlutMethods()
        {
            Glut.glutIdleFunc(render);
            //   Glut.glutTimerFunc(1, GameLoop, 0);         
        }

        public void GameLoop(int i)
        {
            //  player.Move();

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
            graphics.Render(renderMode, msaaMode, contourMode);


        }

        private void update()
        {
        }





    }
}