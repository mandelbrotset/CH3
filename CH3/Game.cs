using System;
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


        public Game()
        {
            renderMode = RenderMode.CEL;
            gameWindow = new Window();

            if (!gameWindow.createWindow())
            {
                Console.WriteLine("ERROR: Could not initialize GLUT");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            map = new Map.Map();
            world = new World(map);
            graphics = new Graphics(world);

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
            if (key == 110) //Toggle cel, normal and depth shading
            {
                if (renderMode == RenderMode.CEL)
                    renderMode = RenderMode.NORMAL;
                else if (renderMode == RenderMode.NORMAL)
                    renderMode = RenderMode.DEPTH;
                else if (renderMode == RenderMode.DEPTH)
                    renderMode = RenderMode.MODEL;
                else
                    renderMode = RenderMode.CEL;
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

            player = new Player(new Vector3(100, 100, 0), new Vector3(0.3f, 0.3f, 0.3f), 0, graphics);

            // player.SetUpdateCamera(graphics.aboveCamera.UpdateCamera);
            world.AddObject(house);
            world.AddObject(farmHouse);
            world.AddObject(farmHouse1);
            world.AddObject(farmHouse2);
            world.AddObject(farmHouse3);
            world.AddObject(farmHouse4);
            world.AddObject(farmHouse5);


            //     world.staticObjects.Add(farmHouse3);

            world.AddObject(player);

        }

        private void CreateSky()
        {
            
            sky = new Sky(new Vector3(0, 0, 0), new Vector3(100.0, 100.0, 100.0), 0f, 0f, 0f, graphics);
            world.AddObject(sky);
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
            graphics.Render(renderMode);


        }

        private void update()
        {
        }





    }
}