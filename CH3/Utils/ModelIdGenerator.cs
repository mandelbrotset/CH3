using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    public class ModelIDGenerator
    {
        private int id;
        private static ModelIDGenerator instance;

        private ModelIDGenerator() {
            id = 0;
        }

        public int GetId() {
            int newId = id + 10;
            if (newId > 255)
                newId = 0;

            int oldId = id;
            id = newId;

            return oldId;
        }

        public static ModelIDGenerator GetInstance() {
            if (instance == null)
                instance = new ModelIDGenerator();

            return instance;
        }

    }
}
