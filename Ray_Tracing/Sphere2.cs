using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class Sphere2 : IShape
    {
        public Point Centre { get; set; }
        public int Radius { get; set; }

        public Sphere2(Point centre, int radius)
        {
            Centre = centre;
            Radius = radius;
        }

        public Vector NormalFromPoint(Point spot)
        {
            Vector result = new Vector(Centre, spot);
            result.Normalize();
            return result;
        }

        public bool TryIntersect(Ray ray, ref double closest, ref Point spot)
        {
            Vector s = new Vector(ray.Begin, Centre);
            double a = Vector.ScalMultiply(ray.Direction, ray.Direction);
            double b = -2 * Vector.ScalMultiply(ray.Direction, s);
            double c = Vector.ScalMultiply(s, s) - Math.Pow(Radius, 2);
            double D = b * b - 4 * a * c;
            if (D < 0)
            {
                return false;
            }
            double k1 = (-b + Math.Sqrt(D)) / 2 * a;
            double k2 = (-b - Math.Sqrt(D)) / 2 * a;
            double dist = 0;
            if (k1 > 0 && k1 < k2)
            {
                dist = k1;
            }
            else if (k2 > 0 && k2 < k1)
            {
                dist = k2;
            }
            else
            {
                return false;
            }

            if (dist >= closest)
            {
                return false;
            }
            closest = dist;
            spot.Translate(ray.Begin.X + (dist * ray.Direction.X), ray.Begin.Y + (dist * ray.Direction.Y),
                ray.Begin.Z + (dist * ray.Direction.Z));
            return true;
        }
    }
}
