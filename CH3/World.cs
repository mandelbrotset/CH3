using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CH3
{
    public class World
    {
        public Map.Map Map { get; private set; }

        public List<GameObject> staticObjects { get; private set; }
        public List<GameObject> dynamicObjects { get; private set; }
        public List<GameObject> allObjects { get; private set; }


        public World(Map.Map map)
        {
            Console.WriteLine("Init World");

            staticObjects = new List<GameObject>();
            dynamicObjects = new List<GameObject>();
            allObjects = new List<GameObject>();


        }

        public void Tick() {
            foreach (DynamicObject obj in dynamicObjects) {
                obj.Tick();
            }
        }

        public void AddObject(GameObject obj) {

            allObjects.Add(obj);

            if (obj is StaticObject)
            {
                Console.WriteLine("ADding static object");
                staticObjects.Add(obj);
            }
            else if (obj is DynamicObject) {
                Console.WriteLine("Adding dynamic object");
                dynamicObjects.Add(obj);
            }
        }

    }
}