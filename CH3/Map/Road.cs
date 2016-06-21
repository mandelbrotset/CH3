using OpenGL;
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
        public float length { get; private set; }
        private Vector3 position;//remove when Drawable
        private float rotation;//remove when Drawable

        public Road(int id, RoadNode fromNode, RoadNode toNode)
        {
            this.id = id;
            this.fromNode = fromNode;
            this.toNode = toNode;
            Vector3 difference = toNode.position - fromNode.position;
            position = (difference / 2) + fromNode.position;
            length = difference.Length;
            rotation = fromNode.position.CalculateAngle(toNode.position);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null) return false;
            if (obj == this)
            {
                return true;
            }
            if (obj is Road)
            {
                Console.WriteLine("Two roads have the same id: " + this.id);
                Road r = (Road)obj;
                return r.id == this.id;                
            } else
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
