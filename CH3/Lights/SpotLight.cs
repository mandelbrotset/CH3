using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CH3.Lights
{
   
    public class SpotLight : Light
    {
        public SpotLight(Vector3 position, Vector3 color, float range, float angle, Vector3 direction) : base()
        {
            this.position = position;
            this.color = color;
            this.range = range;
            this.angle = angle;
            this.direction = direction.Normalized();
            this.type = Type.SpotLight;
        }
    }
}
