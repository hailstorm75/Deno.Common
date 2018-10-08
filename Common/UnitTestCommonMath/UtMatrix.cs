using System;
using System.Collections;
using Common.Math.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Math.Tests
{
  [TestClass]
  public class UtMatrix
  {
    #region Initialization

    [TestMethod, TestCategory("Constructor")]
    public void InitializeLengthHeight()
    {
      var unused = new Matrix<double>(5, 3);
    }

    [TestMethod, TestCategory("Constructor")]
    public void InitializeArray()
    {
      var unused = new Matrix<double>(new double[,]
      {
        { 1, 2, 5 },
        { 3, 5, 8 }
      });
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeInvlidSize(int size)
    {
      var unused = new Matrix<double>(size, false);
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType1()
    {
      var unused = new Matrix<DateTime>(5, 5);
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType2()
    {
      var unused = new Matrix<DateTime>(5, true);
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType3()
    {
      var unused = new Matrix<DateTime>(new[,] { { new DateTime(), } });
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    [DynamicData(nameof(DataMatrix.GetCtorExceptionData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void InitializeException(int length, int height)
    {
      var unused = new Matrix<double>(length, height);
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeNull()
    {
      var unused = new Matrix<double>(null);
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeZeroDimension()
    {
      var unused = new Matrix<double>(new double[,] { });
    }

    [TestMethod, TestCategory("Constructor")]
    [DynamicData(nameof(DataMatrix.GetCtorInvertableData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void InitializeInvertable(int length, int height)
    {
      var matrix = new Matrix<double>(length, height);
      Assert.AreEqual(Matrix<double>.Type.Invertable, matrix.MatrixType);
    }

    [TestMethod, TestCategory("Constructor")]
    public void InitializeIdentity()
    {
      var matrix = new Matrix<double>(DataMatrix.InvertableMatrix);
      Assert.AreEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [TestMethod, TestCategory("Constructor")]
    public void InitializeNonIdentity()
    {
      var matrix = new Matrix<double>(new double[,] { { 1, 2, 3 }, { 1, 2, 3 } });
      Assert.AreNotEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [TestMethod, TestCategory("Constructor")]
    public void InitializeIdentity2()
    {
      var matrix = new Matrix<double>(3, true);
      CollectionAssert.AreEqual(DataMatrix.InvertableMatrix, matrix.MatrixValues);
    }

    #endregion

    #region Propeerties

    [TestMethod, TestCategory("Property")]
    [DynamicData(nameof(DataMatrix.GetInvertData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void Invert(double[,] from, double[,] to)
    {
      var matrix = new Matrix<double>(from);
      var result = matrix.GetInverse();
      CollectionAssert.AreEqual(to, result.MatrixValues, new DoubleComparer());
    }

    [TestMethod, TestCategory("Property")]
    [ExpectedException(typeof(Matrix<double>.InvertableMatrixOperationException))]
    [DynamicData(nameof(DataMatrix.GetNonIdentityMatrixData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void DeterminantException(double[,] from)
    {
      var matrix = new Matrix<double>(from);
      var unused = matrix.GetDeterminant();
    }

    #endregion

    #region Operators

    // TODO

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
