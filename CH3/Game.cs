using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH3.Camera;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
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
        private AAMode aaMode;
        private ContourMode contourMode;



        public Game()
        {
            renderMode = RenderMode.CEL;
            gameWindow = new Window();
            aaMode = AAMode.FXAA;
            contourMode = ContourMode.ON;

            if (!gameWindow.createWindow())
            {
                Console.WriteLine("ERROR: Could not initialize GLFW");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            map = new Map.Map();
            world = new World(map);
            graphics = new Graphics(world, aaMode, contourMode, gameWindow);

          //  Input.Init();
          //  Input.SubscribeKeyDown(KeyDown);

            CreateObjects();
            graphics.aboveCamera.follow = player;



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




        public void run(int fps)
        {
            GameLoop(fps);

        }


        private void GameLoop(int fps)
        {
            //  player.Move();

            while (!Glfw.WindowShouldClose(gameWindow.window))
            {

                handleEvents();
                // update();
                  render();
            }

        }


        private void handleEvents()
        {
            Glfw.PollEvents();

            if (Glfw.GetKey(gameWindow.window, Key.Escape))
                Glfw.SetWindowShouldClose(gameWindow.window, true);

            if (Glfw.GetKey(gameWindow.window, Key.N))
            {
                if (renderMode == RenderMode.CEL)
                    renderMode = RenderMode.NORMAL;
                else if (renderMode == RenderMode.NORMAL)
                    renderMode = RenderMode.MODEL;
                else
                    renderMode = RenderMode.CEL;

                Console.WriteLine(renderMode.ToString());
            }

            if (Glfw.GetKey(gameWindow.window, Key.M))
            {
                if (aaMode == AAMode.OFF)
                    aaMode = AAMode.MSAA_X2;
                else if (aaMode == AAMode.MSAA_X2)
                    aaMode = AAMode.MSAA_X4;
                else if (aaMode == AAMode.MSAA_X4)
                    aaMode = AAMode.MSAA_X8;
                else if (aaMode == AAMode.MSAA_X8)
                    aaMode = AAMode.FXAA;
                else
                    aaMode = AAMode.OFF;

                Console.WriteLine(aaMode.ToString());
            }
            if (Glfw.GetKey(gameWindow.window, Key.B))
            {
                if (contourMode == ContourMode.OFF)
                    contourMode = ContourMode.ON;
                else if (contourMode == ContourMode.ON)
                    contourMode = ContourMode.MSAA;
                else
                    contourMode = ContourMode.OFF;


                Console.WriteLine(contourMode.ToString());
            }

        }

        private void update()
        {
            world.Tick();
        }

        private void render()
        {
            graphics.Render(renderMode, aaMode, contourMode);


        }


    }
}