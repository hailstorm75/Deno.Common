using System;

namespace Common.Math
{
  public interface IMatrix<T> where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
  {
    int Rows { get; }
    int Columns { get; }
    T[,] MatrixValues { get; set; }
    double? Determinant { get; }

    IMatrix<T> Transpose();
    IMatrix<T> MatrixOfCofactors();
    string ToString();
  }
}
