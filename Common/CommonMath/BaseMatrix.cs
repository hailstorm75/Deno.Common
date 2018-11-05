using System;
using System.Text;

namespace Common.Math
{
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

    protected T[,] m_matrixValues;

    #endregion   

    #region Properties

    /// <summary>
    /// Matrix length
    /// </summary>
    public int Columns => MatrixValues.GetLength(1);

    /// <summary>
    /// Matrix height
    /// </summary>
    public int Rows => MatrixValues.GetLength(0);

    public abstract T[,] MatrixValues { get; set; }

    #endregion

    #region Methods

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

    public abstract double GetDeterminant();

    protected abstract IMatrix<T> CalculateInverse();

    public abstract IMatrix<T> GetInverse();

    public abstract IMatrix<T> MatrixOfCofactors();

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
