using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class FarmHouse : Building
    {
        public FarmHouse(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, 1*scale, -((float)Math.PI/2), 0, rotation, graphics)
        {
            LoadModel("../../models/farmhouse.obj", "../../textures/farmhouse.jpg", 1);
        }
    }
}
