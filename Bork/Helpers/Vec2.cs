using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    class Vec2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vec2(Tuple<double, double> v)
        {
            X = v.Item1;
            Y = v.Item2;
        }

        public Vec2(Pair<double> v)
        {
            X = v.X;
            Y = v.Y;
        }

        public Vec2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 operator +(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vec2 operator -(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vec2 operator *(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static Vec2 operator /(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X / v2.X, v1.Y / v2.Y);
        }

        public static Vec2 operator +(Vec2 vec, double val)
        {
            return new Vec2(vec.X + val, vec.Y + val);
        }

        public static Vec2 operator -(Vec2 vec, double val)
        {
            return new Vec2(vec.X - val, vec.Y - val);
        }

        public static Vec2 operator *(Vec2 vec, double val)
        {
            return new Vec2(vec.X * val, vec.Y * val);
        }

        public static Vec2 operator /(Vec2 vec, double val)
        {
            return new Vec2(vec.X / val, vec.Y / val);
        }
    }
}
