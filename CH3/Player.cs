using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class Player : DynamicObject
    {
        private Dictionary<byte, GameObject.Input> controls;

        public Player(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, scale, rotation, shader, normalShader, celShader, depthShader)
        {
            inputs.Add(Input.FORWARD, false);
            inputs.Add(Input.BACKWARD, false);
            inputs.Add(Input.LEFT, false);
            inputs.Add(Input.RIGHT, false);
            controls = new Dictionary<byte, Input>();
            SetControls();
            CH3.Input.SubscribeKeyDown(KeyDown);
            CH3.Input.SubscribeKeyUp(KeyUp);
            CH3.Input.SubscribeMouseMovement(MouseMovement);
            LoadModel("../../models/house.obj", "../../textures/house.png", 1);
        }

        private void MouseMovement(float pitch, float yaw)
        {
            rotationZ = -yaw;
        }

        private void SetControls()
        {
            controls.Add(119, GameObject.Input.FORWARD);//w
            controls.Add(115, GameObject.Input.BACKWARD);//s
            controls.Add(97, GameObject.Input.LEFT);//a
            controls.Add(100, GameObject.Input.RIGHT);//d
        }
        private void KeyDown(byte key, int x, int y)
        {
            GameObject.Input input;
            if (controls.TryGetValue(key, out input))
            {
                ActivateInput(input);
            }
        }

        private void KeyUp(byte key, int x, int y)
        {
            GameObject.Input input;
            if (controls.TryGetValue(key, out input))
            {
                DeactivateInput(input);
            }
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
            //Console.WriteLine(translation + " : " + position);
        }

        public void ActivateInput(Input input)
        {
            HandleActivate(input);
        }

        public void DeactivateInput(Input input)
        {
            HandleDeactivate(input);
        }

    }
}