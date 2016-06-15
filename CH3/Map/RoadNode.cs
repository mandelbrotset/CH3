using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Map
{
    public class RoadNode
    {
        public Vector3 position { get; private set; }
        public int id { get; private set; }

        public RoadNode(int id, Vector3 position)
        {
            this.id = id;
            this.position = position;
        }
    }
}
