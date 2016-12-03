using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Lights
{
    public struct LightDataStruct
    {
        /*
            Must be the same order and size as in shader. And also a multiple of 4.
        */
        Vector3 position;
        float   range;

        Vector3 color;
        int     type;

        Vector3 direction;
        float   angle;

        public LightDataStruct(int i)
        {
            position = new Vector3(0);
            color = new Vector3(0);
            direction = new Vector3(0);
            range = 0;
            type = 0;
            angle = 0;
        }

        public LightDataStruct(Light light)
        {
            position = new Vector3(light.position);
            range = light.range;
            color = new Vector3(light.color);
            type = (int)light.type;
            direction = new Vector3(light.direction);
            angle = light.angle;
        }
    };
}
