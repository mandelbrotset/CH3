using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    public static class MathOpenTK
    {
        public static Vector2 clamp(Vector2 value, Vector2 low, Vector2 high)
        {
            return Vector2.Min(Vector2.Max(value, low), high);
        }

        public static Vector3 clamp(Vector3 value, Vector3 low, Vector3 high)
        {
            return Vector3.Min(Vector3.Max(value, low), high);
        }

        public static Vector4 clamp(Vector4 value, Vector4 low, Vector4 high)
        {
            return Vector4.Min(Vector4.Max(value, low), high);
        }

        public static Vector4 Matrix4Vector4Mul(Matrix4 c, Vector4 v)
        {
            return new Vector4(
                c.M11 * v.X + c.M21 * v.Y + c.M31 * v.Z + c.M41 * v.W,
                c.M12 * v.X + c.M22 * v.Y + c.M32 * v.Z + c.M42 * v.W,
                c.M13 * v.X + c.M23 * v.Y + c.M33 * v.Z + c.M43 * v.W,
                c.M14 * v.X + c.M24 * v.Y + c.M34 * v.Z + c.M44 * v.W);

        }

        public static Vector3 transformPoint(Matrix4 matrix, Vector3 point)
        {
            Vector4 r = Matrix4Vector4Mul(matrix, new Vector4(point, 1.0f));
            return new Vector3(r.Xyz);
        }

        public static Vector3 transformDirection(Matrix4 matrix, Vector3 point)
        {
            Vector4 r = Matrix4Vector4Mul(matrix, new Vector4(point, 0.0f));
            return new Vector3(r.Xyz);
        }
    }
}
