namespace Common.Math.Matricies
{
  public interface IMatrix<T>
  {
    /// <summary>
    /// Matrix height
    /// </summary>
    int Rows { get; }
    /// <summary>
    /// Matrix length
    /// </summary>
    int Columns { get; }
    /// <summary>
    /// Matrix data
    /// </summary>
    T[,] MatrixValues { get; set; }

    /// <summary>
    /// Getter for the determinant property
    /// </summary>
    /// <returns>Value of the determinant</returns>
    double GetDeterminant();
    /// <summary>
    /// Getter for the inverse property
    /// </summary>
    /// <returns>Instance of the inversed matrix</returns>
    IMatrix<T> GetInverse();
    /// <summary>
    /// Transposes the matrix
    /// </summary>
    /// <returns>Instance of the transposed matrix</returns>
    IMatrix<T> Transpose();
    /// <summary>
    /// Calculates the matrix of cofactors
    /// </summary>
    /// <returns>Instance of matrix of cofactors</returns>
    IMatrix<T> MatrixOfCofactors();
    string ToString();
  }
}
