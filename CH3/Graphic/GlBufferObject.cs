using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace CH3.Graphic
{
    public class GlBufferObject<T> where T : struct
    {
        public uint m_id { get; private set; }
        public int m_elements { get; private set; }
        public BufferUsageHint m_dataUpdateKind { get; private set; }

        public GlBufferObject()
        {
            this.m_id = 0;
            this.m_elements = 0;
        }

        public void init(int elements, T[] hostData, BufferUsageHint dataUpdateKind = BufferUsageHint.DynamicCopy)
        {
            m_dataUpdateKind = dataUpdateKind;

            m_id = (uint)GL.GenBuffer();
            m_elements = elements;
            if (elements > 0)
            {
                copyFromHost(hostData, elements);
            }
        }

        public void copyFromHost(T[] hostData, int elements)
        {
            m_elements = elements;
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_id);
            int size = Marshal.SizeOf(typeof(T));
            GL.BufferData<T>(BufferTarget.ArrayBuffer, new IntPtr(m_elements * size), hostData, m_dataUpdateKind);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void bindSlot(BufferTarget target, uint slot)
        {
            GL.BindBufferBase(target, slot, m_id);
        }

    }
}
