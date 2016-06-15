using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
    public class House : Building
    {
        public House(Vector3 position, Vector3 scale, float rotation, Graphics graphics) : base(position, 10 * scale, -((float)Math.PI / 2), 0, rotation, graphics)
        {
            LoadModel("../../models/house.obj", "../../textures/house.png", 1);
        }
    }
}
