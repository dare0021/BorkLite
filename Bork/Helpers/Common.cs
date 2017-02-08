using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.IO;

namespace Bork.Helpers
{
    public static class Common
    {
        private static ulong nextUID = 0;

        public static bool displayBoundingBox = true;

        /// <summary>
        /// Time in ms where if keyUp() is called within this time keyTyped() is called
        /// </summary>
        public static float keyTypeThresh = 200;

        public enum RadiusMode
        {
            Min, Avg, Max
        }

        public enum AudioState
        {
            Stop, Pause, Play, Done
        }

        /// <summary>
        /// Manual: keeps rotating according to the rotation speed set
        /// Tracking: rotates to face the target at MaxRotationSpeed
        /// </summary>
        public enum RotationMode
        {
            Manual, Tracking, TargetRotation
        }

        public static Vec2 rotateVector(Vec2 vect, Radian radians)
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

        /// <summary>
        /// Finds the heading of the vector (point - pivot) in degrees
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Degree getAngleBetween(Vec2 point, Vec2 pivot)
        {
            // X and Y positions inverted since we're using heading (CW)
            // when Math.Atan2() assumes CCW
            return new Degree(Math.Atan2(point.X - pivot.X, point.Y - pivot.Y) * 180 / Math.PI);
        }

        public static ulong getNewUID()
        {
            var retval = nextUID;
            nextUID++;
            return retval;
        }

        /// <summary>
        /// Correct to the 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DateTime timestampToDateTime(int ts)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(ts).ToLocalTime();
        }

        public static double Sin(Radian x)
        {
            return Math.Sin(x);
        }

        public static double Cos(Radian x)
        {
            return Math.Cos(x);
        }

        public static double Tan(Radian x)
        {
            return Math.Tan(x);
        }

        public static List<string> FileReadAllLines(string path)
        {
            var output = new List<string>();
            using (StreamReader f = new StreamReader(path))
            {
                while (!f.EndOfStream)
                    output.Add(f.ReadLine());
            }
            return output;
        }
    }
}
