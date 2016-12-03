using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using CH3.Utils;
using CH3.Lights;

namespace CH3.Graphic
{
    public class LightGrid
    {
        struct ScreenRect
        {
            public Vector2i min, max;

            public ScreenRect(Vector2i min, Vector2i max)
            {
                this.min = min;
                this.max = max;
            }
        }

        private int LIGHT_GRID_TILE_DIM_X;
        private int LIGHT_GRID_TILE_DIM_Y;
        public  int LIGHT_GRID_MAX_DIM_X { get; private set; }
        public  int LIGHT_GRID_MAX_DIM_Y { get; private set; }

        List<ScreenRect> m_screenRects;
        public List<Light> m_viewSpaceLights { get; private set; }
        public List<uint> m_gridOffsets { get; private set; }
        public List<uint> m_gridCounts { get; private set; }
        public List<int> m_tileLightIndexLists { get; private set; }

        public Vector2i m_gridDim;

        public int m_tileSize { get; private set; }

        private Vector2i m_resolution;

        public LightGrid(uint WIDTH, uint HEIGHT, int TILE_SIZE)
        {
            m_tileSize = TILE_SIZE;
            m_resolution = new Vector2i(WIDTH, HEIGHT);
            m_screenRects = new List<ScreenRect>();
            m_viewSpaceLights = new List<Light>();

            m_tileLightIndexLists = new List<int>();

            init(WIDTH, HEIGHT, TILE_SIZE);
        }

        public void init(uint WIDTH, uint HEIGHT, int TILE_SIZE)
        {
            LIGHT_GRID_TILE_DIM_X = TILE_SIZE;
            LIGHT_GRID_TILE_DIM_Y = TILE_SIZE;
            LIGHT_GRID_MAX_DIM_X = (((int)WIDTH  + LIGHT_GRID_TILE_DIM_X - 1) / LIGHT_GRID_TILE_DIM_X);
            LIGHT_GRID_MAX_DIM_Y = (((int)HEIGHT + LIGHT_GRID_TILE_DIM_Y - 1) / LIGHT_GRID_TILE_DIM_Y);

            m_gridOffsets = new List<uint>((int)(LIGHT_GRID_MAX_DIM_X * LIGHT_GRID_MAX_DIM_Y));
            m_gridCounts = new List<uint>((int)(LIGHT_GRID_MAX_DIM_X * LIGHT_GRID_MAX_DIM_Y));

            for (uint i = 0; i < LIGHT_GRID_MAX_DIM_X * LIGHT_GRID_MAX_DIM_Y; ++i)
            {
                m_gridOffsets.Add(0);
                m_gridCounts.Add(0);
            }
        }

        public void build(List<Light> lights, Matrix4 modelView, Matrix4 projection, float near)
        {

            m_gridDim = (m_resolution + m_tileSize - 1) / m_tileSize;

            buildRects(lights, modelView, projection, near);

            int size = LIGHT_GRID_MAX_DIM_X * LIGHT_GRID_MAX_DIM_Y;
            for (int i = 0; i < size; ++i)
            {
                m_gridOffsets[i] = 0;
                m_gridCounts[i] = 0;
            }

            int num_tiles = 0;
            for (int i = 0; i < m_screenRects.Count; ++i)
            {
                ScreenRect r = m_screenRects[i];
                Light light = m_viewSpaceLights[i];

                Vector2i l = Vector2i.clamp(r.min / m_tileSize, 0, m_gridDim + 1);
                Vector2i u = Vector2i.clamp((r.max + m_tileSize - 1) / m_tileSize, 0, m_gridDim + 1);

                for (int y = l.Y; y < u.Y; ++y)
                {
                    for (int x = l.X; x < u.X; ++x)
                    {
                        m_gridCounts[getIndex(x, y)] += 1;
                        ++num_tiles;
                    }
                }
            }

            int num_elements = Math.Max(m_tileLightIndexLists.Count - num_tiles, 0);
            m_tileLightIndexLists.RemoveRange(m_tileLightIndexLists.Count - num_elements, num_elements);
            for (int i = m_tileLightIndexLists.Count; i < num_tiles; ++i)
                m_tileLightIndexLists.Add(0);

            uint offset = 0;

            for (int y = 0; y < m_gridDim.Y; ++y)
            {
                for (int x = 0; x < m_gridDim.X; ++x)
                {
                    uint count = m_gridCounts[getIndex(x, y)];

                    m_gridOffsets[getIndex(x, y)] = offset + count;
                    offset += count;
                }
            }

            if (m_tileLightIndexLists.Count > 0)
            {
                for (int i = 0; i < m_screenRects.Count; ++i)
                {
                    uint lightId = (uint)i;

                    Light light = m_viewSpaceLights[i];
                    ScreenRect r = m_screenRects[i];

                    Vector2i l = Vector2i.clamp(r.min / m_tileSize, 0, m_gridDim + 1);
                    Vector2i u = Vector2i.clamp((r.max + m_tileSize - 1) / m_tileSize, 0, m_gridDim + 1);


                    for (int y = l.Y; y < u.Y; ++y)
                    {
                        for (int x = l.X; x < u.X; ++x)
                        {
                            int index = (int)m_gridOffsets[getIndex(x, y)] - 1;
                            m_tileLightIndexLists[index] = (int)lightId;
                            m_gridOffsets[getIndex(x, y)] = (uint)index;
                        }
                    }
                }
            }
        }

