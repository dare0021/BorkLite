using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public class Degree
    {
        double val;

        public Degree(double v)
        {
            val = v;
        }

        public static Degree operator +(Degree tis, double oth)
        {
            return new Degree(tis.val + oth);
        }

        public static Degree operator -(Degree tis, double oth)
        {
            return new Degree(tis.val - oth);
        }

        public static Degree operator *(Degree tis, double oth)
        {
            return new Degree(tis.val * oth);
        }

        public static Degree operator *(double oth, Degree tis)
        {
            return new Degree(tis.val * oth);
        }

        public static implicit operator Radian(Degree v) { return new Radian(v.val * Math.PI / 180.0); }

        public static implicit operator double(Degree v) { return v.val; }
    }
}
