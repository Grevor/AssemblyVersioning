using System;
using System.Collections.Generic;

namespace Viking.AssemblyVersioning
{
    internal static class EqualityComparer
    {
        public static bool IEnumerableEquality<T>(IEnumerable<T> a, IEnumerable<T> b) => IEnumerableEquality(a, b, EqualityComparer<T>.Default.Equals);
        public static bool IEnumerableEquality<T>(IEnumerable<T> a, IEnumerable<T> b, Func<T,T,bool> cmp)
        {
            var aa = a.GetEnumerator();
            var bb = a.GetEnumerator();
            while (true)
            {
                T ae, be;
                if (!aa.MoveNext())
                    return !bb.MoveNext();
                if (!bb.MoveNext())
                    return false;

                ae = aa.Current;
                be = bb.Current;
                if (!cmp(ae, be))
                    return false;
            }
        }



        public static bool Equals(Type a, Type b)
        {
            return a.FullName.Equals(b.FullName);
        }
    }
}
