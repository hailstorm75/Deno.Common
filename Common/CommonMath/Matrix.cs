using System;

namespace Common.Math
{
  public class Matrix
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
    public int Length => MatrixValues.GetLength(1);

    /// <summary>
    /// Matrix height
    /// </summary>
    public int Height => MatrixValues.GetLength(0);

    /// <summary>
    /// Matrix data
    /// </summary>
    public int[,] MatrixValues
    {
      get => matrixValues;
      set
      {
        matrixValues = value;
        determinant = (MatrixType & Type.Invertable) != 0 ? new Lazy<int?>(() => FindDeterminant(matrixValues)) : new Lazy<int?>(() => null);
      }
    }
    private int[,] matrixValues;

    /// <summary>
    /// Determinant of the matrix.
    /// <para>WARNING: Returns null if <see cref="MatrixType"/> is <see cref="Type.NonInvertable"/></para>
    /// </summary>
    public int? Determinant { get { return determinant.Value; } }
    private Lazy<int?> determinant;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="values">2D Array of values</param>
    /// <exception cref="ArgumentException"></exception>
    public Matrix(int[,] values)
    {
      if (values == null) throw new ArgumentException($"Argument {nameof(values)} cannot be null.");
      if (values.GetLength(0) == 0 || values.GetLength(1) == 0) throw new ArgumentException($"Argument {nameof(values)} cannot have a dimension of size 0.");

      MatrixValues = values;
      MatrixType = GetMatrixType(values);
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
      if (length < 0) throw new ArgumentException($"Argument {nameof(length)} cannot be negative.");
      if (height < 0) throw new ArgumentException($"Argument {nameof(height)} cannot be negative.");
      if (length == 0 || height == 0) throw new ArgumentException($"Arguments {nameof(length)} and {nameof(height)} cannot be equal to 0.");

      MatrixValues = new int[length, height];

      if (Length == Height) MatrixType = Type.Invertable;
    }

    #endregion

    #region Operators

    public static Matrix operator +(Matrix m, Matrix n)
    {
      if (m.Height != n.Height || m.Length != m.Height)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be summed.");

      var outputvalues = new int[m.Length, m.Height];
      var outputmatrix = new Matrix(outputvalues);

      for (var i = 0; i < m.Length; i++)
        for (var j = 0; j < m.Height; j++)
          outputvalues[i, j] = m.MatrixValues[i, j] + n.MatrixValues[i, j];

      return outputmatrix;
    }

    public static Matrix operator -(Matrix m, Matrix n)
    {
      if (m.Height != n.Height || m.Length != m.Height)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be subtracted.");

      var outputValues = new int[m.Length, m.Height];
      var outputMatrix = new Matrix(outputValues);

      for (var i = 0; i < m.Length; i++)
        for (var j = 0; j < m.Height; j++)
          outputValues[i, j] = m.MatrixValues[i, j] - n.MatrixValues[i, j];

      return outputMatrix;
    }

    public static Matrix operator *(Matrix m, Matrix n)
    {
      if (m.Height != n.Length)
        throw new MatrixDimensionException("");

      var outputValues = new int[m.Height, n.Length];
      var outputMatrix = new Matrix(outputValues);

      for (var i = 0; i < n.Length; i++)
        for (var j = 0; j < m.Height; j++)
          for (var k = 0; k < m.Height; k++)
            outputValues[i, j] += m.MatrixValues[i, k] * n.MatrixValues[k, j];

      return outputMatrix;
    }

    #endregion

    #region Methods

    public override string ToString()
    {
      var result = new StringBuilder();
      for (var row = 0; row < Height; row++)
      {
        for (var column = 0; column < Length - 1; column++)
          result.Append($"{MatrixValues[row, column]} ");
        result.Append($"{MatrixValues[row, Length - 1]}");
        if (row != Height - 1) result.Append('\n');
      }

      return result.ToString();
    }

    /// <summary>
    /// Validates whether given <paramref name="matrix"/> is an identity matrix
    /// </summary>
    /// <param name="matrix">Matrix to validate</param>
    /// <returns>True if <paramref name="matrix"/> is an identity matrix</returns>
    private static bool IsIdentity(int[,] matrix)
    {
      if (matrix.GetLength(0) != matrix.GetLength(1)) return false;

      var index = 0;
      for (var i = 0; i < matrix.GetLength(0); i++)
      {
        for (var j = 0; j < matrix.GetLength(1); j++)
          if (matrix[i, j] != (index == j ? 1 : 0))
            return false;
        ++index;
      }

      return true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    private static Type GetMatrixType(int[,] matrix)
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
    ///
    /// </summary>
    /// <returns></returns>
    public Matrix Reduce()
    {
      // Traversing matrix diagonally
      for (var i = 0; i < Height; i++)
      {
        if (MatrixValues[i, i] != 1)
        {
          // TODO Introduce leading 1
        }

        // Reducing column
        for (var row = i + 1; row < Height; row++)
        {
          // TODO Doesn't work, fit it
          if (MatrixValues[row, i] == 0) continue;

          var factor = MatrixValues[row, i];
          for (var column = 0; column < Length; column++)
            MatrixValues[row, column] -= factor * MatrixValues[row - 1, column];
        }
      }

      return this;
    }

    /// <summary>
    /// Finds the determinant of the matrix
    /// </summary>
    /// <exception cref="InvertableMatrixOperationException"></exception>
    /// <returns>Determinant value</returns>
    private static int FindDeterminant(int[,] matrix)
    {
      if (matrix.GetLength(0) != matrix.GetLength(1))
        throw new InvertableMatrixOperationException("Determinant can be calculated only for NxN matricies.");

      switch (matrix.GetLength(0))
      {
        case 2:
          return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        case 3:
          var left = matrix[0, 0] * matrix[1, 1] * matrix[2, 2]
                   + matrix[0, 1] * matrix[1, 2] * matrix[2, 0]
                   + matrix[0, 2] * matrix[1, 0] * matrix[2, 1];
          var right = matrix[0, 2] * matrix[1, 1] * matrix[2, 0]
                    + matrix[0, 0] * matrix[1, 2] * matrix[2, 1]
                    + matrix[0, 1] * matrix[1, 0] * matrix[2, 2];
          return left - right;
        default:
          var determinant = 0;
          var sign = 1;
          for (var i = 0; i < matrix.GetLength(0); i++)
          {
            var coords = new[] { 0, 0 };
            var data = new int[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];

            for (int row = 1; row < matrix.GetLength(0); row++)
            {
              for (int col = 0; col < matrix.GetLength(0); col++)
              {
                if (row != 0 && col != i)
                {
                  data[coords[0], coords[1]++] = matrix[row, col];

                  if (coords[1] == matrix.GetLength(0) - 1)
                  {
                    coords[0]++;
                    coords[1] = 0;
                  }
                }
              }
            }

            determinant += matrix[0, i] * FindDeterminant(data) * sign;
            sign = -sign;
          }

          return determinant;
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="InvertableMatrixOperationException"></exception>
    /// <returns></returns>
    public Matrix FindInverse()
    {
      if (!MatrixType.HasFlag(Type.Invertable))
        throw new InvertableMatrixOperationException("Inverse can be calculated only for NxN matricies.");

      throw new NotImplementedException();
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
