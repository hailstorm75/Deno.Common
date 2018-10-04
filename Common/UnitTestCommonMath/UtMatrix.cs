using System;
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
      try
      {
        var unused = new Matrix<double>(5, 3);
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, TestCategory("Constructor")]
    public void InitializeArray()
    {
      try
      {
        var unused = new Matrix<double>(new double[,]
        {
          { 1, 2, 5 },
          { 3, 5, 8 }
        });
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    [DynamicData(nameof(DataMatrix.GetCtorExceptionData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void InitializeException(int length, int height)
    {
      var unused = new Matrix<double>(length, height);
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
      var result = matrix.Inverse;
      CollectionAssert.AreEqual(to, result.MatrixValues);
    }

    #endregion

    #region Operators

    // TODO

    #endregion
  }
}
