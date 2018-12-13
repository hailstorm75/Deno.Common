using System;
using System.Text;

namespace Common.Math.Matricies
{
  /// <summary>
  /// Base class for all matrix classes
  /// </summary>
  /// <inheritdoc />
  [Serializable]
  public abstract class BaseMatrix<T> : IMatrix<T>
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

    #region Fields

    /// <summary>
    /// Determinant of the matrix.
    /// </summary>
    protected Lazy<double> m_determinant;

    /// <summary>
    /// Inverse of the matrix
    /// </summary>
    protected Lazy<IMatrix<T>> m_inverse;

    /// <summary>
    /// Matrix data
    /// </summary>
    protected T[,] m_matrixValues;

    #endregion   

    #region Properties

    /// <summary>
    /// Matrix type
    /// </summary>
    public abstract Type MatrixType { get; }

    /// <inheritdoc />
    public int Columns => MatrixValues.GetLength(1);

    /// <inheritdoc />
    public int Rows => MatrixValues.GetLength(0);

    /// <inheritdoc />
    public abstract T[,] MatrixValues { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Requests the <see cref="m_determinant"/> and <seealso cref="m_inverse"/> to be recalculated
    /// </summary>
    protected void Recalculate()
    {
      m_determinant = new Lazy<double>(() => CalculateDeterminant(m_matrixValues));
      m_inverse = new Lazy<IMatrix<T>>(CalculateInverse);
    }

    /// <summary>
    /// Getter for the matrix type property
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns>Matrix type</returns>
    protected static Type GetMatrixType(T[,] matrix)
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
    protected static bool IsIdentity(T[,] matrix)
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
    /// Finds the determinant of the matrix
    /// </summary>
    /// <exception cref="BaseMatrix{T}.InvertableMatrixOperationException"></exception>
    /// <returns>Determinant value</returns>
    protected static double CalculateDeterminant(T[,] matrix)
    {
      if (matrix.GetLength(0) != matrix.GetLength(1))
        throw new InvertableMatrixOperationException("Determinant can be calculated only for NxN matricies.");

      switch (matrix.GetLength(0))
      {
        case 2:
          return UniversalNumericOperation.Subtract<T, double>(
                  UniversalNumericOperation.Multiply<T, T>(matrix[0, 0], matrix[1, 1]),
                  UniversalNumericOperation.Multiply<T, T>(matrix[0, 1], matrix[1, 0]));
        case 3:
          var left = UniversalNumericOperation.Add<T, T>(
                      UniversalNumericOperation.Multiply<T, T>(matrix[0, 0], matrix[1, 1], matrix[2, 2]),
                      UniversalNumericOperation.Multiply<T, T>(matrix[0, 1], matrix[1, 2], matrix[2, 0]),
                      UniversalNumericOperation.Multiply<T, T>(matrix[0, 2], matrix[1, 0], matrix[2, 1]));
          var right = UniversalNumericOperation.Add<T, T>(
                       UniversalNumericOperation.Multiply<T, T>(matrix[0, 2], matrix[1, 1], matrix[2, 0]),
                       UniversalNumericOperation.Multiply<T, T>(matrix[0, 0], matrix[1, 2], matrix[2, 1]),
                       UniversalNumericOperation.Multiply<T, T>(matrix[0, 1], matrix[1, 0], matrix[2, 2]));
          return UniversalNumericOperation.Subtract<T, double>(left, right);
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

            determinant = UniversalNumericOperation.Add<T, double, double>(
              UniversalNumericOperation.Multiply<T, int, T>(
                UniversalNumericOperation.Multiply<T, double, T>(matrix[0, i], CalculateDeterminant(data)),
                sign),
              determinant);
            sign = -sign;
          }

          return determinant;
      }
    }

    /// <inheritdoc />
    public abstract double GetDeterminant();

    /// <summary>
    /// Find the inverse of the matrix
    /// </summary>
    /// <returns>Instance of the inversed matrix</returns>
    protected abstract IMatrix<T> CalculateInverse();

    /// <inheritdoc />
    public abstract IMatrix<T> GetInverse();

    /// <inheritdoc />
    public abstract IMatrix<T> MatrixOfCofactors();

    /// <inheritdoc />
    public abstract IMatrix<T> Transpose();

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

    #endregion

    #region Exceptions

    internal sealed class InvertableMatrixOperationException : Exception
    {
      public InvertableMatrixOperationException(string message) : base(message) { }
    }

    internal sealed class MatrixDimensionException : Exception
    {
      public MatrixDimensionException(string message) : base(message) { }
    }

    #endregion
  }
}
