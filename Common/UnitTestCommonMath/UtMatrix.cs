using System;
using UnitTestConstants;
using System.Collections;
using Common.Math.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Math.Tests
{
  [TestClass]
  public class UtMatrix
  {
    #region Initialization

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeLengthHeight()
    {
      var unused = new Matrix<double>(5, 3);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeArray()
    {
      var unused = new Matrix<double>(new double[,]
      {
        { 1, 2, 5 },
        { 3, 5, 8 }
      });
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [DataRow(0)]
    [DataRow(-1)]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeInvlidSize(int size)
    {
      var unused = new Matrix<double>(size, false);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType1()
    {
      var unused = new Matrix<DateTime>(5, 5);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType2()
    {
      var unused = new Matrix<DateTime>(5, true);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType3()
    {
      var unused = new Matrix<DateTime>(new[,] { { new DateTime(), } });
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    [DynamicData(nameof(DataMatrix.GetCtorExceptionData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void InitializeException(int length, int height)
    {
      var unused = new Matrix<double>(length, height);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeNull()
    {
      var unused = new Matrix<double>(null);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeZeroDimension()
    {
      var unused = new Matrix<double>(new double[,] { });
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [DynamicData(nameof(DataMatrix.GetCtorInvertableData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void InitializeInvertable(int length, int height)
    {
      var matrix = new Matrix<double>(length, height);
      Assert.AreEqual(Matrix<double>.Type.Invertable, matrix.MatrixType);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeIdentity()
    {
      var matrix = new Matrix<double>(DataMatrix.InvertableMatrix);
      Assert.AreEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeNonIdentity()
    {
      var matrix = new Matrix<double>(new double[,] { { 1, 2, 3 }, { 1, 2, 3 } });
      Assert.AreNotEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeIdentity2()
    {
      var matrix = new Matrix<double>(3, true);
      CollectionAssert.AreEqual(DataMatrix.InvertableMatrix, matrix.MatrixValues);
    }

    #endregion

    #region Methods

    [TestMethod, TestCategory(Constants.METHOD)]
    [DynamicData(nameof(DataMatrix.GetInvertData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void Invert(double[,] from, double[,] to)
    {
      var matrix = new Matrix<double>(from);
      var result = matrix.GetInverse();
      CollectionAssert.AreEqual(to, result.MatrixValues, new DoubleComparer());
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    [ExpectedException(typeof(Matrix<double>.InvertableMatrixOperationException))]
    [DynamicData(nameof(DataMatrix.GetNonIdentityMatrixData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void DeterminantException(double[,] from)
    {
      var matrix = new Matrix<double>(from);
      var unused = matrix.GetDeterminant();
    }

    #endregion

    #region Operators

    #region Addition

    #endregion

    #region Subtraction

    #endregion

    #region Multiplication

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetMultiplicationClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void MultiplicationClassClass(double[,] lhs, double[,] rhs, double[,] expected)
    {
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);
      var result = matrixA * matrixB;

      CollectionAssert.AreEqual(expected, result.MatrixValues, new DoubleComparer());
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetMultiplicationInvalidClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    [ExpectedException(typeof(Matrix<double>.MatrixDimensionException))]
    public void MultiplicationInvalidClassClass(double[,] lhs, double[,] rhs)
    {
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);
      var unused = matrixA * matrixB;
    }

    #endregion

    #region Division

    #endregion

    #endregion
  }

  internal class DoubleComparer : IComparer
  {
    public int Compare(object x, object y)
    {
      // ReSharper disable once PossibleNullReferenceException
      var a = (double)x;
      // ReSharper disable once PossibleNullReferenceException
      var b = (double)y;

      return System.Math.Abs(a - b) < 0.0000000000001 ? 0 : 1;
    }
  }
}
