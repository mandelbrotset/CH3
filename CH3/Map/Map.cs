using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH3.Map
{
    public class Map
    {
        public Dictionary<int, RoadNode> roadNodes { get; private set; }
        public Dictionary<int, Road> roads { get; private set; }


    }
}