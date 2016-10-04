using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public static class Common
    {
        public enum RadiusMode
        {
            Min, Avg, Max
        }

        public static double getRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double getDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static Vec2 rotateVector(Vec2 vect, double radians)
        {
            return new Vec2(Math.Cos(radians) * vect.X - Math.Sin(radians) * vect.Y,
                            Math.Sin(radians) * vect.X + Math.Cos(radians) * vect.Y);
        }
    }
}
