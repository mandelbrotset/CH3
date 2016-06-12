﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class Player : DynamicObject
    {

        private Dictionary<byte, GameObject.Input> controls;

        public Player(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader) : base(position, scale, rotation, shader)
        {
            inputs.Add(Input.FORWARD, false);
            inputs.Add(Input.BACKWARD, false);
            inputs.Add(Input.LEFT, false);
            inputs.Add(Input.RIGHT, false);
            controls = new Dictionary<byte, Input>();
            SetControls();
            CH3.Input.SubscribeKeyDown(KeyDown);
            CH3.Input.SubscribeKeyUp(KeyUp);
            LoadModel("../../models/house.obj", "../../textures/house.png");
        }

        private void SetControls()
        {
            controls.Add(119, GameObject.Input.FORWARD);
            controls.Add(115, GameObject.Input.BACKWARD);
            controls.Add(97, GameObject.Input.LEFT);
            controls.Add(100, GameObject.Input.RIGHT);
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
                x = 1;
            }
            else if (inputs[Input.BACKWARD])
            {
                x = -1;
            }
            if (inputs[Input.LEFT])
            {
                y = 1;
            }
            else if (inputs[Input.RIGHT])
            {
                y = -1;
            }
            Vector3 translation = new Vector3(x, y, 0);
            translation *= Matrix4.CreateRotationZ(rotationZ);
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