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
        public HashSet<Road> outRoads { get; private set; }
        public HashSet<Road> inRoads { get; private set; }
        public RoadNode(int id, Vector3 position)
        {
            this.id = id;
            this.position = position;
            outRoads = new HashSet<Road>();
            inRoads = new HashSet<Road>();
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null) return false;
            if (obj == this)
            {
                return true;
            }
            if (obj is RoadNode)
            {
                Console.WriteLine("Two roadnodes have the same id: " + this.id);
                RoadNode r = (RoadNode)obj;
                return r.id == this.id;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.id;
        }
    }
}
