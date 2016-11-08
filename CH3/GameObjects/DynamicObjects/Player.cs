using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;
using static SFML.Window.Keyboard;

namespace CH3
{
    public class Player : DynamicObject
    {

        public Vector3 forward = Vector3.Right;

        public Player(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, 10*scale, -((float)Math.PI/2),0, rotation, graphics)
        {
            /*
            inputs.Add(Input.FORWARD, false);
            inputs.Add(Input.BACKWARD, false);
            inputs.Add(Input.LEFT, false);
            inputs.Add(Input.RIGHT, false);
            */
            LoadModel("../../models/Joda.obj", "../../textures/Joda_D.png", 1);//borde ändra detta sen
        }

        protected override void Act(Input input)
        {
            float x = 0;
            float y = 0;
            if (inputs[Input.FORWARD])
            {
                y = 1;
            }
            else if (inputs[Input.BACKWARD])
            {
                y = -1;
            }
            if (inputs[Input.LEFT])
            {
                x = -1;
            }
            else if (inputs[Input.RIGHT])
            {
                x = 1;
            }
            Vector3 translation = new Vector3(x, y, 0);
            translation *= Matrix4.CreateRotationZ(-rotationZ);
            velocity = translation;
            position = new Vector3(10, 0, 0);
        }

        protected void ActivateInput(Input input)
        {
            HandleActivate(input);
        }

        protected void DeactivateInput(Input input)
        {
            HandleDeactivate(input);
        }

        public void HandleInput(Vector2 mouse)
        {
            float sense = 0.005f;
            float speed = 0.5f;
            Vector3 translation = new Vector3(0, 0, 0);
            Vector3 right = new Vector3(0, 0, 0);

            rotationZ -= (mouse.x) * sense;

            // Tittar Joda i fel axel eller är det fel någon annanstans?
            float joda_offset = (float)Math.PI;

            forward.x = (float)Math.Sin(rotationZ + joda_offset);
            forward.y = (float)Math.Cos(rotationZ + joda_offset);
            right.x = (float)Math.Sin(rotationZ + Math.PI * 0.5f + joda_offset);
            right.y = (float)Math.Cos(rotationZ + Math.PI * 0.5f + joda_offset);

            if (CH3.Input.IsKeyActive(Key.W))
            {
                translation += speed * forward;
            }
            if (CH3.Input.IsKeyActive(Key.A))
            {
                translation -= speed * right;
            }
            if (CH3.Input.IsKeyActive(Key.S))
            {
                translation -= speed * forward;
            }
            if (CH3.Input.IsKeyActive(Key.D))
            {
                translation += speed * right;
            }
            position += translation;
        }
    }
}