        public void buildRects(List<Light> lights, Matrix4 modelView, Matrix4 projection, float near)
        {
            m_viewSpaceLights.Clear();
            m_screenRects.Clear();

            for (int i = 0; i < lights.Count; ++i)
            {
                Light l = lights[i];

                if(l.type == Light.Type.PointLight) { 
                    Vector3 position = MathOpenTK.transformPoint(modelView, l.position);
                    ScreenRect rect = findScreenSpaceBounds(projection, position, l.range, m_resolution.X, m_resolution.Y, near);
                    if (rect.min.X < rect.max.X && rect.min.Y < rect.max.Y) {
                        m_screenRects.Add(rect);
                        m_viewSpaceLights.Add(new PointLight(position, l.color, l.range));
                    }
                } else if (l.type == Light.Type.SpotLight) {
                    Vector3 offset = MathOpenTK.transformPoint(modelView, l.position + l.direction * l.range * 0.5f);

                    Vector3 position = MathOpenTK.transformPoint(modelView, l.position);
                    ScreenRect rect = findScreenSpaceBounds(projection, offset, l.range, m_resolution.X, m_resolution.Y, near);

                    Vector3 direction = MathOpenTK.transformDirection(modelView, l.direction);
                    Vector3 direction2 = MathOpenTK.transformDirection(modelView, new Vector3(1,0,0));

                    if (rect.min.X < rect.max.X && rect.min.Y < rect.max.Y)
                    {
                        m_screenRects.Add(rect);
                        m_viewSpaceLights.Add(new SpotLight(position, l.color, l.range, l.angle, MathOpenTK.transformDirection(modelView, l.direction)));
                    }
                } else if(l.type == Light.Type.DirectionalLight) {
                    m_screenRects.Add(new ScreenRect(new Vector2i(0,0), new Vector2i(Window.WIDTH, Window.HEIGHT)));
                    m_viewSpaceLights.Add(new DirectionalLight(l.color, MathOpenTK.transformDirection(modelView, l.direction)));
                }
            }
        }

        private ScreenRect findScreenSpaceBounds(Matrix4 projection, Vector3 pt, float rad, float width, float height, float near)
        {
            Vector4 reg = ClipRegion.computeClipRegion(pt, rad, near, projection);

            reg = -reg;

            float tmp = reg.X;
            reg.X = reg.Z;
            reg.Z = tmp;

            tmp = reg.Y;
            reg.Y = reg.W;
            reg.W = tmp;

            reg *= 0.5f;
            reg += new Vector4(0.5f);

            Vector4 zeros = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            Vector4 ones = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            reg = MathOpenTK.clamp(reg, zeros, ones);

            return new ScreenRect(
                new Vector2i(reg.X * width, reg.Y * height),
                new Vector2i(reg.Z * width, reg.W * height));
        }

        public uint tileLightCount(int x, int y)
        {
            return m_gridCounts[getIndex(x, y)];
        }

        public uint tileLightIndexListOffset(int x, int y)
        {
            return m_gridOffsets[getIndex(x, y)];
        }

        private int getIndex(int x, int y)
        {
            return (int)(x + y * LIGHT_GRID_MAX_DIM_X);
        }
    }
}

