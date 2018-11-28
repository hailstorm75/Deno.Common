using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTestCommonMath")]
namespace Common.Math.Matricies
{
  /// <summary>
  /// Class representing a mathematical matrix with modulo values
  /// </summary>
  /// <typeparam name="T">Type can only be numeric and non-negative</typeparam>
  [Serializable]
  public sealed class ModuloMatrix<T> : BaseMatrix<T> where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
  {
    #region Properties

    /// <inheritdoc />
    public override Type MatrixType { get; }

    /// <inheritdoc />
    public override T[,] MatrixValues
    {
      get => m_matrixValues;
      set
      {
        m_matrixValues = value;
        Recalculate();
      }
    }

    public uint ModuloValue { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="values">2D Array of values</param>
    /// <param name="modulo">ModuloValue of matrix</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public ModuloMatrix(T[,] values, uint modulo)
    {
      if (!default(T).IsUnsignedInteger()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (values == null) throw new ArgumentException($"Argument {nameof(values)} cannot be null.");
      if (values.GetLength(0) == 0 || values.GetLength(1) == 0) throw new ArgumentException($"Argument {nameof(values)} cannot have a dimension of size 0.");

      ModuloValue = modulo;
      MatrixType = GetMatrixType(values);
      MatrixValues = values;

      for (var row = 0; row < Rows; row++)
        for (var col = 0; col < Columns; col++)
          MatrixValues[row, col] = AdjustValue(MatrixValues[row, col]);
    }

    /// <summary>
    /// Constructor for square matrix
    /// </summary>
    /// <param name="size">Value for both the <see cref="BaseMatrix{T}.Rows"/> and <see cref="BaseMatrix{T}.Columns"/> properties</param>
    /// <param name="modulo">ModuloValue of matrix</param>
    /// <param name="identity">Set to True to create an identity matrix</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public ModuloMatrix(int size, uint modulo, bool identity = false)
    {
      if (!default(T).IsUnsignedInteger()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (size < 0) throw new ArgumentException($"Argument {nameof(size)} cannot be negative.");
      if (size == 0) throw new ArgumentException($"Argument {nameof(size)} cannot be equal to 0.");

      var matrix = new T[size, size];
      if (identity)
        for (var i = 0; i < size; i++)
          matrix[i, i] = (dynamic)1;

      ModuloValue = modulo;
      MatrixType = Type.Identity | Type.Invertable;
      MatrixValues = matrix;
    }

    /// <summary>
    /// Default Constructor
    /// <para>Matrix values are initialized to 0</para>
    /// </summary>
    /// <param name="length">Length of matrix</param>
    /// <param name="height">Height of matrix</param>
    /// <param name="modulo">ModuloValue of matrix</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public ModuloMatrix(int length, int height, uint modulo)
    {
      if (!default(T).IsNumber()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (length < 0) throw new ArgumentException($"Argument {nameof(length)} cannot be negative.");
      if (height < 0) throw new ArgumentException($"Argument {nameof(height)} cannot be negative.");
      if (length == 0 || height == 0) throw new ArgumentException($"Arguments {nameof(length)} and {nameof(height)} cannot be equal to 0.");

      ModuloValue = modulo;
      MatrixValues = new T[length, height];

      if (Columns == Rows) MatrixType = Type.Invertable;
    }

    #endregion

    #region Operators

    public static ModuloMatrix<T> operator +(ModuloMatrix<T> m, ModuloMatrix<T> n)
    {
      if (m.Rows != n.Rows || m.Columns != m.Rows)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be summed.");

      var outputvalues = new T[m.Columns, m.Rows];

      for (var i = 0; i < m.Columns; i++)
        for (var j = 0; j < m.Rows; j++)
          outputvalues[i, j] = m.AdjustValue(UniversalNumericOperation.Add<T, T>(m.MatrixValues[i, j], n.MatrixValues[i, j]));

      return new ModuloMatrix<T>(outputvalues, m.ModuloValue);
    }

    public static ModuloMatrix<T> operator -(ModuloMatrix<T> m, ModuloMatrix<T> n)
    {
      if (m.Rows != n.Rows || m.Columns != m.Rows)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be subtracted.");

      var outputValues = new T[m.Columns, m.Rows];

      for (var i = 0; i < m.Columns; i++)
        for (var j = 0; j < m.Rows; j++)
          outputValues[i, j] = m.AdjustValue(UniversalNumericOperation.Subtract<T, T>(m.MatrixValues[i, j], n.MatrixValues[i, j]));

      return new ModuloMatrix<T>(outputValues, m.ModuloValue);
    }

    public static ModuloMatrix<T> operator *(ModuloMatrix<T> m, ModuloMatrix<T> n)
    {
      if (m.Rows != n.Columns)
        throw new MatrixDimensionException("");

      var outputValues = new T[m.Rows, n.Columns];

      for (var i = 0; i < n.Columns; i++)
        for (var j = 0; j < m.Rows; j++)
          for (var k = 0; k < m.Rows; k++)
            outputValues[i, j] = m.AdjustValue(outputValues[i, j].Add(UniversalNumericOperation.Multiply<T, T>(m.MatrixValues[i, k], n.MatrixValues[k, j])));

      return new ModuloMatrix<T>(outputValues, m.ModuloValue);
    }

    public static ModuloMatrix<T> operator *(double m, ModuloMatrix<T> n)
    {
      var outputvalues = new T[n.Rows, n.Columns];
      for (var i = 0; i < n.Rows; i++)
        for (var j = 0; j < n.Columns; j++)
          outputvalues[i, j] = UniversalNumericOperation.Multiply<T, double, T>(n.MatrixValues[i, j], m);

      return new ModuloMatrix<T>(outputvalues, n.ModuloValue);
    }

    public static ModuloMatrix<T> operator /(double m, ModuloMatrix<T> n)
    {
      var outputvalues = new T[n.Rows, n.Columns];
      for (var i = 0; i < n.Rows; i++)
        for (var j = 0; j < n.Columns; j++)
          outputvalues[i, j] = UniversalNumericOperation.Divide<T, double, T>(n.MatrixValues[i, j], m);

      return new ModuloMatrix<T>(outputvalues, n.ModuloValue);
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override double GetDeterminant() => m_determinant.Value;

    /// <inheritdoc />
    public override IMatrix<T> GetInverse() => m_inverse.Value;

    /// <inheritdoc />
    public override IMatrix<T> Transpose()
    {
      var transposedValues = new T[Rows, Columns];
      for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Columns; j++)
          transposedValues[i, j] = MatrixValues[j, i];

      return new ModuloMatrix<T>(transposedValues, ModuloValue);
    }

    /// <inheritdoc />
    public override IMatrix<T> MatrixOfCofactors()
    {
      var sign = 1;
      var cofactorvalues = new T[Rows, Columns];
      for (var i = 0; i < Columns; i++)
      {
        for (var j = 0; j < Columns; j++)
        {
          cofactorvalues[i, j] = NumberInRange<T>.AdjustValue(UniversalNumericOperation.Multiply<T, int, T>(MatrixValues[i, j], sign), (dynamic)0, (dynamic)ModuloValue);
          sign = -sign;
        }

        if (Rows % 2 == 0) sign = -sign;
      }

      return new ModuloMatrix<T>(cofactorvalues, ModuloValue);
    }

    /// <inheritdoc />
    protected override IMatrix<T> CalculateInverse()
    {
      // TODO Introduce value adjustment
      if (MatrixValues.GetLength(0) != MatrixValues.GetLength(1))
        throw new InvertableMatrixOperationException("Determinant can be calculated only for NxN matricies.");

      if (Rows == 2)
      {
        var inverseArray = new T[2, 2];
        inverseArray[0, 0] = NumberInRange<T>.AdjustValue(UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[1, 1], 1 / GetDeterminant()), (dynamic)0, (dynamic)ModuloValue);
        inverseArray[0, 1] = NumberInRange<T>.AdjustValue(UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[0, 1], 1 / -GetDeterminant()), (dynamic)0, (dynamic)ModuloValue);
        inverseArray[1, 0] = NumberInRange<T>.AdjustValue(UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[1, 0], 1 / -GetDeterminant()), (dynamic)0, (dynamic)ModuloValue);
        inverseArray[1, 1] = NumberInRange<T>.AdjustValue(UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[0, 0], 1 / GetDeterminant()), (dynamic)0, (dynamic)ModuloValue);

        return new ModuloMatrix<T>(inverseArray, ModuloValue);
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

          matrixofminors[i, j] = (dynamic)CalculateDeterminant(submatrix);
        }
      }

      var minorMatrix = new ModuloMatrix<T>(matrixofminors, ModuloValue);
      var cofactorMatrix = minorMatrix.MatrixOfCofactors().Transpose();

      for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Columns; j++)
          cofactorMatrix.MatrixValues[i, j] = NumberInRange<T>.AdjustValue(UniversalNumericOperation.Multiply<T, double, T>(cofactorMatrix.MatrixValues[i, j], (1 / GetDeterminant())), (dynamic)0, (dynamic)ModuloValue);

      return cofactorMatrix;
    }

    /// <summary>
    /// Adjusts given <paramref name="value"/> to modulo range
    /// </summary>
    /// <param name="value">Value to adjust</param>
    /// <returns>Adjusted value</returns>
    private T AdjustValue(T value) => NumberInRange<T>.AdjustValue(value, (dynamic)0, (dynamic)ModuloValue);

    #endregion
  }
}
