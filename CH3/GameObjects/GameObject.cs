using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ObjLoader.Loader.Loaders;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;
using System.Collections;
using OpenGL;
using BulletSharp;

namespace CH3
{
    public abstract class GameObject : Drawable
    {
        public RigidBody body;

        public int mass { get; protected set; }

        public bool has_physics = false;

        //public Vector3 position { get { return position; } set { position = value; } }
        public enum Input
        {
            FORWARD, BACKWARD, LEFT, RIGHT
        }
        public GameObject inside { get; private set; }
        protected Dictionary<Input, bool> inputs;

        public GameObject(OpenGL.Vector3 position, OpenGL.Vector3 scale, float rotationX, float rotationY, float rotationZ, Graphics graphics) : base(position, scale, rotationX, rotationY, rotationZ, graphics)
        {
            mass = 0;
            inputs = new Dictionary<Input, bool>();
        }

        public float GetRadius()
        {
            throw new NotImplementedException();
        }

        protected virtual void Act(Input input) { }

        public Boolean HandleDeactivate(Input input)
        {
            if (inputs.ContainsKey(input))
            {
                inputs[input] = false;
                Act(input);
                return true;
            }
            else
            {
                if (inside != null)
                {
                    return inside.HandleDeactivate(input);
                }
                else
                {
                    return false;
                }
            }
        }

        public Boolean HandleActivate(Input input)
        {
            if (inputs.ContainsKey(input))
            {
                inputs[input] = true;
                Act(input);
                return true;
            } else
            {
                if (inside != null)
                {
                    return inside.HandleActivate(input);
                } else
                {
                    return false;
                }
            }
        }
    }
}