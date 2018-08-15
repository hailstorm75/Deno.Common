using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Math
{
    class Matrix
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public double[,] MatrixValues { get; set; }


        public Matrix(int rows, int columns, double[,] values)
        {
            Rows = rows;
            Columns = columns;
            MatrixValues = values;
        }

        public static Matrix operator +(Matrix m, Matrix n)
        {
            //first check that we can add the two matrices
            double[,] outputvalues = new double[m.Rows, m.Columns];
            Matrix outputmatrix = new Matrix(m.Rows, m.Columns, outputvalues);
            if (m.Rows == n.Rows && m.Columns == m.Rows)
            {
                //we can safely add
                for (int i = 0; i < m.Rows; i++)
                {
                    for (int j = 0; j < m.Columns; j++)
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
            double[,] outputvalues = new double[m.Columns, m.Rows];
            Matrix outputmatrix = new Matrix(m.Rows, m.Columns, outputvalues);
            if (m.Rows == n.Rows && m.Columns == n.Columns)
            {
                //we can safely add
                for (int i = 0; i < m.Rows; i++)
                {
                    for (int j = 0; j < m.Columns; j++)
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
            double[,] outputvalues = new double[m.Rows, n.Columns];
            Matrix outputmatrix = new Matrix(m.Columns, m.Rows, outputvalues);
            if (m.Columns == n.Rows)
            {
                //begin the loop
                for (int i = 0; i < m.Rows; i++)
                {
                    for (int j = 0; j < n.Columns; j++)
                    {
                        for (int k = 0; k < n.Rows; k++)
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
    

        public static Matrix operator *(Matrix m, double n)
        {
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Columns; j++)
                {
                    m.MatrixValues[i, j] = m.MatrixValues[i, j] * n;
                }
            }
            return m;

        }
        public static Matrix operator *(Matrix m, Matrix n)
        {
            double[,] outputvalues = new double[m.Rows, n.Columns];
            Matrix outputmatrix = new Matrix(m.Columns, m.Rows, outputvalues);
            if (m.Columns == n.Rows)
            {
                //begin the loop
                for (int i = 0; i < m.Rows; i++)
                {
                    for (int j = 0; j < n.Columns; j++)
                    {
                        for (int k = 0; k < n.Rows; k++)
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

        public static Matrix operator /(Matrix m, double n)
        {
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Columns; j++)
                {
                    m.MatrixValues[i, j] = m.MatrixValues[i, j] / n;
                }
            }
            return m;
        }


        public double TraceMatrix()
        {
            double trace = 0;
            for (int i = 0; i < this.Columns; i++)
            {
                trace += this.MatrixValues[i, i];
            }
            return trace;
        }

        public Matrix TransposeMatrix()
        {
            double[,] TransposedValues = new double[this.Rows, this.Columns];
            double[,] matarray = (double[,])this.MatrixValues.Clone();
            for (int i = 0; i < this.Columns; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    TransposedValues[i, j] = matarray[j, i];
                }
            }
            Matrix TransposedMatrix = new Matrix(this.Rows, this.Columns, TransposedValues);
            return TransposedMatrix;
        }

        public Matrix MatrixOfCofactors()
        {

            bool positive = true;
            double[,] cofactorvalues = new double[this.Rows, this.Columns];
            for (int i = 0; i < this.Columns; i++)
            {

                for (int j = 0; j < this.Columns; j++)
                {
                    if (!positive)
                    {
                        cofactorvalues[i, j] = -1 * this.MatrixValues[i, j];
                    }
                    positive = !positive;
                }
                if (this.Rows % 2 == 0)
                {
                    positive = !positive;
                }
            }
            //fx
            Matrix CofactorMatrix = new Matrix(this.Rows, this.Columns, cofactorvalues);
            return CofactorMatrix;
        }
        //now we want to find the inverse of a matrix
        public Matrix Inverse()
        {
            List<double> determinants = new List<double>();
            Stack<double> depthlist = new Stack<double>();
            double determinant = this.Determinant(this, 0, true, 0, determinants, this.Columns, depthlist);
            //matrix of cofactors
            double[,] matrixofminors = new double[this.Columns, this.Rows];
            double[,] matrixofcofactors = new double[this.Columns, this.Rows];
            //same size as the big matrix. then
            if (this.Rows == 2)
            {
                double[,] DimensionTwoInverseArray = new double[2, 2];
                DimensionTwoInverseArray[0, 0] = this.MatrixValues[1, 1] * determinant;
                DimensionTwoInverseArray[0, 1] = -1 * this.MatrixValues[0, 1] * determinant;
                DimensionTwoInverseArray[1, 0] = -1 * this.MatrixValues[1, 0] * determinant;
                DimensionTwoInverseArray[1, 1] = this.MatrixValues[0, 0] * determinant;
                return new Matrix(2, 2, DimensionTwoInverseArray);
            }

            else
            {
                for (int i = 0; i < this.Columns; i++)
                {
                    for (int j = 0; j < this.Columns; j++)
                    {
                        int rowindex = 0;
                        int columnindex = 0;
                        //calculate the determinant of the submatrix formed when we take the submatrix
                        double[,] submatrix = new double[this.Columns - 1, this.Rows - 1];
                        for (int k = 0; k < this.Columns; k++)
                        {
                            for (int l = 0; l < this.Columns; l++)
                            {
                                if (k != i && l != j)
                                {
                                    submatrix[rowindex, columnindex] = this.MatrixValues[k, l];
                                    columnindex++;
                                }

                            }
                            if (columnindex == this.Columns - 1)
                            {
                                rowindex++;
                            }
                            columnindex = 0;
                        }

                        Matrix submat = new Matrix(this.Columns - 1, this.Rows - 1, submatrix);
                        List<double> newlist = new List<double>();
                        Stack<double> newstack = new Stack<double>();
                        double determinantofsubmatrix = Determinant(submat, 0, true, 0, li, this.Columns - 1, s);
                        //  matrixofminors[i, j] = Determinant(submat, 0, true, 0, li, m.Columns-1, s);
                        matrixofminors[i, j] = determinantofsubmatrix;
                            
                    }

                }
            }
            Matrix MinorMatrix = new Matrix(this.Columns, this.Rows, matrixofminors);
            Matrix CofactorMatrix = MinorMatrix.MatrixOfCofactors();
            CofactorMatrix = CofactorMatrix.TransposeMatrix();

            for (int i = 0; i < this.Columns; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    CofactorMatrix.MatrixValues[i, j] = CofactorMatrix.MatrixValues[i, j] * (1 / determinant);
                }
            }

            return CofactorMatrix;
        }

        public double Determinant(Matrix m, double toprowindex, bool positive, double determinant, List<double> determinants, int origColumns, Stack<double> depthlist)
        {

            // we shall use Leibniz's formula
            //or use recursive definition
            //so if it is not a 2x2 we reduce it
            //-378 - (-56*6) + (-18*5) - (-63*4) + (-28*6) - (-9*5) + (-56*4) - (-28*6) + (0) - (-18*4) +(-9*6) - (0*5)

            if (m.Columns == m.Rows)
            {


                if (m.Columns == 2)
                {
                    //  int minini = m.MatrixValues[0, 0] * m.MatrixValues[1, 1] - m.MatrixValues[0, 1] * m.MatrixValues[1, 0];
                    //  if (!positive)
                    //  {
                    //     return -1 * (m.MatrixValues[0, 0] * m.MatrixValues[1, 1] - m.MatrixValues[0, 1] * m.MatrixValues[1, 0]);

                    // }
                    // else {

                    Stack<double> clonedepth = new Stack<double>(new Stack<double>(depthlist));
                    double depthval = (m.MatrixValues[0, 0] * m.MatrixValues[1, 1] - m.MatrixValues[0, 1] * m.MatrixValues[1, 0]);
                    while (clonedepth.Count != 0)
                    {
                        depthval *= clonedepth.Pop();
                    }
                    return depthval;
                    // return  (m.MatrixValues[0, 0] * m.MatrixValues[1, 1] - m.MatrixValues[0, 1] * m.MatrixValues[1, 0]);



                    // }
                }
                else
                {
                    positive = true;
                    for (int h = 0; h < m.Rows; h++)
                    {
                        double[,] submatrixarray = new double[m.Columns - 1, m.Rows - 1];
                        int rowindex = 0;
                        int columnindex = 0;
                        for (int i = 0; i < m.Columns; i++)
                        {
                            for (int j = 0; j < m.Columns; j++)
                            {
                                if (i != 0 && j != h)
                                {
                                    submatrixarray[rowindex, columnindex] = m.MatrixValues[i, j];
                                    columnindex++;
                                }
                            }
                            if (i != 0)
                            {
                                rowindex++;
                            }
                            columnindex = 0;
                        }
                        Matrix submatrix = new Matrix(m.Columns - 1, m.Rows - 1, submatrixarray);

                        if (positive)
                        {
                            // multipliers.Add(m.MatrixValues[0, h]);
                            depthlist.Push(m.MatrixValues[0, h]);
                            determinants.Add(Determinant(submatrix, h, positive, determinant, determinants, origColumns, depthlist));
                            depthlist.Pop();
                            //  determinant += m.MatrixValues[0,h] * Determinant(submatrix, h, positive, determinant, determinants);
                        }
                        else
                        {
                            depthlist.Push(-1 * m.MatrixValues[0, h]);
                            //  multipliers.Add(-1 * m.MatrixValues[0, h]);
                            determinants.Add(Determinant(submatrix, h, positive, determinant, determinants, origColumns, depthlist));
                            depthlist.Pop();
                            // determinant -= m.MatrixValues[0,h] * Determinant(submatrix, h, positive, determinant, determinants);
                        }

                        positive = !positive;
                        rowindex = 0;
                        columnindex = 0;

                    }

                    //    positive = !positive;

                }

            }
            else
            {
                throw new ArgumentException("You must enter a square matrix.");

            }
            double det = 0;

            double hi = numofiterations(origColumns);
            if (determinants.Count == hi)
            {
                for (int i = 0; i < determinants.Count; i++)
                {

                    det += determinants[i];
                }
            }
            return det;

        }

        public double numofiterations(double i)
        {
            if (i == 0 || i == 1)
            {
                return 0;
            }
            else if (i == 2)
            {
                return 1;
            }
            else if (i == 3)
            {
                return 3;
            }
            else
            {
                return (numofiterations(i - 1) + 1) * i;
            }

        }
    }
}
