using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Map
{
    class Path
    {
        //In graph theory, a path in a graph is a finite or infinite sequence of edges which connect a sequence of vertices which,
        //by most definitions, are all distinct from one another.
        public LinkedList<Tuple<Road, RoadNode>> sequence;

        public Path()
        {
            sequence = new LinkedList<Tuple<Road, RoadNode>>();
        }

        public void AddLast(Road road, RoadNode roadNode)
        {
            sequence.AddLast(new LinkedListNode<Tuple<Road, RoadNode>>(new Tuple<Road, RoadNode>(road, roadNode)));
        }

        public void AddFirst(Road road, RoadNode roadNode)
        {
            sequence.AddFirst(new LinkedListNode<Tuple<Road, RoadNode>>(new Tuple<Road, RoadNode>(road, roadNode)));
        }

        public float GetLength()
        {
            float length = 0;
            foreach (Tuple<Road, RoadNode> step in sequence)
            {
                Road road = step.Item1;
                length += road.length;
            }
            return length;
        }

        public float GetSteps()
        {
            return sequence.Count;
        }

        public Tuple<Road, RoadNode> GetNext()
        {
            return sequence.First.Value;
        }

        public Tuple<Road, RoadNode> Pop()
        {
            Tuple<Road, RoadNode> step = sequence.First();
            sequence.RemoveFirst();
            return step;
        }
        public float GetBirdWay()
        {
            return (sequence.First.Value.Item2.position - sequence.Last.Value.Item2.position).Length;
        }
    }
}
