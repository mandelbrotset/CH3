using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.FreeGlut;

namespace CH3
{
    public class Input
    {
        public delegate void MouseMovement(float pitch, float yaw);
        public delegate void KeyDown(byte key, int x, int y);
        public delegate void KeyUp(byte key, int x, int y);
        public delegate void MouseButton(int button, int state, int x, int y);

        private static ArrayList mouseMovements = new ArrayList();
        private static ArrayList keyDowns = new ArrayList();
        private static ArrayList keyUps = new ArrayList();
        private static ArrayList mouseButtons = new ArrayList();
        private static float pitch, yaw;
        private static bool warped;

        public static void Init()
        {
            Glut.PassiveMotionCallback motionFunc = Motion;
            Glut.glutPassiveMotionFunc(motionFunc);
            Glut.KeyboardCallback keyDownFunc = KeyDownEvent;
            Glut.glutKeyboardFunc(keyDownFunc);
            Glut.KeyboardUpCallback keyUpFunc = KeyUpEvent;
            Glut.glutKeyboardUpFunc(keyUpFunc);
            Glut.MouseCallback mouseFunc = MouseButtonEvent;
            Glut.glutMouseFunc(mouseFunc);
        }

        public static void Motion(int x, int y)
        {
            if (warped)
            {
                warped = false;
                return;
            }

            float dX = ((float)x - Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / 2);
            float dY = ((float)y - Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT) / 2);
            float sense = 0.001f;
            yaw -= dX * sense;
            pitch += dY * sense;
            warped = true;
            Glut.glutWarpPointer(Glut.glutGet(Glut.GLUT_WINDOW_WIDTH) / 2, Glut.glutGet(Glut.GLUT_WINDOW_HEIGHT) / 2);
            foreach (MouseMovement mouseMovement in mouseMovements)
            {
                mouseMovement(pitch, yaw);
            }
        }

        public static void KeyUpEvent(byte key, int x, int y)
        {
            foreach (KeyUp keyUp in keyUps)
            {
                keyUp(key, x, y);
            }
        }

        public static void KeyDownEvent(byte key, int x, int y)
        {
            foreach (KeyDown keyDown in keyDowns)
            {
                keyDown(key, x, y);
            }
        }

        public static void MouseButtonEvent(int button, int state, int x, int y)
        {
            foreach (MouseButton mouseButton in mouseButtons)
            {
                mouseButton(button, state, x, y);
            }
        }

        public static void SubscribeMouseMovement(MouseMovement mouseMovement)
        {
            mouseMovements.Add(mouseMovement);
        }

        public static void UnsubscribeMouseMovement(MouseMovement mouseMovement)
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

        public static void SubscribeKeyDown(KeyDown keyDown)
        {
            keyDowns.Add(keyDown);
        }

        public static void UnsubscribeKeyDown(KeyDown keyDown)
        {
            keyDowns.Remove(keyDown);
        }

        public static void SubscribeKeyUp(KeyUp keyUp)
        {
            keyUps.Add(keyUp);
        }

        public static void UnsubscribeKeyUp(KeyUp keyUp)
        {
            keyUps.Remove(keyUp);
        }
    }
}