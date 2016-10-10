using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public class Pair<T, U>
    {
        public T X { get; set; }
        public U Y { get; set; }

        public Pair(Tuple<T, U> v)
        {
            X = v.Item1;
            Y = v.Item2;
        }

        public Pair(T x, U y)
        {
            X = x;
            Y = y;
        }
    }
}