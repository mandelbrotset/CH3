using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3
{
    class CollisionHandler
    {
        List<GameObject> gameObjects;
        public CollisionHandler (List<GameObject> gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        public void HandleCollisions()
        {
            throw new NotImplementedException();
        }

        private float Distance(GameObject obj1, GameObject obj2)
        {
            throw new NotImplementedException();
        }

        private bool IsNear(GameObject obj1, GameObject obj2)
        {
            if (Distance(obj1, obj2) < obj1.GetRadius() + obj2.GetRadius())
            {
                return true;
            } else
            {
                return false;
            }
        }
        
        private bool IsColliding(GameObject obj1, GameObject obj2)
        {
            if (IsNear(obj1, obj2))
            {
                throw new NotImplementedException();
            } else
            {
                return false;
            }
        }

        private void HandleConflict(GameObject obj1, GameObject obj2)
        {
            throw new NotImplementedException();
        }


    }
}
