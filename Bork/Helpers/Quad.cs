using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public class Quad
    {
        public Vec2 v0, v1, v2, v3;

        public Quad(Vec2 v0, Vec2 v1, Vec2 v2, Vec2 v3)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public Quad(Vec2 center, Vec2 size)
        {
            var halfSize = size / 2;
            v0 = center - halfSize;
            v1 = new Vec2(center.X + halfSize.X, center.Y - halfSize.Y);
            v2 = center + halfSize;
            v1 = new Vec2(center.X - halfSize.X, center.Y + halfSize.Y);
        }

        public void translate(Vec2 displacement)
        {
            v0 += displacement;
            v1 += displacement;
            v2 += displacement;
            v3 += displacement;
        }

        public void scale(Vec2 scale)
        {
            v0 *= scale;
            v1 *= scale;
            v2 *= scale;
            v3 *= scale;
        }
    }
}
