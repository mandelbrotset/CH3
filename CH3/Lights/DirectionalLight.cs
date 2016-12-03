using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CH3.Lights
{
   
    public class DirectionalLight : Light
    {
        public DirectionalLight(Vector3 color, Vector3 direction) : base()
        {
            this.color = color;
            this.direction = direction.Normalized();
            this.type = Type.DirectionalLight;
        }
    }
}
