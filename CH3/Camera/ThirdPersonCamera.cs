﻿using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Camera
{
    public abstract class ThirdPersonCamera : Camera
    {
        public GameObject follow { get; set; }
    }
}
