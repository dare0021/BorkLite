using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Bork.Helpers
{
    public static class Common
    {
        public static bool displayBoundingBox = true;

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

        public static LineSegment generateLineSegment(Vec2 a, Vec2 b)
        {
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(a.X,a.Y);

            LineSegment myLineSegment = new LineSegment();
            myLineSegment.Point = new Point(b.X,b.Y);

            return myLineSegment;
        }

        public static double getAngleBetween(Vec2 a, Vec2 b)
        {
            // X and Y positions inverted since we're using heading (CW)
            // when Math.Atan2() assumes CCW
            return Common.getDegrees(Math.Atan2(b.X - a.X, b.Y - a.Y));
        }
    }
}
