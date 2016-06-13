using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Map
{
    public class Road
    {
        public RoadNode n1 { get; private set; }
        public RoadNode n2 { get; private set; }

        public Road(RoadNode n1, RoadNode n2)
        {
            this.n1 = n1;
            this.n2 = n2;
        }
    }
}
