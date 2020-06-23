using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ray_Tracing
{
    public class Sphere : IShape
    {
        public Point Centre { get; private set; }
        public double Radius { get; private set; }

        public Sphere(Point centre, double radius)
        {
            Centre = centre;
            Radius = radius;
        }

        public Vector NormalFromPoint(Point spot)
        {
            Vector result = new Vector(Centre, spot).Normalize();
            return result;
        }

        public bool TryIntersect(Ray ray, out double dist, out Point spot)
        {
            Vector s = new Vector(ray.Begin, Centre);
            double a = ray.Direction * ray.Direction;
            double b = -2 * (ray.Direction * s);
            double c = s * s - Math.Pow(Radius, 2);
            double D = b * b - 4 * a * c;
            if (D < 0) // Если дискриминант меньше нуля - луч фигуру не пересекает
            {
                dist = 0;
                spot = null;
                return false;
            }
            /*Коэффициенты уравнения луча для точек пересечения*/
            double k1 = (-b + Math.Sqrt(D)) / 2 * a;
            double k2 = (-b - Math.Sqrt(D)) / 2 * a;
            dist = 0;
            if(k1 > 0 && k2 > 0) // Если обе точки пересечения лежат на луче - выбираем ближайшую
            {
                dist = Math.Min(k1, k2);
            }
            else if(k1 > 0)
            {
                dist = k1;
            }
            else if(k2 > 0)
            {
                dist = k2;
            }
            else // Если обе точки лежат с обратной стороны, значит луч вигуру не пересекает
            {
                spot = null;
                return false;
            }

            spot = new Point(ray.Begin.X + (dist * ray.Direction.X), ray.Begin.Y + (dist * ray.Direction.Y), 
                ray.Begin.Z + (dist * ray.Direction.Z));
            return true;
        }

        public Point Translate(Point spot)
        {
            Point localSpot = new Point(spot.X, spot.Y, spot.Z); 
            localSpot.Translate(-Centre.X, -Centre.Y, -Centre.Z); //Освещённая точка в системе координат с началом в центре сферы

            /*Нахождение сферических координат точки*/
            double r = Math.Sqrt(Math.Pow(localSpot.X, 2) + Math.Pow(localSpot.Y, 2) + Math.Pow(localSpot.Z, 2));
            double latitude = Math.Acos(localSpot.Z / r);
            double longtitude;
            if (localSpot.X == 0) //Проверка, что точка лежит на полюсе
            {
                longtitude = 0;
            }
            else
            {
                 longtitude = Math.Atan(localSpot.Y / localSpot.X);
            }
            return new Point(latitude, longtitude, r);

        }
    }
}
