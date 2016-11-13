using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH3.Camera;

using static SFML.Window.Keyboard;
using OpenGL;


using static CH3.Graphics;
using CH3.Utils;
using CH3.Physics;
using CH3.GameObjects.DynamicObjects.Vehicles;

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
        private Mouse mouse;
        private WorldPhysics physics;
        private bool[] activeKeys = new bool[255];
        private RenderMode renderMode;
        private AAMode aaMode;
        private ContourMode contourMode;
        private WorldCuller culler;
        private int prev_time;
        DynamicObject active_object;
        SFML.System.Clock clock;


        public Car car;

        public Game()
        {
            renderMode = RenderMode.CEL;
            gameWindow = new Window();
            aaMode = AAMode.FXAA;
            contourMode = ContourMode.MSAA;

            mouse = new Mouse();
            if (!gameWindow.createWindow())
            {
                Console.WriteLine("ERROR: Could not initialize GLFW");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(1);
            }

            map = new Map.Map();
            world = new World(map);
            graphics = new GraphicsForward(world, aaMode, contourMode, gameWindow);
            
            culler = new WorldCuller(world, graphics);
            clock = new SFML.System.Clock();

            physics = new WorldPhysics();
            graphics.aboveCamera.follow = player;
            CreateObjects();
            
            Input.SubscribeKeyPressed(KeyPressed);
        }

        private void CreateObjects()
        {
            world.staticObjects.Add(null);



            StaticObject house = new House(new Vector3(-120, -120, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse = new FarmHouse(new Vector3(100, 0, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse1 = new FarmHouse(new Vector3(0, 100, 0), new Vector3(1, 1, 1), 0f, graphics);
            StaticObject farmHouse2 = new FarmHouse(new Vector3(100, -100, 0), new Vector3(1, 1, 1), 0f, graphics);

            car = new Car(new Vector3(0, 0, 0), new Vector3(2, 2, 2), 0f, graphics);
            graphics.aboveCamera.follow = car;
            DynamicFarmHouse farmHouse3 = new DynamicFarmHouse(new Vector3(0, -100, 100), new Vector3(1, 1, 1), 0f, graphics);
            DynamicFarmHouse farmHouse4 = new DynamicFarmHouse(new Vector3(0, -200, 500), new Vector3(1, 1, 1), 0f, graphics);
            DynamicFarmHouse farmHouse5 = new DynamicFarmHouse(new Vector3(0, 200, 100), new Vector3(1, 1, 1), 0f, graphics);

            player = new Player(new Vector3(0, 0, 0), new Vector3(0.3f, 0.3f, 0.3f), 0, graphics);

            //player.SetUpdateCamera(graphics.aboveCamera.UpdateCamera);

            world.AddObject(farmHouse);
            world.AddObject(farmHouse1);
            world.AddObject(farmHouse2);
            world.AddObject(farmHouse3);
            world.AddObject(farmHouse4);
            world.AddObject(farmHouse5);
            world.AddObject(house);
            

            foreach (GameObject o in world.allObjects) {
                o.has_physics = true;
                physics.AddRigidBody(o);
            }

            world.AddObject(car);
            car.has_physics = true;
            physics.AddVehicle(car);

            CreateSky();
            CreateSoil();

            world.AddObject(player);
            
            
            culler.Init();

            active_object = car;

            world.visibleObjects.AddRange(world.allObjects);
        }

        private void CreateSky()
        {    
            sky = new Sky(new Vector3(0, 0, -1000), new Vector3(0.01, 0.01, 0.01), (float)-Math.PI/2, 0f, 0f, graphics);
            world.sky = sky;
            world.AddObject(sky);
        }

        private void CreateSoil()
        {      
            float scale = 10.0f;
            soil = new Soil(new Vector3(0, 0, 0), new Vector3(scale, scale, scale), 0f, scale, graphics);
            world.AddObject(soil);
        }

        public void run(uint fps)
        {
            GameLoop(fps);
        }

        private void GameLoop(uint fps_target)
        {
            //  player.Move();
            //while (!Glfw.WindowShouldClose(gameWindow.window))
            gameWindow.window.SetFramerateLimit(fps_target);

            uint fps = 0;
            float fps_timer = 0;
            while (true)
            {
                int time = clock.ElapsedTime.AsMilliseconds();
                float dt = (time - prev_time) * 0.001f;
                fps_timer += dt;
                prev_time = time;
                fps++;
                if(fps_timer > 1)
                {
                    Console.WriteLine(fps);
                    fps_timer = 0;
                    fps = 0;
                }


                gameWindow.HandleEvents();
                update(dt);
                render();
            }
        }

        private void KeyPressed(Key key)
        {
           switch (key)
            {
                case Key.Escape:
                    physics.Exit();
                    Environment.Exit(0);
                    break;
                case Key.N:
                    if (renderMode == RenderMode.CEL)
                        renderMode = RenderMode.NORMAL;
                    else if (renderMode == RenderMode.NORMAL)
                        renderMode = RenderMode.MODEL;
                    else
                        renderMode = RenderMode.CEL;

                    Console.WriteLine(renderMode.ToString());
                    break;
                case Key.M:
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
                    break;
                case Key.B:
                    if (contourMode == ContourMode.OFF)
                        contourMode = ContourMode.ON;
                    else if (contourMode == ContourMode.ON)
                        contourMode = ContourMode.MSAA;
                    else
                        contourMode = ContourMode.OFF;
                    Console.WriteLine(contourMode.ToString());
                    break;
                case Key.P:
                    mouse.ToogleCaptured();
                    break;
                case Key.E:
                    switch(graphics.cameraMode)
                    {
                        case CameraMode.FPS:
                            graphics.cameraMode = CameraMode.PLAYER;
                            graphics.activeCamera = graphics.aboveCamera;
                            break;
                        case CameraMode.PLAYER:
                            graphics.cameraMode = CameraMode.FPS;
                            graphics.activeCamera = graphics.fpsCamera;
                            break;
                    }
                    break;
            }
        }

        private void update(float dt)
        {
            physics.Update(dt);
            Vector2 mouse_movement = mouse.Update();
            if(graphics.cameraMode == CameraMode.PLAYER)
                active_object.HandleInput(mouse_movement, dt);

            graphics.activeCamera.Update(mouse_movement);
            world.Tick();
            
        }

        private void render()
        {
            graphics.Render(renderMode, aaMode, contourMode);
        }
    }
}