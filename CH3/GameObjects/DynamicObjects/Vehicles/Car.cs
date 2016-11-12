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

        private float engine_force;
        private float breaking_force;
        private float steering;
        private const float steering_increment = 0.25f;
        private const float engine_force_max = 10000f;
        private const float breaking_force_max = 200;
        private const float steering_clamp = 1.0f;


        public Car(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, scale, -((float)Math.PI / 2), 0, rotation, graphics)
        {
            LoadModel("../../models/caroof.obj", "../../textures/klccbake.jpg", 1, false);
            mass = 800;
            steering = 0;
            engine_force = 0;
            breaking_force = 0;
        }

        public override void HandleInput(Vector2 mouse, float dt)
        {
            if (CH3.Input.IsKeyActive(Key.A))
            {
                steering -= dt * steering_increment;
                if (steering < -steering_clamp)
                    steering = -steering_clamp;
            }
            else if ((steering + float.Epsilon) < 0)
            {
                steering += dt * steering_increment;
            }


            if (CH3.Input.IsKeyActive(Key.D))
            {
                steering += dt * steering_increment;
                if (steering > steering_clamp)
                    steering = steering_clamp;
            }
            else if ((steering - float.Epsilon) > 0)
            {
                steering -= dt * steering_increment;
            }

            if (CH3.Input.IsKeyActive(Key.W))
            {
                if(vehicle.CurrentSpeedKmHour < 50)
                    engine_force = engine_force_max;
            }
            else
            {
                engine_force = 0;
                breaking_force = breaking_force_max;
            }

            if (CH3.Input.IsKeyActive(Key.S))
            {
                engine_force = -engine_force_max;
            }

            if (CH3.Input.IsKeyActive(Key.Space))
            {
                breaking_force = breaking_force_max;
            }

            if (CH3.Input.IsKeyActive(Key.Space))
            {
                breaking_force = 0;
                
            } else
            {


            }
            engine_force *= (1.0f - dt);
            
            vehicle.ApplyEngineForce(engine_force, 2);
            vehicle.SetBrake(breaking_force, 2);
            vehicle.ApplyEngineForce(engine_force, 3);
            vehicle.SetBrake(breaking_force, 3);

            vehicle.SetSteeringValue(steering, 0);
            vehicle.SetSteeringValue(steering, 1);
        }
    }
}
