using System;
using System.Text;
using static Common.Math.UniversalNumericOperation;

namespace Common.Math
{
  [Serializable]
  public sealed class Matrix<T> : IMatrix<T> where T: struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
  {
    /// <summary>
    /// Matrix properties
    /// </summary>
    [Flags]
    public enum Type
    {
      /// <summary>
      /// N-by-M Matrix
      /// </summary>
      NonInvertable,
      /// <summary>
      /// N-by-N Matrix
      /// </summary>
      Invertable,
      /// <summary>
      /// Matrix with ones on the main diagonal and zeros elsewhere
      /// </summary>
      Identity
    }

    #region Properties

    /// <summary>
    /// Matrix type
    /// </summary>
    public Type MatrixType { get; }

    /// <summary>
    /// Matrix length
    /// </summary>
    public int Columns => MatrixValues.GetLength(1);

    /// <summary>
    /// Matrix height
    /// </summary>
    public int Rows => MatrixValues.GetLength(0);

    /// <summary>
    /// Matrix data
    /// </summary>
    public T[,] MatrixValues
    {
      get => matrixValues;
      set
      {
        matrixValues = value;
        UpdateProperties();
      }
    }
    private T[,] matrixValues;

    /// <summary>
    /// Determinant of the matrix.
    /// <para>WARNING: Returns null if <see cref="MatrixType"/> is <see cref="Type.NonInvertable"/></para>
    /// </summary>
    public double? Determinant => determinant.Value;
    private Lazy<double?> determinant;

