﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class Player : DynamicObject
    {

        public Player(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, scale, rotation, shader, normalShader, celShader, depthShader)
        {
            inputs.Add(Input.FORWARD, false);
            inputs.Add(Input.BACKWARD, false);
            inputs.Add(Input.LEFT, false);
            inputs.Add(Input.RIGHT, false);
            LoadModel("../../models/house.obj", "../../textures/house.png", 1);//borde ändra detta sen
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
        }

        protected void ActivateInput(Input input)
        {
            HandleActivate(input);
        }

        protected void DeactivateInput(Input input)
        {
            HandleDeactivate(input);
        }

    }
}