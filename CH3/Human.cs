using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public class Human : Player
    {
        private Dictionary<byte, GameObject.Input> controls;
        public Human(Vector3 position, Vector3 scale, float rotation, BasicShaderProgram shader, NormalShader normalShader, CelShader celShader, DepthShader depthShader) : base(position, scale, rotation, shader, normalShader, celShader, depthShader)
        {
            SetControls();
            CH3.Input.SubscribeKeyDown(KeyDown);
            CH3.Input.SubscribeKeyUp(KeyUp);
            CH3.Input.SubscribeMouseMovement(MouseMovement);
        }

        private void MouseMovement(float pitch, float yaw)
        {
            rotationZ = -yaw;
        }

        private void SetControls()
        {
            controls = new Dictionary<byte, Input>();
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
    }
}