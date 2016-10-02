using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public class Pair<T> 
    {
        public T X { get; set; }
        public T Y { get; set; }

        public Pair(Tuple<T,T> v)
        {
            X = v.Item1;
            Y = v.Item2;
        }

        public Pair(T x, T y)
        {
            X = x;
            Y = y;
        }
    }
}
