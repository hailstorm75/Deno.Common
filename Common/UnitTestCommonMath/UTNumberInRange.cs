using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Math.Tests
{
  [TestClass]
  public class UTNumberInRange
  {
    #region Initialization tests

    [TestMethod]
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

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeEqualMinMax()
    {
      var numberInRange = new NumberInRange(0, 0, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeGreaterMin()
    {
      var numberInRange = new NumberInRange(0, 1, 0);
    }

    [TestMethod]
    public void InitializeAdjustToRange()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      Assert.AreEqual(1, numberInRange.Value);
    }

    #endregion

    #region Operator tests

    [TestMethod]
    public void AdditionIntClass()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = 6 + numberInRange;
      Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void AdditionClassInt()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = numberInRange + 6;
      Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void AdditionClassClass()
    {
      var numberInRangeA = new NumberInRange(5, 0, 4);
      var numberInRangeB = new NumberInRange(6, 0, 4);
      var result = numberInRangeA + numberInRangeB;
      Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void SubtractionIntClass()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = 6 - numberInRange;
      Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void SubtractionClassInt()
    {
      var numberInRange = new NumberInRange(5, 0, 4);
      var result = numberInRange - 6;
      Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void SubtractionClassClass()
    {
      var numberInRangeA = new NumberInRange(5, 0, 4);
      var numberInRangeB = new NumberInRange(6, 0, 4);
      var result = numberInRangeA - numberInRangeB;
      Assert.AreEqual(4, result);
    }

    #endregion
  }
}
