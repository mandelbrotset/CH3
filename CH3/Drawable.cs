using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGL;

namespace CH3
{
    public interface Drawable
    {

        void render(int time, Matrix4 projectionMatrix, Matrix4 viewMatrix);
    }
}