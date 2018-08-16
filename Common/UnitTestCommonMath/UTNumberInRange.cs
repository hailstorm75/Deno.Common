using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Math.Tests
{
  [TestClass]
  public class UtNumberInRange
  {
    #region Initialization tests

    [TestMethod, TestCategory("Constructor")]
    public void Initialize()
    {
      try
      {
        var numberInRange = new NumberInRange(0);
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeEqualMinMax()
    {
      var unused = new NumberInRange(0, 0, 0);
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeGreaterMin()
    {
      var unused = new NumberInRange(0, 1, 0);
    }

    [TestMethod, TestCategory("Constructor")]
    public void InitializeAdjustToRange()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      Assert.AreEqual(1, numberInRange.Value);
    }

    #endregion

    #region Operator tests

    #region Additionthrow

    [TestMethod, TestCategory("Operator")]
    public void AdditionIntClass()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = 6 + numberInRange;
      Assert.AreEqual(7, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void AdditionClassInt()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = numberInRange + 6;
      Assert.AreEqual(1, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void AdditionClassClass()
    {
      var numberInRangeA = new NumberInRange(5, 0, 4);
      var numberInRangeB = new NumberInRange(6, 0, 4);
      var result = numberInRangeA + numberInRangeB;
      Assert.AreEqual(1, result);
    }

    #endregion

    #region Subtraction

    [TestMethod, TestCategory("Operator")]
    public void SubtractionIntClass()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = 6 - numberInRange;
      Assert.AreEqual(5, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void SubtractionClassInt()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = numberInRange - 6;
      Assert.AreEqual(4, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void SubtractionClassClass()
    {
      var numberInRangeA = new NumberInRange(5, 0, 4);
      var numberInRangeB = new NumberInRange(6, 0, 4);
      var result = numberInRangeA - numberInRangeB;
      Assert.AreEqual(4, result);
    }

    #endregion

    #region Multiplication

    [TestMethod, TestCategory("Operator")]
    public void MultiplicationIntClass()
    {
      var numberInRange = new NumberInRange(6, 0, 4);
      var result = 6 * numberInRange;
      Assert.AreEqual(6, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void MultiplicationClassInt()
    {
      var numberInRange = new NumberInRange(9, 0, 4);
      var result = numberInRange * 7;
      Assert.AreEqual(3, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void MultiplicationClassClass()
    {
      var numberInRangeA = new NumberInRange(9, 0, 4);
      var numberInRangeB = new NumberInRange(7, 0, 4);
      var result = numberInRangeA * numberInRangeB;
      Assert.AreEqual(3, result);
    }

    #endregion

    #region Division

    [TestMethod, TestCategory("Operator")]
    public void DivisonIntClass()
    {
      var numberInRange = new NumberInRange(7, 0, 4);
      var result = 6 / numberInRange;
      Assert.AreEqual(3, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void DivisionClassInt()
    {
      var numberInRange = new NumberInRange(9, 0, 4);
      var result = numberInRange / 7;
      Assert.AreEqual(2, result);
    }

    [TestMethod, TestCategory("Operator")]
    public void DivisionClassClass()
    {
      var numberInRangeA = new NumberInRange(9, 0, 4);
      var numberInRangeB = new NumberInRange(7, 0, 4);
      var result = numberInRangeA / numberInRangeB;
      Assert.AreEqual(2, result);
    }

    #endregion

    #endregion

    #region Calculation stress tests

    [TestMethod, TestCategory("Operator")]
    public void AdditionClassIntNegativeRange()
    {
      var numberInRange = new NumberInRange(1, 0, 4);
      var result = numberInRange + 16;
      Assert.AreEqual(2, result);
    }

    #endregion
  }
}
