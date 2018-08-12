using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Math
{
    class Matrix
    {
        public int Length { get; set; }
        public int Height { get; set; }
        public int[,] MatrixValues { get; set; }

        public Matrix(int length, int height, int[,] values)
        {
            Length = length;
            Height = height;
            MatrixValues = values;
        }

        public static Matrix operator +(Matrix m, Matrix n)
        {
            //first check that we can add the two matrices
            int[,] outputvalues = new int[m.Length, m.Height];
            Matrix outputmatrix = new Matrix(m.Height, m.Length, outputvalues);
            if (m.Height == n.Height && m.Length == m.Height)
            {
                //we can safely add
                for (int i = 0; i < m.Length; i++)
                {
                    for (int j = 0; j < m.Height; j++)
                    {
                        outputvalues[i, j] = m.MatrixValues[i, j] + n.MatrixValues[i, j];
                    }

                }
            }
            else
            {
                //cannot be added
            }
            return outputmatrix;
        }

        public static Matrix operator -(Matrix m, Matrix n)
        {
            //first check that we can add the two matrices
            int[,] outputvalues = new int[m.Length, m.Height];
            Matrix outputmatrix = new Matrix(m.Height, m.Length, outputvalues);
            if (m.Height == n.Height && m.Length == m.Height)
            {
                //we can safely add
                for (int i = 0; i < m.Length; i++)
                {
                    for (int j = 0; j < m.Height; j++)
                    {
                        outputvalues[i, j] = m.MatrixValues[i, j] - n.MatrixValues[i, j];
                    }

                }
            }
            else
            {
                //cannot be added
            }
            return outputmatrix;
        }

        public static Matrix operator *(Matrix m, Matrix n)
        {

            int[,] outputvalues = new int[m.Height, n.Length];
            Matrix outputmatrix = new Matrix(m.Height, m.Length, outputvalues);
            if (m.Height == n.Length)
            {
                //begin the loop
                for (int i = 0; i < n.Length; i++)
                {
                    for (int j = 0; j < m.Height; j++)
                    {
                        for (int k = 0; k < m.Height; k++)
                        {
                            outputvalues[i, j] += m.MatrixValues[i, k] * n.MatrixValues[k, j];
                        }
                    }
                }
            }
            else
            {
                //not possible to multiply

            }



            return outputmatrix;
        }
    }
}
