using System;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Math.Matricies
{
  /// <summary>
  /// Base class for all matrix classes
  /// </summary>
  /// <inheritdoc />
  [Serializable]
  public abstract class BaseMatrix<T>
    : IMatrix<T>
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
    protected T[][] m_matrixValues;

    #endregion

    #region Properties

    /// <summary>
    /// Matrix type
    /// </summary>
    public abstract Type MatrixType { get; }

    /// <inheritdoc />
    public int Columns => MatrixValues[0].Length;

    /// <inheritdoc />
    public int Rows => MatrixValues.Length;

    // TODO: Properties should not return arrays
    /// <inheritdoc />
    public abstract T[][] MatrixValues { get; set; }

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
    protected static Type GetMatrixType(T[][] matrix)
    {
      Type result;
      if (matrix.Length == matrix[0].Length)
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
    protected static bool IsIdentity(T[][] matrix)
    {
      var index = 0;
      for (var i = 0; i < matrix.Length; i++)
      {
        for (var j = 0; j < matrix[0].Length; j++)
          if (matrix[i][j] != (dynamic)(index == j ? 1 : 0))
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
    protected static double CalculateDeterminant(T[][] matrix)
    {
      if (matrix.Length != matrix[0].Length)
        throw new InvertableMatrixOperationException("Determinant can be calculated only for NxN matricies.");

      switch (matrix.Length)
      {
        case 2:
          return UniversalNumericOperation.Subtract<T, double>(
                  UniversalNumericOperation.Multiply<T, T>(matrix[0][0], matrix[1][1]),
                  UniversalNumericOperation.Multiply<T, T>(matrix[0][1], matrix[1][0]));
        case 3:
          var left = UniversalNumericOperation.Add<T, T>(
                      UniversalNumericOperation.Multiply<T, T>(matrix[0][0], matrix[1][1], matrix[2][2]),
                      UniversalNumericOperation.Multiply<T, T>(matrix[0][1], matrix[1][2], matrix[2][0]),
                      UniversalNumericOperation.Multiply<T, T>(matrix[0][2], matrix[1][0], matrix[2][1]));
          var right = UniversalNumericOperation.Add<T, T>(
                       UniversalNumericOperation.Multiply<T, T>(matrix[0][2], matrix[1][1], matrix[2][0]),
                       UniversalNumericOperation.Multiply<T, T>(matrix[0][0], matrix[1][2], matrix[2][1]),
                       UniversalNumericOperation.Multiply<T, T>(matrix[0][1], matrix[1][0], matrix[2][2]));
          return UniversalNumericOperation.Subtract<T, double>(left, right);
        default:
          var determinant = 0d;
          var sign = 1;
          for (var i = 0; i < matrix.Length; i++)
          {
            var coords = new[] { 0, 0 };
            var data = InitializeArray(matrix.Length - 1, matrix.Length - 1);

            for (var row = 1; row < matrix.Length; row++)
            {
              for (var col = 0; col < matrix.Length; col++)
              {
                if (row == 0 || col == i) continue;
                data[coords[0]][coords[1]++] = matrix[row][col];

                if (coords[1] != matrix.Length - 1) continue;
                coords[0]++;
                coords[1] = 0;
              }
            }

            determinant = UniversalNumericOperation.Add<T, double, double>(
              UniversalNumericOperation.Multiply<T, int, T>(
                UniversalNumericOperation.Multiply<T, double, T>(matrix[0][i], CalculateDeterminant(data)),
                sign),
              determinant);
            sign = -sign;
          }

          return determinant;
      }
    }

    /// <inheritdoc />
    public abstract double GetDeterminant();

    private bool Swap(T[][] rows, int row, int column)
    {
      var swapped = false;

      for (int z = Rows - 1; z > row; z--)
        if (rows[z][row].IsNotEqual(0))
        {
          var temp = new T[rows[0].Length];
          temp = rows[z];
          rows[z] = rows[column];
          rows[column] = temp;
          swapped = true;
        }

      return swapped;
    }

    /// <summary>
    /// Applies the Gauss Elimination Method
    /// </summary>
    /// <returns></returns>
    public IMatrix<T> ApplyGEM()
    {
      int length = Rows;

      for (int i = 0; i < Rows - 1; i++)
      {
        if (MatrixValues[i][i].IsEqual(0) && !Swap(MatrixValues, i, i))
          return null;

        for (int j = i; j < Rows; j++)
        {
          var d = new T[length];
          for (int x = 0; x < length; x++)
          {
            d[x] = MatrixValues[j][x];

            if (MatrixValues[j][i].IsNotEqual(0))
              d[x] = d[x].Divide(MatrixValues[j][i]);
          }

          // TODO: Optimize
          for (int x = 0; x < length; x++)
            MatrixValues[j][x] = d[x];
        }

        for (int y = i + 1; y < Rows; y++)
        {
          var f = new T[length];
          for (int g = 0; g < length; g++)
          {
            f[g] = MatrixValues[y][g];
            if (MatrixValues[y][i].IsNotEqual(0))
              f[g] = f[g].Subtract(MatrixValues[i][g]);
          }

          // TODO: Optimize
          for (int g = 0; g < length; g++)
            MatrixValues[y][g] = f[g];
        }
      }

      return this;
    }

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

    /// <inheritdoc cref="IMatrix{T}.ToString" />
    public override string ToString()
    {
      var result = new StringBuilder();
      for (var row = 0; row < Rows; row++)
      {
        for (var column = 0; column < Columns - 1; column++)
          result.Append($"{MatrixValues[row][column]} ");
        result.Append($"{MatrixValues[row][Columns - 1]}");
        if (row != Rows - 1) result.Append('\n');
      }

      return result.ToString();
    }

    protected static T[][] InitializeArray(int rows, int columns)
    {
      var array = new T[rows][];
      for (int i = 0; i < rows; i++)
        array[i] = new T[columns];

      return array;
    }

    #endregion

    #region Exceptions

    [Serializable]
    internal sealed class InvertableMatrixOperationException
      : Exception
    {
      public InvertableMatrixOperationException() { }

      public InvertableMatrixOperationException(string message) : base(message) { }

      public InvertableMatrixOperationException(string message, Exception innerException) : base(message, innerException) { }

      private InvertableMatrixOperationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    internal sealed class MatrixDimensionException
      : Exception
    {
      public MatrixDimensionException() { }

      public MatrixDimensionException(string message) : base(message) { }

      public MatrixDimensionException(string message, Exception innerException) : base(message, innerException) { }

      private MatrixDimensionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    #endregion
  }
}
