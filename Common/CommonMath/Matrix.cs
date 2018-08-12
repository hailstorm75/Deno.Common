using System;

namespace Common.Math
{
  // TODO Create custom exception in this file to indicate attempt of invalid mathematical operation
  // Refer to https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions

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
    public int[,] MatrixValues { get; }

    #endregion

    #region Constructors

    public Matrix(int[,] values)
    {
      if (values == null) throw new ArgumentException($"Argument {nameof(values)} cannot be null.");
      if (values.GetLength(0) == 0 || values.GetLength(1) == 0) throw new ArgumentException($"Argument {nameof(values)} cannot have a dimension of size 0.");

      MatrixValues = values;
      MatrixType = GetMatrixType(values);
    }

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
        throw new ArgumentException();

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
        throw new ArgumentException();

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
        throw new ArgumentException();

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

    #endregion
  }
}
