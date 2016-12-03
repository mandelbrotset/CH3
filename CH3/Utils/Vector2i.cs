using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    public class Vector2i
    {
        public int X, Y;

        public Vector2i()
        {
           
        }

        public Vector2i(OpenTK.Vector2 v)
        {
            this.X = (int)v.X;
            this.Y = (int)v.Y;
        }

        public Vector2i(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Vector2i(float X, float Y)
        {
            this.X = (int)X;
            this.Y = (int)Y;
        }

        public static Vector2i Max(Vector2i v0, Vector2i v1)
        {
            return new Vector2i(v0.X > v1.X ? v0.X : v1.X, v0.Y > v1.Y ? v0.Y : v1.Y);
        }

        public static Vector2i Min(Vector2i v0, Vector2i v1)
        {
            return new Vector2i(v0.X < v1.X ? v0.X : v1.X, v0.Y < v1.Y ? v0.Y : v1.Y);
        }

        public static Vector2i clamp(Vector2i value, Vector2i low, Vector2i high)
        {
            return Min(Max(value, low), high);
        }

        public static Vector2i clamp(Vector2i value, int low, Vector2i high)
        {
            return Min(Max(value, new Vector2i(low,low)), high);
        }

        public static Vector2i clamp(Vector2i value, int low, int high)
        {
            return Min(Max(value, new Vector2i(low, low)), new Vector2i(high, high));
        }

        public static Vector2i clamp(Vector2i value, Vector2i low, int high)
        {
            return Min(Max(value, low), new Vector2i(high, high));
        }

        public static Vector2i operator +(Vector2i v0, Vector2i v1)
        {
            return new Vector2i(v0.X + v1.X, v0.Y + v1.Y);
        }

        public static Vector2i operator +(Vector2i v0, uint i)
        {
            return new Vector2i(v0.X + i, v0.Y + i);
        }

        public static Vector2i operator -(Vector2i v0, uint i)
        {
            return new Vector2i(v0.X - i, v0.Y - i);
        }
        public static Vector2i operator +(Vector2i v0, int i)
        {
            return new Vector2i(v0.X + i, v0.Y + i);
        }

        public static Vector2i operator -(Vector2i v0, int i)
        {
            return new Vector2i(v0.X - i, v0.Y - i);
        }

        public static Vector2i operator /(Vector2i v0, Vector2i v1)
        {
            return new Vector2i(v0.X / v1.X, v0.Y / v1.Y);
        }

        public static Vector2i operator /(Vector2i v0, int i)
        {
            return new Vector2i(v0.X / i, v0.Y / i);
        }

        public static Vector2i operator /(Vector2i v0, uint i)
        {
            return new Vector2i(v0.X / i, v0.Y / i);
        }
    }
}
