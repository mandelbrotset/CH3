using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace CH3
{
   
    public class DirectionalLight
    {
        
        public Vector3 direction { get; set; }

        public DirectionalLight(Vector3 direction) {
            this.direction = direction;
        }
    }
}
