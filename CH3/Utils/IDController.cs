using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    class IDController
    {
        private int highest = -1;
        private HashSet<int> ids;
        private HashSet<int> available;

        public IDController()
        {
            ids = new HashSet<int>();
            available = new HashSet<int>();
        }

        public bool IsOccupied(int id)
        {
            return ids.Contains(id);
        }
        public int Next()
        {
            int id;
            if (available.Count > 0)
            {
                id = available.ToList<int>()[0];
                available.Remove(id);
                if (id > highest)
                {
                    highest = id;
                }
            } else
            {
                highest++;
                id = highest;
            }
            ids.Add(id);
            return id;
        }

        public void Add(int id)
        {
            if (!ids.Contains(id))
            {
                ids.Add(id);
                available.Remove(id);
                if (id > highest)
                {
                    highest = id;
                }
            } else
            {
                throw new ArgumentException($"No!! ID: {id} is occupied!");
            }
        }

        public bool Remove(int id)
        {
            if (!ids.Contains(id))
            {
                return false;
            } else
            {
                ids.Remove(id);
                available.Add(id);
                if (id == highest)
                {
                    //TODO: Detta är en asdålig lösning, kom på nåt bättre!
                    List<int> list = ids.ToList<int>();
                    list.Sort();
                    highest = list.Last<int>();
                }
                return true;
            }
        }
    }
}
