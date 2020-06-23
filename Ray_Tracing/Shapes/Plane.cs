using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class Plane : IShape
    {
        /*Коэффициенты уравнения плоскости*/
        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }
        public double D { get; private set; }
        private ReferenceFrame frame; // Система отсчёта, в которой оси Х и Y лежат на данной плоскости

        public Plane(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            frame = CreateFrame();
        }

        public Vector NormalFromPoint(Point spot)
        {
            /*Определение, какой стороной к наблюдателю повёрнута плоскость*/
            if (D < 0)
            {
                return -new Vector(A, B, C).Normalize();
            }
            else
            {
                return new Vector(A, B, C).Normalize();
            }
        }

        public bool TryIntersect(Ray ray, out double dist, out Point spot)
        {
            double a = A * ray.Direction.X + B * ray.Direction.Y + C * ray.Direction.Z;
            if (a == 0)
            {
                dist = 0;
                spot = null;
                return false;
            }

            dist = -(A * ray.Begin.X + B * ray.Begin.Y + C * ray.Begin.Z + D) / a;
            spot = ray.Begin + ray.Direction * dist;
            if (dist <= 0)
            {
               
                return false;
            }
            
            return true;
        }

        public Point Translate(Point spot)
        {
            double[,] A = MatrixHandler.MatrixInverse(frame.TransMatrix);
            double[,] localO = new double[3, 1]; //Точка начала местной системы координат в глобальных координатах
            localO[0, 0] = frame.O.X;
            localO[1, 0] = frame.O.Y;
            localO[2, 0] = frame.O.Z;
            double[,] O = MatrixHandler.Multiply(MatrixHandler.Multiply(A, localO), -1); //Начало глобальных координат в местных координатах
            double[,] spotColumn = new double[3, 1]; // Столбец координат данной точки
            spotColumn[0, 0] = spot.X;
            spotColumn[1, 0] = spot.Y;
            spotColumn[2, 0] = spot.Z;
            double[,] localSpot = MatrixHandler.Sum(O, MatrixHandler.Multiply(A, spotColumn)); // Столбец координат данной точки в местных координатах
            return new Point(localSpot[0, 0], localSpot[1, 0], localSpot[2, 0]);
        }

        private ReferenceFrame CreateFrame()
        {
            Vector n = new Vector(A, B, C);
            Vector dir;
            /* Выбор оси, пересекающейся с плоскостью */
            if(new Vector(1, 0, 0) * n != 0)
            {
                dir = new Vector(1, 0, 0);
            }
            else if(new Vector(0, 1, 0) * n != 0)
            {
                dir = new Vector(0, 1, 0);
            }
            else
            {
                dir = new Vector(0, 0, 1);
            }
            /* Точка пересечения оси и плоскости - точка начала новой системы координат */
            TryIntersect(new Ray(new Point(0, 0, 0), dir), out double dist, out Point o);
            Vector x;
            if (n.X == 0) // Проверка плоскости на параллельность оси Х
            {
                x = new Vector(1, 0, 0);
            }
            else
            {
                x = new Vector(-n.Z / n.X, 0, 1).Normalize(); // Вычисление направление новой оси Х
            }
            Vector y = Vector.Cross(x, n).Normalize(); // Новая ось Y
            Vector z = n.Normalize(); // Новая ось Z

            return new ReferenceFrame(o, x, y, z);
        }
    }
}
