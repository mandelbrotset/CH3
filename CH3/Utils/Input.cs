using SFML.Window;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH3
{
    public class Input
    {
        public delegate void MouseMoved(float pitch, float yaw);
        public delegate void KeyPressed(Keyboard.Key key);
        public delegate void KeyReleased(Keyboard.Key key);
        public delegate void MouseButton(int button, int state, int x, int y);

        private static ArrayList mouseMovements = new ArrayList();
        private static ArrayList keyPresseds = new ArrayList();
        private static ArrayList keyReleaseds = new ArrayList();
        private static ArrayList mouseButtons = new ArrayList();
        
        private static float pitch, yaw;
        private static bool warped;
        private static Dictionary<Keyboard.Key, bool> activeKeys = new Dictionary<Keyboard.Key, bool>();

        public static bool IsKeyActive(Keyboard.Key key)
        {
            if (!activeKeys.ContainsKey(key))
            {
                return false;
            } else
            {
                return activeKeys[key];
            }
        }

        public static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            Console.Write(e.Code);
            activeKeys[e.Code] = true;
            foreach (KeyPressed keyPressed in keyPresseds)
            {
                keyPressed(e.Code);
            }
        }

        public static void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {

            /*
            SFML.Window.Mouse.SetPosition(new SFML.System.Vector2i((int)VideoMode.DesktopMode.Width / 2, (int)VideoMode.DesktopMode.Height / 2));



            if (warped)
            {
                warped = false;
                return;
            }
            float centerX = VideoMode.DesktopMode.Width / 2;
            float centerY = VideoMode.DesktopMode.Height / 2;
            Console.Write(VideoMode.DesktopMode.Width);
            float dX = ((float)e.X - centerX);
            float dY = ((float)e.Y - centerY);
            float sense = 0.01f;
            yaw -= dX * sense;
            pitch += dY * sense;
            warped = true;

            
            foreach (MouseMoved mouseMoved in mouseMovements)
            {
                mouseMoved(pitch, yaw);
            }
            */
        }

        public static void OnKeyReleased(object sender, KeyEventArgs e)
        {
            activeKeys[e.Code] = false;
            foreach (KeyReleased keyReleased in keyReleaseds)
            {
                keyReleased(e.Code);
            }
        }

        public static void MouseButtonEvent(int button, int state, int x, int y)
        {
            foreach (MouseButton mouseButton in mouseButtons)
            {
                mouseButton(button, state, x, y);
            }
        }

        public static void SubscribeMouseMovement(MouseMoved mouseMovement)
        {
            mouseMovements.Add(mouseMovement);
        }

        public static void UnsubscribeMouseMovement(MouseMoved mouseMovement)
        {
            mouseMovements.Remove(mouseMovement);
        }

        public static void SubscribeMouseButton(MouseButton mouseButton)
        {
            mouseButtons.Add(mouseButton);
        }

        public static void UnsubscribeMouseButton(MouseButton mouseButton)
        {
            mouseButtons.Remove(mouseButton);
        }

        public static void SubscribeKeyPressed(KeyPressed keyPressed)
        {
            keyPresseds.Add(keyPressed);
        }

        public static void UnsubscribeKeyPressed(KeyPressed PressedDown)
        {
            keyPresseds.Remove(PressedDown);
        }

        public static void SubscribeKeyUp(KeyReleased keyUp)
        {
            keyReleaseds.Add(keyUp);
        }

        public static void UnsubscribeKeyUp(KeyReleased keyUp)
        {
            keyReleaseds.Remove(keyUp);
        }
    }
}