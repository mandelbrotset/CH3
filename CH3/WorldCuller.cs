using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3
{
    public class WorldCuller
    {
        World world;
        Graphics graphics;

        public WorldCuller (World world, Graphics graphics) {
            this.world = world;
            this.graphics = graphics;
        }

        public void Init()
        {
        }

        public void CullWorld()
        {

//            world.visibleObjects.Clear();
 //           world.visibleObjects.AddRange(world.allObjects);
/*
            if (graphics.cameraMode == Graphics.CameraMode.PLAYER) { 
                world.visibleObjects.Clear();
                Vector4 cam = graphics.aboveCamera.visible;
                foreach (GameObject obj in world.allObjects)
                {
                    if (((obj.obb.m_min[0].position < cam.x && obj.obb.m_max[0].position > cam.x) ||
                         (obj.obb.m_min[0].position > cam.x && obj.obb.m_max[0].position < cam.y) ||
                         (obj.obb.m_min[0].position < cam.y && obj.obb.m_max[0].position > cam.y)) &&
                        ((obj.obb.m_min[1].position < cam.z && obj.obb.m_max[1].position > cam.z) ||
                         (obj.obb.m_min[1].position > cam.z && obj.obb.m_max[1].position < cam.w) ||
                         (obj.obb.m_min[1].position < cam.w && obj.obb.m_max[1].position > cam.w)))
                    {
                        world.visibleObjects.Add(obj);
                    }
                }
            } else
            {
                // inte så vackert
                world.visibleObjects.Clear();
                world.visibleObjects.AddRange(world.allObjects);
            }

            */
            /*
            for (int i = 1; i < x.Count; ++i) {
                EndPoint e = x[i];
                int j = i - 1;
                while(j >= 0 && x[j].position > e.position) {
                    x[j + 1] = x[j];
                    j = j - 1;
                }
                x[j + 1] = e;
                
                if (x[j].obb.Equals(x[j+1].obb))
                {
                    Console.WriteLine("Sammasamma");
                }
            }*/

            /*

            obj.min < cam.min && obj.max > cam.min // Till vänster
            obj.min > cam.min && obj.max < cam.max // Inuti
            obj.min < cam.max && obj.max > cam.max // Till höger

            */
        }
    }
}
