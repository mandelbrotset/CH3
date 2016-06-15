using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Map
{
    public class Road
    {
        public RoadNode fromNode { get; private set; }
        public RoadNode toNode { get; private set; }
        public int id { get; private set; }

        public Road(int id, RoadNode fromNode, RoadNode toNode)
        {
            this.id = id;
            this.fromNode = fromNode;
            this.toNode = toNode;
        }
      
    }
}
