namespace Common.Math
{
  public interface IMatrix
  {
    int Rows { get; }
    int Columns { get; }
    double[,] MatrixValues { get; set; }
    double? Determinant { get; }

    IMatrix Transpose();
    IMatrix MatrixOfCofactors();
    string ToString();
  }
}