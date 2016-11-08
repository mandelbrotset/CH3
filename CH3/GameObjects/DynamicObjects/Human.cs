using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;
using static SFML.Window.Keyboard;

namespace CH3
{
    public class Human : Player
    {
        private Dictionary<SFML.Window.Keyboard.Key, GameObject.Input> controls;
        public Human(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, scale, rotation, graphics)
        {
            SetControls();
            CH3.Input.SubscribeKeyPressed(KeyDown);
            CH3.Input.SubscribeKeyUp(KeyUp);
            CH3.Input.SubscribeMouseMovement(MouseMovement);
        }

        private void MouseMovement(float pitch, float yaw)
        {
            rotationZ = -yaw;
        }

        private void SetControls()
        {
            /*
            controls = new Dictionary<SFML.Window.Keyboard.Key, Input>();
            controls.Add(Key.W, GameObject.Input.FORWARD);//w
            controls.Add(Key.S, GameObject.Input.BACKWARD);//s
            controls.Add(Key.A, GameObject.Input.LEFT);//a
            controls.Add(Key.D, GameObject.Input.RIGHT);//d
            */
        }

        private void KeyDown(Key key)
        {
            GameObject.Input input;
            if (controls.TryGetValue(key, out input))
            {
                ActivateInput(input);
            }
        }

        private void KeyUp(Key key)
        {
            GameObject.Input input;
            if (controls.TryGetValue(key, out input))
            {
                DeactivateInput(input);
            }
        }
    }
}