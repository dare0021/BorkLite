using Bork.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Modules
{
    static class UnitTest
    {
        static bool fail = false;

        static private bool test(bool testingFunction)
        {
            return fail || !testingFunction;
        }

        static public bool run()
        {
            Vec2 a1 = new Vec2(0, 0);
            Vec2 a2 = new Vec2(0, 0);
            Vec2 b1 = new Vec2(0, 0);
            Vec2 b2 = new Vec2(0, 0);
            Vec2 output;

            test(CollisionDetection.LineSegementsIntersect(a1, a2, b1, b2, out output));

            return !fail;
        }
    }
}
