using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public class Vec2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vec2(Vec2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        public Vec2(Tuple<double, double> v)
        {
            X = v.Item1;
            Y = v.Item2;
        }

        public Vec2(System.Windows.Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public Vec2(Pair<double, double> v)
        {
            X = v.X;
            Y = v.Y;
        }

        public Vec2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 Zero()
        {
            return new Vec2(0, 0);
        }

        public double getLength()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double Cross(Vec2 v2)
        {
            return X * v2.Y - Y * v2.X;
        }

        public double Dot(Vec2 v2)
        {
            return X * v2.X + Y * v2.Y;
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

        public static Vec2 operator *(double mult, Vec2 v)
        {
            return new Vec2(v.X * mult, v.Y * mult);
        }

        public static Vec2 operator /(Vec2 vec, double val)
        {
            return new Vec2(vec.X / val, vec.Y / val);
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}
