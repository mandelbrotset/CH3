using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CH3.Lights
{
   
    public class PointLight : Light
    {
        public PointLight(Vector3 position, Vector3 color, float range) : base()
        {
            this.position = position;
            this.color = color;
            this.range = range;
            this.type = Type.PointLight;
        }
    }
}