    /// <summary>
    /// Inverse of the matrix
    /// </summary>
    public IMatrix<T> Inverse => inverse.Value;
    private Lazy<IMatrix<T>> inverse;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="values">2D Array of values</param>
    /// <exception cref="ArgumentException"></exception>
    public Matrix(T[,] values)
    {
      if (!default(T).IsNumber()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (values == null) throw new ArgumentException($"Argument {nameof(values)} cannot be null.");
      if (values.GetLength(0) == 0 || values.GetLength(1) == 0) throw new ArgumentException($"Argument {nameof(values)} cannot have a dimension of size 0.");

      MatrixType = GetMatrixType(values);
      MatrixValues = values;
    }

    /// <summary>
    /// Constructor for square matrix
    /// </summary>
    /// <param name="size">Value for both the <see cref="Rows"/> and <see cref="Columns"/> properties</param>
    /// <param name="identity">Set to True to create an identity matrix</param>
    /// <exception cref="ArgumentException"></exception>
    public Matrix(int size, bool identity)
    {
      if (!default(T).IsNumber()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (size < 0) throw new ArgumentException($"Argument {nameof(size)} cannot be negative.");
      if (size == 0) throw new ArgumentException($"Argument {nameof(size)} cannot be equal to 0.");

      var matrix = new T[size, size];
      if (identity)
        for (var i = 0; i < size; i++)
          matrix[i, i] = (dynamic)1;

      MatrixType = Type.Identity | Type.Invertable;
      MatrixValues = matrix;
    }

    /// <summary>
    /// Default Constructor
    /// <para>Matrix values are initialized to 0</para>
    /// </summary>
    /// <param name="length">Length of matrix</param>
    /// <param name="height">Height of matrix</param>
    /// <exception cref="ArgumentException"></exception>
    public Matrix(int length, int height)
    {
      if (!default(T).IsNumber()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (length < 0) throw new ArgumentException($"Argument {nameof(length)} cannot be negative.");
      if (height < 0) throw new ArgumentException($"Argument {nameof(height)} cannot be negative.");
      if (length == 0 || height == 0) throw new ArgumentException($"Arguments {nameof(length)} and {nameof(height)} cannot be equal to 0.");

      MatrixValues = new T[length, height];

      if (Columns == Rows) MatrixType = Type.Invertable;
    }

    #endregion

    #region Operators

    public static Matrix<T> operator +(Matrix<T> m, Matrix<T> n)
    {
      if (m.Rows != n.Rows || m.Columns != m.Rows)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be summed.");

      var outputvalues = new T[m.Columns, m.Rows];

      for (var i = 0; i < m.Columns; i++)
        for (var j = 0; j < m.Rows; j++)
          outputvalues[i, j] = Add<T, T>(m.MatrixValues[i, j], n.MatrixValues[i, j]);

      return new Matrix<T>(outputvalues);
    }

    public static Matrix<T> operator -(Matrix<T> m, Matrix<T> n)
    {
      if (m.Rows != n.Rows || m.Columns != m.Rows)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be subtracted.");

      var outputValues = new T[m.Columns, m.Rows];

      for (var i = 0; i < m.Columns; i++)
        for (var j = 0; j < m.Rows; j++)
          outputValues[i, j] = Subtract<T, T>(m.MatrixValues[i, j], n.MatrixValues[i, j]);

      return new Matrix<T>(outputValues);
    }

    public static Matrix<T> operator *(Matrix<T> m, Matrix<T> n)
    {
      if (m.Rows != n.Columns)
        throw new MatrixDimensionException("");

      var outputValues = new T[m.Rows, n.Columns];

      for (var i = 0; i < n.Columns; i++)
        for (var j = 0; j < m.Rows; j++)
          for (var k = 0; k < m.Rows; k++)
            outputValues[i, j] = Multiply<T, T>(outputValues[i, j], m.MatrixValues[i, k], n.MatrixValues[k, j]);

      return new Matrix<T>(outputValues);
    }

    public static Matrix<T> operator *(double m, Matrix<T> n)
    {
      var outputvalues = new T[n.Rows, n.Columns];
      for (var i = 0; i < n.Rows; i++)
        for (var j = 0; j < n.Columns; j++)
          outputvalues[i, j] = Multiply<T, double, T>(n.MatrixValues[i, j], m);

      return new Matrix<T>(outputvalues);
    }

    public static Matrix<T> operator /(double m, Matrix<T> n)
    {
      var outputvalues = new T[n.Rows, n.Columns];
      for (var i = 0; i < n.Rows; i++)
        for (var j = 0; j < n.Columns; j++)
          outputvalues[i, j] = Divide<T, double, T>(n.MatrixValues[i, j], m);

      return new Matrix<T>(outputvalues);
    }

    #endregion

    #region Methods

    private void UpdateProperties()
    {
      determinant = (MatrixType & Type.Invertable) != 0 ? new Lazy<double?>(() => FindDeterminant(matrixValues)) : new Lazy<double?>(() => null);
      inverse = (MatrixType & Type.Invertable) != 0 ? new Lazy<IMatrix<T>>(FindInverse) : new Lazy<IMatrix<T>>(() => null);
    }

    public override string ToString()
    {
      var result = new StringBuilder();
      for (var row = 0; row < Rows; row++)
      {
        for (var column = 0; column < Columns - 1; column++)
          result.Append($"{MatrixValues[row, column]} ");
        result.Append($"{MatrixValues[row, Columns - 1]}");
        if (row != Rows - 1) result.Append('\n');
      }

      return result.ToString();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    private static Type GetMatrixType(T[,] matrix)
    {
      Type result;
      if (matrix.GetLength(0) == matrix.GetLength(1))
      {
        result = Type.Invertable;
        if (IsIdentity(matrix)) result |= Type.Identity;
      }
      else
        result = Type.NonInvertable;

      return result;
    }

    /// <summary>
    /// Validates whether given <paramref name="matrix"/> is an identity matrix
    /// </summary>
    /// <param name="matrix">Matrix to validate</param>
    /// <returns>True if <paramref name="matrix"/> is an identity matrix</returns>
    private static bool IsIdentity(T[,] matrix)
    {
      var index = 0;
      for (var i = 0; i < matrix.GetLength(0); i++)
      {
        for (var j = 0; j < matrix.GetLength(1); j++)
          if (matrix[i, j] != (dynamic)(index == j ? 1 : 0))
            return false;
        ++index;
      }

      return true;
    }

    /// <summary>
    /// Transposes matrix
    /// </summary>
    /// <returns>Transposed matrix</returns>
    public IMatrix<T> Transpose()
    {
      var transposedValues = new T[Rows, Columns];
      for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Columns; j++)
          transposedValues[i, j] = MatrixValues[j, i];

      return new Matrix<T>(transposedValues);
    }

    public IMatrix<T> MatrixOfCofactors()
    {
      var sign = 1;
      var cofactorvalues = new T[Rows, Columns];
      for (var i = 0; i < Columns; i++)
      {
        for (var j = 0; j < Columns; j++)
        {
          cofactorvalues[i, j] = Multiply<T, int, T>(MatrixValues[i, j], sign);
          sign = -sign;
        }

        if (Rows % 2 == 0) sign = -sign;
      }

      return new Matrix<T>(cofactorvalues);
    }

    /// <summary>
    /// Find the inverse of the matrix
    /// </summary>
    /// <returns>Inverted matrix</returns>
    private IMatrix<T> FindInverse()
    {
      if (Rows == 2)
      {
        var inverseArray = new T[2, 2];
        inverseArray[0, 0] = Multiply<T, double?, T>(MatrixValues[1, 1], 1 / Determinant);
        inverseArray[0, 1] = Multiply<T, double?, T>(MatrixValues[0, 1], 1 / -Determinant);
        inverseArray[1, 0] = Multiply<T, double?, T>(MatrixValues[1, 0], 1 / -Determinant);
        inverseArray[1, 1] = Multiply<T, double?, T>(MatrixValues[0, 0], 1 / Determinant);

        return new Matrix<T>(inverseArray);
      }

      var matrixofminors = new T[Columns, Rows];
      for (var i = 0; i < Columns; i++)
      {
        for (var j = 0; j < Columns; j++)
        {
          var coords = new[] { 0, 0 };
          var submatrix = new T[Columns - 1, Rows - 1];
          for (var k = 0; k < Columns; k++)
          {
            for (var l = 0; l < Columns; l++)
            {
              if (k == i || l == j) continue;
              submatrix[coords[0], coords[1]++] = MatrixValues[k, l];
            }

            if (coords[1] == Columns - 1) coords[0]++;

            coords[1] = 0;
          }

          matrixofminors[i, j] = (dynamic)FindDeterminant(submatrix);
        }
      }

      var minorMatrix = new Matrix<T>(matrixofminors);
      var cofactorMatrix = minorMatrix.MatrixOfCofactors().Transpose();

      for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Columns; j++)
          cofactorMatrix.MatrixValues[i, j] = Multiply<T, double?, T>(cofactorMatrix.MatrixValues[i, j], (1 / Determinant));

      return cofactorMatrix;
    }

    /// <summary>
    /// Finds the determinant of the matrix
    /// </summary>
    /// <exception cref="InvertableMatrixOperationException"></exception>
    /// <returns>Determinant value</returns>
    private static double FindDeterminant(T[,] matrix)
    {
      if (matrix.GetLength(0) != matrix.GetLength(1))
        throw new InvertableMatrixOperationException("Determinant can be calculated only for NxN matricies.");

      switch (matrix.GetLength(0))
      {
        case 2:
          return Subtract<T, double>(
                  Multiply<T, T>(matrix[0, 0], matrix[1, 1]),
                  Multiply<T, T>(matrix[0, 1], matrix[1, 0]));
        case 3:
          var left = Add<T, T>(
                      Multiply<T, T>(matrix[0, 0], matrix[1, 1], matrix[2, 2]),
                      Multiply<T, T>(matrix[0, 1], matrix[1, 2], matrix[2, 0]),
                      Multiply<T, T>(matrix[0, 2], matrix[1, 0], matrix[2, 1]));
          var right = Add<T, T>(
                       Multiply<T, T>(matrix[0, 2], matrix[1, 1], matrix[2, 0]),
                       Multiply<T, T>(matrix[0, 0], matrix[1, 2], matrix[2, 1]),
                       Multiply<T, T>(matrix[0, 1], matrix[1, 0], matrix[2, 2]));
          return Subtract<T, double>(left, right);
        default:
          var determinant = 0d;
          var sign = 1;
          for (var i = 0; i < matrix.GetLength(0); i++)
          {
            var coords = new[] { 0, 0 };
            var data = new T[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];

            for (var row = 1; row < matrix.GetLength(0); row++)
            {
              for (var col = 0; col < matrix.GetLength(0); col++)
              {
                if (row == 0 || col == i) continue;
                data[coords[0], coords[1]++] = matrix[row, col];

                if (coords[1] != matrix.GetLength(0) - 1) continue;
                coords[0]++;
                coords[1] = 0;
              }
            }

            determinant = Add<T, double, double>(
              Multiply<T, int, T>(
                Multiply<T, double, T>(matrix[0, i], FindDeterminant(data)),
                sign),
              determinant);
            sign = -sign;
          }

          return determinant;
      }
    }

    #endregion

    #region Exceptions

    private sealed class InvertableMatrixOperationException : Exception
    {
      public InvertableMatrixOperationException(string message) : base(message) { }
    }

    private sealed class MatrixDimensionException : Exception
    {
      public MatrixDimensionException(string message) : base(message) { }
    }

    #endregion
  }
}
