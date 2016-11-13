using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.GameObjects.DynamicObjects.Vehicles
{
    public class Wheel : DynamicObject
    {
        public Wheel(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, scale, -((float)Math.PI / 2), 0, rotation, graphics) { 
            LoadModel("../../models/wheel.obj", "../../textures/klccbake.jpg", 1, false);
        }
    }
}
