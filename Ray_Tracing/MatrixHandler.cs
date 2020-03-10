using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray_Tracing
{
    public static class MatrixHandler
    {
        public static double[,] Multiply(double[,] first, double[,] second)
        {
            if (first.GetLength(1) != second.GetLength(0))
            {
                throw new System.ArgumentException("Количество столбцов первой матрицы не совпадает с количеством строк второй");
            }
            double[,] result = new double[first.GetLength(0), second.GetLength(1)];
            for (int i = 0; i < first.GetLength(0); i++)
            {
                for (int j = 0; j < second.GetLength(1); j++)
                {
                    for (int k = 0; k < second.GetLength(0); k++)
                    {
                        result[i, j] += first[i, k] * second[k, j];
                    }
                }
            }

            return result;
        }

        public static double[,] Sum(double[,] first, double[,] second)
        {
            if(first.GetLength(0) != second.GetLength(0) || first.GetLength(1) != second.GetLength(1))
            {
                throw new ArgumentException("Измерения матриц не равны");
            }
            double[,] result = new double[first.GetLength(0), first.GetLength(1)];
            for(int i = 0; i < first.GetLength(0); i++)
            {
                for(int j = 0; j < first.GetLength(1); j++)
                {
                    result[i, j] = first[i, j] + second[i, j];
                }
            }
            return result;
        }

        public static double[,] Multiply(double[,] matrix, double num)
        {
            double[,] result = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i, j] = matrix[i, j] * num;
                }
            }
            return result;
        }

        /*Инвертирование матрицы с помощью LUP-разложения*/
        public static double[,] MatrixInverse(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double[,] result = MatrixDuplicate(matrix);

            int[] perm;
            int toggle;
            double[,] lum = MatrixDecompose(matrix, out perm,
              out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }

                double[] x = HelperSolve(lum, b);

                for (int j = 0; j < n; ++j)
                    result[j, i] = x[j];
            }
            return result;
        }

        private static double[,] MatrixDuplicate(double[,] matrix)
        {
            double[,] result = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); ++i)
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    result[i,j] = matrix[i,j];
            return result;
        }

        private static double[] HelperSolve(double[,] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            int n = luMatrix.GetLength(0);
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i, j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1, n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i, j] * x[j];
                x[i] = sum / luMatrix[i, i];
            }

            return x;
        }

        /*Вычисление LUP-Разложения данной матрицы*/
        private static double[,] MatrixDecompose(double[,] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // returns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1); // assume square
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            int n = rows; // convenience

            double[,] result = MatrixDuplicate(matrix);

            perm = new int[n]; // set up row permutation result
            for (int i = 0; i < n; ++i) { perm[i] = i; }

            toggle = 1; // toggle tracks row swaps.
                        // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (int j = 0; j < n - 1; ++j) // each column
            {
                double colMax = Math.Abs(result[j,j]); // find largest val in col
                int pRow = j;
                
                for (int i = j + 1; i < n; ++i)
                {
                    if (Math.Abs(result[i,j]) > colMax)
                    {
                        colMax = Math.Abs(result[i,j]);
                        pRow = i;
                    }
                }

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    double[] rowPtr = GetRow(pRow, result);
                    SetRow(pRow, result, GetRow(j, result));
                    SetRow(j, result, rowPtr);

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                if (result[j, j] == 0.0)
                {
                    // find a good row to swap
                    int goodRow = -1;
                    for (int row = j + 1; row < n; ++row)
                    {
                        if (result[row, j] != 0.0)
                            goodRow = row;
                    }

                    if (goodRow == -1)
                        throw new Exception("Cannot use Doolittle's method");

                    // swap rows so 0.0 no longer on diagonal
                    double[] rowPtr = GetRow(goodRow, result);
                    SetRow(goodRow, result, GetRow(j, result));
                    SetRow(j, result, rowPtr);

                    int tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                for (int i = j + 1; i < n; ++i)
                {
                    result[i, j] /= result[j, j];
                    for (int k = j + 1; k < n; ++k)
                    {
                        result[i, k] -= result[i, j] * result[j, k];
                    }
                }


            } // main j column loop

            return result;
        }

        private static double[] GetRow(int a, double[,] matrix)
        {
            double[] result = new double[matrix.GetLength(1)];
            for(int i = 0; i < matrix.GetLength(1); i++)
            {
                result[i] = matrix[a, i];
            }
            return result;
        }

        public static void SetRow(int a, double[,] matrix, double[] row)
        {
            for (int i = 0; i < row.Length; i++ )
            {
                matrix[a, i] = row[i];
            }
        }
    }
}
