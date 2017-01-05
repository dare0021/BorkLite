using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public class Radian
    {
        double val;

        public Radian(double v)
        {
            val = v;
        }

        public static Radian operator +(Radian tis, double oth)
        {
            return new Radian(tis.val + oth);
        }

        public static Radian operator -(Radian tis, double oth)
        {
            return new Radian(tis.val - oth);
        }

        public static Radian operator *(Radian tis, double oth)
        {
            return new Radian(tis.val * oth);
        }

        public static Radian operator *(double oth, Radian tis)
        {
            return new Radian(tis.val * oth);
        }

        public static implicit operator Degree(Radian v) { return new Degree(v.val * 180.0 / Math.PI); }

        public static implicit operator double(Radian v) { return v.val; }
    }
}
