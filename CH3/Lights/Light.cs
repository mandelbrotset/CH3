using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CH3.Lights
{
    public abstract class Light
    {
        public Vector3  position { get; set; }
        public Vector3  direction { get; set; }
        public Vector3  color { get; set; }
        public float    range { get; set; }
        public float    angle { get; set; }

        public Type     type { get; protected set; }

        public enum Type
        {
            PointLight,
            SpotLight,
            DirectionalLight
        }
        
        public Light()
        {
            position = new Vector3(0f);
            direction = new Vector3(0f);
            color = new Vector3(0f);
            range = 0f;
            angle = 0f;
        }
    }
}
