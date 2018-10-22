using System;
using Common.Math.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestConstants;

namespace Common.Math.Tests
{
  [TestClass]
  public class UtNumberInRange
  {
    #region Initialization tests

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void Initialize()
    {
      try
      {
        var unsued = new NumberInRange<int>(0, 5, 10);
      }
      catch (Exception e)
      {
        Assert.Fail($"Exception caught: {e}");
      }
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeUnsupportedException()
    {
      var unused = new NumberInRange<float>(0, 0, 5);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    [DynamicData(nameof(DataNumberInRange.GetCtorArgumentExceptionData), typeof(DataNumberInRange), DynamicDataSourceType.Method)]
    public void InitializeArgumentException(int value, int min, int max)
    {
      var unused = new NumberInRange<int>(value, min, max);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [DynamicData(nameof(DataNumberInRange.GetCtorAdjustRangeData), typeof(DataNumberInRange), DynamicDataSourceType.Method)]
    public void InitializeAdjustToRange(int value, int min, int max, int expected)
    {
      var numberInRange = new NumberInRange<int>(value, min, max);
      Assert.AreEqual(expected, numberInRange.Value);
    }

    #endregion

    #region Operator tests

    #region Additionthrow

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void AdditionIntClass()
    {
      var numberInRange = new NumberInRange<int>(6, 0, 4);
      var result = 6 + numberInRange;
      Assert.AreEqual(7, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void AdditionClassInt()
    {
      var numberInRange = new NumberInRange<int>(5, 0, 4);
      var result = numberInRange + 6;
      Assert.AreEqual(1, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void AdditionClassClass()
    {
      var numberInRangeA = new NumberInRange<int>(5, 0, 4);
      var numberInRangeB = new NumberInRange<int>(6, 0, 4);
      var result = numberInRangeA + numberInRangeB;
      Assert.AreEqual(1, result);
    }

    #endregion

    #region Subtraction

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void SubtractionIntClass()
    {
      var numberInRange = new NumberInRange<int>(6, 0, 4);
      var result = 6 - numberInRange;
      Assert.AreEqual(5, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void SubtractionClassInt()
    {
      var numberInRange = new NumberInRange<int>(5, 0, 4);
      var result = numberInRange - 6;
      Assert.AreEqual(4, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void SubtractionClassClass()
    {
      var numberInRangeA = new NumberInRange<int>(5, 0, 4);
      var numberInRangeB = new NumberInRange<int>(6, 0, 4);
      var result = numberInRangeA - numberInRangeB;
      Assert.AreEqual(4, result);
    }

    #endregion

    #region Multiplication

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void MultiplicationIntClass()
    {
      var numberInRange = new NumberInRange<int>(6, 0, 4);
      var result = 6 * numberInRange;
      Assert.AreEqual(6, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void MultiplicationClassInt()
    {
      var numberInRange = new NumberInRange<int>(9, 0, 4);
      var result = numberInRange * 7;
      Assert.AreEqual(3, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void MultiplicationClassClass()
    {
      var numberInRangeA = new NumberInRange<int>(9, 0, 4);
      var numberInRangeB = new NumberInRange<int>(7, 0, 4);
      var result = numberInRangeA * numberInRangeB;
      Assert.AreEqual(3, result);
    }

    #endregion

    #region Division

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void DivisonIntClass()
    {
      var numberInRange = new NumberInRange<int>(7, 0, 4);
      var result = 6 / numberInRange;
      Assert.AreEqual(3, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void DivisionClassInt()
    {
      var numberInRange = new NumberInRange<int>(9, 0, 4);
      var result = numberInRange / 7;
      Assert.AreEqual(2, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void DivisionClassClass()
    {
      var numberInRangeA = new NumberInRange<int>(9, 0, 4);
      var numberInRangeB = new NumberInRange<int>(7, 0, 4);
      var result = numberInRangeA / numberInRangeB;
      Assert.AreEqual(2, result);
    }

    #endregion

    #endregion
  }
}
