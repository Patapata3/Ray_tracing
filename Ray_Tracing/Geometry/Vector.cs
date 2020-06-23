using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class Vector
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public double Length { get; private set; }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Length = Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector(Point begin, Point end)
        {
            X = end.X - begin.X;
            Y = end.Y - begin.Y;
            Z = end.Z - begin.Z;
            Length = Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector Normalize()
        {
            return new Vector(X / Length, Y / Length, Z / Length);
        }


        public static Vector operator +(Vector first, Vector second)
        {
            return new Vector(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }

        public static Vector operator -(Vector first, Vector second)
        {
            return new Vector(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        public static Vector operator *(Vector first, double mul)
        {
            return new Vector(first.X * mul, first.Y * mul, first.Z * mul);
        }

        public static Vector operator *(double mul, Vector first)
        {
            return new Vector(first.X * mul, first.Y * mul, first.Z * mul);
        }

        public static Vector operator /(Vector first, double value)
        {
            return new Vector(first.X / value, first.Y / value, first.Z / value);
        }

        public static Vector operator -(Vector first)
        {
            return new Vector(-first.X, -first.Y, -first.Z);
        }

        public static double operator *(Vector first, Vector second) //Скалярное произведение
        {
            double result = first.X * second.X + first.Y * second.Y + first.Z * second.Z;
            return result;
        }

        public static Vector Cross(Vector first, Vector second) //Векторное произведение
        {
            return new Vector(first.Y * second.Z - first.Z * second.Y, 
                first.Z * second.X - first.X * second.Z, 
                first.X * second.Y - first.Y * second.X);
        }
    }
}
