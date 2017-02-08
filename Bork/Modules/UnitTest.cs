using Bork.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Bork.Modules
{
    static class UnitTest
    {
        static bool fail = false;

        /// <summary>
        /// Flags the class's "fail variable" if the input is false
        /// </summary>
        static private bool test(bool testingFunction)
        {
            Debug.Assert(testingFunction);
            return fail || !testingFunction;
        }

        /// <summary>
        /// Convenience function
        /// </summary>
        static private void set4Vec2(out Vec2 a1, out Vec2 a2, out Vec2 b1, out Vec2 b2, double a1x, double a1y, double a2x, double a2y, double b1x, double b1y, double b2x, double b2y)
        {
            a1 = new Vec2(a1x, a1y);
            a2 = new Vec2(a2x, a2y);
            b1 = new Vec2(b1x, b1y);
            b2 = new Vec2(b2x, b2y);
        }

        static public bool run()
        {
            Vec2 a1 = new Vec2(0, 0);
            Vec2 a2 = new Vec2(0, 0);
            Vec2 b1 = new Vec2(0, 0);
            Vec2 b2 = new Vec2(0, 0);
            Vec2 output;

            set4Vec2(out a1, out a2, out b1, out b2, -1, -1, 1, 1, -1, 1, 1, -1);
            test(CollisionDetection.LineSegementsIntersect(a1, a2, b1, b2, out output));
            set4Vec2(out a1, out a2, out b1, out b2, -1, 1, 1, -1, -1, -1, 1, 1);
            test(CollisionDetection.LineSegementsIntersect(a1, a2, b1, b2, out output));
            set4Vec2(out a1, out a2, out b1, out b2, -1, 0, 1, 0, 0, -1, 0, 1);
            test(CollisionDetection.LineSegementsIntersect(a1, a2, b1, b2, out output));
            set4Vec2(out a1, out a2, out b1, out b2, 0, -1, 0, 1, -1, 0, 1, 0);
            test(CollisionDetection.LineSegementsIntersect(a1, a2, b1, b2, out output));

            test(jsonTest("data/jsontest.json"));

            return !fail;
        }

        /// <summary>
        /// Obviously hard coded and not thorough
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool jsonTest(string path)
        {
            var jsontest = Common.FileReadAllLines(path);
            foreach (var s in jsontest)
                Console.WriteLine(s);

            var json = Common.FileReadJson(path);
            Console.WriteLine(json["objkobj"]["objkarr"][6]);

            return true;
        }
    }
}
