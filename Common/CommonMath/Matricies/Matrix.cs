using System;

namespace Common.Math.Matricies
{
  /// <summary>
  /// Class representing a mathematical matrix
  /// </summary>
  /// <example>
  /// Suppoerted generic types for parameter <typeparamref name="T"/>
  /// <list type="bullet">
  ///   <item>
  ///     <description>
  ///       Int16
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       Int32
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       Int64
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       UInt16
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       UInt32
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       UInt64
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       Single
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       Double
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       Decimal
  ///     </description>
  ///   </item>
  /// </list>
  /// </example>
  /// <typeparam name="T">Type of matrix values.<para/>Type can only be numeric</typeparam>
  /// <inheritdoc />
  [Serializable]
  public sealed class Matrix<T>
    : BaseMatrix<T> where T
    : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
  {
    #region Properties

    /// <inheritdoc />
    public override Type MatrixType { get; }

    /// <inheritdoc />
    public override T[][] MatrixValues
    {
      get => m_matrixValues;
      set
      {
        m_matrixValues = value;
        Recalculate();
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="values">2D Array of values</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <example>
    /// How to initialize an instance of <see cref="Matrix{T}"/>
    /// <code>
    /// var arrayOfValues = { { 1, 2, 3 }, { 1, 4, 7 } };
    /// var matrix = Matrix&lt;double&gt;(arrayOfValues);
    /// </code>
    /// </example>
    public Matrix(T[][] values)
    {
      if (!default(T).IsNumber()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (values == null) throw new ArgumentException($"Argument {nameof(values)} cannot be null.");
      if (values.Length == 0 || values[0].Length == 0) throw new ArgumentException($"Argument {nameof(values)} cannot have a dimension of size 0.");

      MatrixType = GetMatrixType(values);
      MatrixValues = values;
    }

    /// <summary>
    /// Constructor for square matrix
    /// </summary>
    /// <param name="size">Value for both the <see cref="BaseMatrix{T}.Rows"/> and <see cref="BaseMatrix{T}.Columns"/> properties</param>
    /// <param name="identity">Set to True to create an identity matrix</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <example>
    /// How to initialize an instance of <see cref="Matrix{T}"/> with 1's on the matrix diagonal
    /// <code>
    /// var matrix = Matrix&lt;double&gt;(4, true);
    /// </code>
    /// </example>
    public Matrix(int size, bool identity)
    {
      if (!default(T).IsNumber()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (size < 0) throw new ArgumentException($"Argument {nameof(size)} cannot be negative.");
      if (size == 0) throw new ArgumentException($"Argument {nameof(size)} cannot be equal to 0.");

      var matrix = new T[size][];
      if (identity)
        for (var i = 0; i < size; i++)
        {
          matrix[i] = new T[size];
          matrix[i][i] = (dynamic)1;
        }

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
    /// <exception cref="NotSupportedException"></exception>
    /// <example>
    /// How to initialize an instance of <see cref="Matrix{T}"/>
    /// <code>
    /// var matrix = Matrix&lt;double&gt;(4, 3);
    /// </code>
    /// </example>
    public Matrix(int length, int height)
    {
      if (!default(T).IsNumber()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (length < 0) throw new ArgumentException($"Argument {nameof(length)} cannot be negative.");
      if (height < 0) throw new ArgumentException($"Argument {nameof(height)} cannot be negative.");
      if (length == 0 || height == 0) throw new ArgumentException($"Arguments {nameof(length)} and {nameof(height)} cannot be equal to 0.");

      MatrixValues = InitializeArray(length, height);

      if (Columns == Rows) MatrixType = Type.Invertable;
    }

    #endregion

    #region Operators

    public static Matrix<T> operator +(Matrix<T> m, Matrix<T> n)
    {
      if (m.Rows != n.Rows || m.Columns != n.Columns)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be summed.");

      var outputValues = InitializeArray(m.Rows, m.Columns);

      for (var i = 0; i < m.Rows; i++)
        for (var j = 0; j < m.Columns; j++)
          outputValues[i][j] = m.MatrixValues[i][j].Add(n.MatrixValues[i][j]);

      return new Matrix<T>(outputValues);
    }

    public static Matrix<T> operator -(Matrix<T> m, Matrix<T> n)
    {
      if (m.Rows != n.Rows || m.Columns != n.Columns)
        throw new MatrixDimensionException("Matricies of different dimensions cannot be subtracted.");

      var outputValues = InitializeArray(m.Rows, m.Columns);

      for (var i = 0; i < m.Rows; i++)
        for (var j = 0; j < m.Columns; j++)
          outputValues[i][j] = m.MatrixValues[i][j].Subtract(n.MatrixValues[i][j]);

      return new Matrix<T>(outputValues);
    }

    public static Matrix<T> operator *(Matrix<T> m, Matrix<T> n)
    {
      if (m.Rows != n.Columns)
        throw new MatrixDimensionException("");

      var outputValues = InitializeArray(m.Rows, n.Columns);

      for (var i = 0; i < n.Columns; i++)
        for (var j = 0; j < m.Rows; j++)
          for (var k = 0; k < m.Rows; k++)
            outputValues[i][j] = outputValues[i][j].Add(m.MatrixValues[i][k].Multiply(n.MatrixValues[k][j]));

      return new Matrix<T>(outputValues);
    }

    public static Matrix<T> operator *(double m, Matrix<T> n)
    {
      var outputValues = InitializeArray(n.Rows, n.Columns);
      for (var i = 0; i < n.Rows; i++)
        for (var j = 0; j < n.Columns; j++)
          outputValues[i][j] = UniversalNumericOperation.Multiply<T, double, T>(n.MatrixValues[i][j], m);

      return new Matrix<T>(outputValues);
    }

    public static Matrix<T> operator /(Matrix<T> m, double n)
    {
      var outputValues = InitializeArray(m.Rows, m.Columns);
      for (var i = 0; i < m.Rows; i++)
        for (var j = 0; j < m.Columns; j++)
          outputValues[i][j] = UniversalNumericOperation.Divide<T, double, T>(m.MatrixValues[i][j], n);

      return new Matrix<T>(outputValues);
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public override double GetDeterminant()
      => m_determinant.Value;

    /// <inheritdoc />
    public override IMatrix<T> GetInverse()
      => m_inverse.Value;

    /// <inheritdoc />
    public override IMatrix<T> Transpose()
    {
      var transposedValues = InitializeArray(Rows, Columns);
      for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Columns; j++)
          transposedValues[i][j] = MatrixValues[j][i];

      return new Matrix<T>(transposedValues);
    }

    /// <inheritdoc />
    public override IMatrix<T> MatrixOfCofactors()
    {
      var sign = 1;
      var cofactorvalues = InitializeArray(Rows, Columns);
      for (var i = 0; i < Columns; i++)
      {
        for (var j = 0; j < Columns; j++)
        {
          cofactorvalues[i][j] = UniversalNumericOperation.Multiply<T, int, T>(MatrixValues[i][j], sign);
          sign = -sign;
        }

        if (Rows % 2 == 0) sign = -sign;
      }

      return new Matrix<T>(cofactorvalues);
    }

    /// <inheritdoc />
    protected override IMatrix<T> CalculateInverse()
    {
      if (Rows != Columns)
        throw new InvertableMatrixOperationException("Determinant can be calculated only for NxN matricies.");

      if (Rows == 2)
      {
        var inverseArray = InitializeArray(2, 2);
        inverseArray[0][0] = UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[1][1], 1 / GetDeterminant());
        inverseArray[0][1] = UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[0][1], 1 / -GetDeterminant());
        inverseArray[1][0] = UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[1][0], 1 / -GetDeterminant());
        inverseArray[1][1] = UniversalNumericOperation.Multiply<T, double, T>(MatrixValues[0][0], 1 / GetDeterminant());

        return new Matrix<T>(inverseArray);
      }

      var matrixofminors = InitializeArray(Columns, Rows);
      for (var i = 0; i < Columns; i++)
      {
        for (var j = 0; j < Columns; j++)
        {
          var coords = new[] { 0, 0 };
          var submatrix = InitializeArray(Columns - 1, Rows - 1);
          for (var k = 0; k < Columns; k++)
          {
            for (var l = 0; l < Columns; l++)
            {
              if (k == i || l == j) continue;
              submatrix[coords[0]][coords[1]++] = MatrixValues[k][l];
            }

            if (coords[1] == Columns - 1) coords[0]++;

            coords[1] = 0;
          }

          matrixofminors[i][j] = (dynamic)CalculateDeterminant(submatrix);
        }
      }

      var minorMatrix = new Matrix<T>(matrixofminors);
      var cofactorMatrix = minorMatrix.MatrixOfCofactors().Transpose();

      for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Columns; j++)
          cofactorMatrix.MatrixValues[i][j] = UniversalNumericOperation.Multiply<T, double, T>(cofactorMatrix.MatrixValues[i][j], (1 / GetDeterminant()));

      return cofactorMatrix;
    }

    #endregion
  }
}
