using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Utils
{
    public class BiDictionary<A, B>
    {
        private Dictionary<A, B> aToB;
        private Dictionary<B, A> bToA;

        public BiDictionary() {
            aToB = new Dictionary<A, B>();
            bToA = new Dictionary<B, A>();
        }

        public void Add(A a, B b)
        {
            aToB.Add(a, b);
            bToA.Add(b, a);
        }

        public bool Get(A a, out B b)
        {
            return aToB.TryGetValue(a, out b);
        }
        public bool Get(B b, out A a)
        {
            return bToA.TryGetValue(b, out a);
        }
        public A Get(B key)
        {
            A a;
            bToA.TryGetValue(key, out a);
            return a;
        }

        public B Get(A key)
        {
            B b;
            aToB.TryGetValue(key, out b);
            return b;
        }
        
        public IEnumerable<T> GetValues<T>()
        {
            if (typeof(T) == typeof(A))
            {
                return aToB.Values.Cast<T>();
            } else if (typeof(T) == typeof(B))
            {
                return bToA.Values.Cast<T>();
            } else
            {
                throw new InvalidOperationException("Weird type..");
            }
        }
       
        public bool Remove(A key)
        {
            B b;
            bool ok = aToB.TryGetValue(key, out b);
            if (!ok) return false;
            aToB.Remove(key);
            bToA.Remove(b);
            return true;
        }
        public bool Remove(B key)
        {
            A a;
            bool ok = bToA.TryGetValue(key, out a);
            if (!ok) return false;
            bToA.Remove(key);
            aToB.Remove(a);
            return true;
        }
        /*public Dictionary<T, V>.ValueCollection GetValues<T, V>()
        {
            if (T == typeof(A))
            {

            }
        }*/

        /* public Dictionary<B, A>.ValueCollection GetValues()
         {
             return bToA.Values;
         }*/


    }
}
