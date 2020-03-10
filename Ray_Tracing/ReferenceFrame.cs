using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public class ReferenceFrame
    {
        public Point O { get; private set; } // Точка начала координат
        public Vector XAxis { get; private set; } // Ось Х
        public Vector YAxis { get; private set; } // Ось У
        public Vector ZAxis { get; private set; } // Ось Z
        public double[,] TransMatrix { get; private set; } //Матрица перехода из данной системы координат в глобальную

        public ReferenceFrame(Point o, Vector x, Vector y, Vector z)
        {
            O = o;
            XAxis = x;
            YAxis = y;
            ZAxis = z;
            TransMatrix = new double[3, 3];
            MatrixHandler.SetRow(0, TransMatrix, new double[] { XAxis.X, YAxis.X, ZAxis.X});
            MatrixHandler.SetRow(1, TransMatrix, new double[] { XAxis.Y, YAxis.Y, ZAxis.Y });
            MatrixHandler.SetRow(2, TransMatrix, new double[] { XAxis.Z, YAxis.Z, ZAxis.Z });
        }
    }
}
