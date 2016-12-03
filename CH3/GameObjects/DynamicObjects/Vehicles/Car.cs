using BulletSharp;
using CH3.Lights;
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


        public SpotLight break_light0 { get; private set; }
        public SpotLight break_light1 { get; private set; }
        public SpotLight head_light0 { get; private set; }
        public SpotLight head_light1 { get; private set; }

        public Car(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, scale, -((float)Math.PI / 2), 0, rotation, graphics)
        {
            LoadModel("../../models/caroof_nowheel.obj", "../../textures/Green.png", 1, false);

            wheel = new Wheel(new Vector3(0,0,0), scale, 0, graphics);
            mass = 400;
            steering = 0;
            engine_force = 0;
            breaking_force = 0;

            break_light0 = new SpotLight(new OpenTK.Vector3(0), new OpenTK.Vector3(1, 0, 0), 4.0f, 1.2f, new OpenTK.Vector3(0));
            break_light1 = new SpotLight(new OpenTK.Vector3(0), new OpenTK.Vector3(1, 0, 0), 4.0f, 1.2f, new OpenTK.Vector3(0));

            head_light0 = new SpotLight(new OpenTK.Vector3(0), new OpenTK.Vector3(1, 1, 1), 100.0f, 0.8f, new OpenTK.Vector3(0));
            head_light1 = new SpotLight(new OpenTK.Vector3(0), new OpenTK.Vector3(1, 1, 1), 100.0f, 0.8f, new OpenTK.Vector3(0));


        }


        public override void UpdateLights() {
            OpenTK.Vector3 break_light0_model_pos = new OpenTK.Vector3(-1.087f, 2.165f - 0.25f, 0.974f) * new OpenTK.Vector3(scale.x, scale.y, scale.z);
            break_light0.position = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(break_light0_model_pos, 1.0f)).Xyz;
            break_light0.direction = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(0, 1, 0, 0.0f)).Xyz;

            OpenTK.Vector3 break_light1_model_pos = new OpenTK.Vector3(1.087f, 2.165f - 0.25f, 0.974f) * new OpenTK.Vector3(scale.x, scale.y, scale.z);
            break_light1.position = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(break_light1_model_pos, 1.0f)).Xyz;
            break_light1.direction = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(0, 1, 0, 0.0f)).Xyz;


            OpenTK.Vector3 head_light0_model_pos = new OpenTK.Vector3(-0.971f, -2.199f + 1.0f, 1.107f) * new OpenTK.Vector3(scale.x, scale.y, scale.z);
            head_light0.position = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(head_light0_model_pos, 1.0f)).Xyz;
            head_light0.direction = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(0, -1, 0, 0.0f)).Xyz;

            OpenTK.Vector3 head_light1_model_pos = new OpenTK.Vector3(0.971f, -2.199f + 1.0f, 1.107f) * new OpenTK.Vector3(scale.x, scale.y, scale.z);
            head_light1.position = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(head_light1_model_pos, 1.0f)).Xyz;
            head_light1.direction = Utils.MathOpenTK.Matrix4Vector4Mul(vehicle.ChassisWorldTransform, new OpenTK.Vector4(0, -1, 0, 0.0f)).Xyz;
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

                break_light0.range = 16.0f;
                break_light1.range = 16.0f;

            }
            else
            {
                break_light0.range = 4.0f;
                break_light1.range = 4.0f;

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
