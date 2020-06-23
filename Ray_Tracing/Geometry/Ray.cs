using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class Ray
    {
        public Point Begin { get; private set; }
        public Vector Direction { get; private set; }

        public Ray(Point begin, Vector direction)
        {
            Begin = begin;
            Direction = direction.Normalize();
        }

        public Ray(Point begin, Point middle)
        {
            Begin = begin;
            Direction = new Vector(begin, middle).Normalize();
        }
    }
}
