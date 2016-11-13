using BulletSharp;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;

namespace CH3.GameObjects.DynamicObjects.Vehicles
{
    public class Car : DynamicObject
    {
        public RaycastVehicle vehicle;
        public Wheel wheel;
        private float engine_force;
        private float breaking_force;
        private float steering;
        private const float steering_increment = 0.9f;
        private const float engine_force_max = 5000f;
        private const float breaking_force_max = 150;
        private const float steering_clamp = 0.8f;

        
        public Car(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, scale, -((float)Math.PI / 2), 0, rotation, graphics)
        {
            LoadModel("../../models/caroof_nowheel.obj", "../../textures/Green.png", 1, false);

            wheel = new Wheel(new Vector3(0,0,0), scale, 0, graphics);
            mass = 400;
            steering = 0;
            engine_force = 0;
            breaking_force = 0;
        }

        public override void HandleInput(Vector2 mouse, float dt)
        {
            if (CH3.Input.IsKeyActive(Key.D))
            {
                steering -= dt * steering_increment;
                if (steering < -steering_clamp)
                    steering = -steering_clamp;
            } else if (CH3.Input.IsKeyActive(Key.A))
            {
                steering += dt * steering_increment;
                if (steering > steering_clamp)
                    steering = steering_clamp;
            }
            else
            {
                steering *= (0.9f - dt);
            }

            if (CH3.Input.IsKeyActive(Key.W))
            {
                    engine_force = engine_force_max;
            }
            else
            {
                engine_force = 0;
            }

            if (CH3.Input.IsKeyActive(Key.S))
            {
                engine_force = -engine_force_max * 0.5f ;
            }

            if (CH3.Input.IsKeyActive(Key.Space))
            {
                breaking_force = breaking_force_max;
            } else
            {
                breaking_force = 0;
            }

            //engine_force *= (1.0f - dt);

            vehicle.ApplyEngineForce(engine_force, 0);
            vehicle.SetBrake(breaking_force, 2);
            vehicle.ApplyEngineForce(engine_force, 1);
            vehicle.SetBrake(breaking_force, 3);

            vehicle.SetSteeringValue(steering, 2);
            vehicle.SetSteeringValue(steering, 3);
        }
    }
}
