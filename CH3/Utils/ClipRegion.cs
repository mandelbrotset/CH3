using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CH3.Utils
{
    public static class ClipRegion
    {

        private static void updateClipRegionRoot(float nc, float lc, float lz, float lightRadius, float cameraScale, ref float clipMin, ref float clipMax)
        {
            float nz = (lightRadius - nc * lc) / lz;
            float pz = (lc * lc + lz * lz - lightRadius * lightRadius) / (lz - (nz / nc) * lc);

            if (pz < 0.0f)
            {
                float c = -nz * cameraScale / nc;
                if (nc < 0.0f)
                {
                    clipMin = clipMin < c ? c : clipMin;
                }
                else
                {
                    clipMax = clipMax > c ? c : clipMax;
                }
            }
        }

        private static void updateClipRegion(float lc, float lz, float lightRadius, float cameraScale, ref float clipMin, ref float clipMax)
        {
            float rSq = lightRadius * lightRadius;
            float lcSqPluslzSq = lc * lc + lz * lz;
            float d = rSq * lc * lc - lcSqPluslzSq * (rSq - lz * lz);

            if (d >= 0.0f)
            {
                float a = lightRadius * lc;
                float b = (float)Math.Sqrt(d);
                float nx0 = (a + b) / lcSqPluslzSq;
                float nx1 = (a - b) / lcSqPluslzSq;

                updateClipRegionRoot(nx0, lc, lz, lightRadius, cameraScale, ref clipMin, ref clipMax);
                updateClipRegionRoot(nx1, lc, lz, lightRadius, cameraScale, ref clipMin, ref clipMax);
            }
        }

        public static Vector4 computeClipRegion(Vector3 lightPosView, float lightRadius, float cameraNear, Matrix4 projection)
        {
            Vector4 clipRegion = new Vector4(1.0f, 1.0f, -1.0f, -1.0f);
            if (lightPosView.Z - lightRadius <= -cameraNear)
            {
                Vector2 clipMin = new Vector2(-1.0f, -1.0f);
                Vector2 clipMax = new Vector2(1.0f, 1.0f);

                updateClipRegion(lightPosView.X, lightPosView.Z, lightRadius, projection.M11, ref clipMin.X, ref clipMax.X);
                updateClipRegion(lightPosView.Y, lightPosView.Z, lightRadius, projection.M22, ref clipMin.Y, ref clipMax.Y);

                clipRegion = new Vector4(clipMin.X, clipMin.Y, clipMax.X, clipMax.Y);
            }
            return clipRegion;
        }
    }
}
