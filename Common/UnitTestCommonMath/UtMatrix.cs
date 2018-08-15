using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Math.Tests
{
  [TestClass]
  public class UtMatrix
  {
    #region Initialization

    [TestMethod]
    public void InitializeLengthHeigt()
    {
      try
      {
        var unused = new Matrix(5, 3);
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod]
    public void InitializeArray()
    {
      try
      {
        var unused = new Matrix(new double[,] { { 1, 2, 5 }, { 3, 5, 8 } });
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeNegativeLength()
    {
      var unused = new Matrix(-5, 5);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeNegativeHeight()
    {
      var unused = new Matrix(5, -5);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeZeroSize()
    {
      var unused = new Matrix(0, 0);
    }

    [TestMethod]
    public void InitializeInvertable()
    {
      var matrix = new Matrix(4, 4);
      Assert.AreEqual(Matrix.Type.Invertable, matrix.MatrixType);
    }

    [TestMethod]
    public void InitializeIdentity()
    {
      var matrix = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
      Assert.AreEqual(Matrix.Type.Invertable | Matrix.Type.Identity, matrix.MatrixType);
    }

    #endregion

    #region Propeerties

    [TestMethod]
    public void Invert3X3()
    {
      var matrix = new Matrix(new double[,]
      {
        { 1, 2, 3 },
        { 0, 1, 4 },
        { 5, 6, 0 },
      });
      var result = matrix.Inverse;
      CollectionAssert.AreEqual(new double[,] { { -24, 18, 5 }, { 20, -15, -4 }, { -5, 4, 1 } }, result.MatrixValues);
    }

    // TODO

    #endregion

    #region Operators

    // TODO

    #endregion
  }
}
