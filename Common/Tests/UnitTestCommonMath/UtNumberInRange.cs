using System;
using Common.Math.Tests.Data;
using NUnit.Framework;
using UnitTestConstants;

namespace Common.Math.Tests
{
  [TestFixture]
  public class UtNumberInRange
  {
    #region Initialization tests

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void Initialize()
    {
      var unsued = new NumberInRange<int>(0, 5, 10);
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeUnsupportedException()
    {
      Assert.Throws<NotSupportedException>(() =>
      {
        var unused = new NumberInRange<float>(0, 0, 5);
      });
    }

    [Test, TestCaseSource(typeof(DataNumberInRange), nameof(DataNumberInRange.GetCtorArgumentExceptionData))]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeArgumentException(int value, int min, int max)
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var unused = new NumberInRange<int>(value, min, max);
      });
    }

    [Test, TestCaseSource(typeof(DataNumberInRange), nameof(DataNumberInRange.GetCtorAdjustRangeData))]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeAdjustToRange(int value, int min, int max, int expected)
    {
      var numberInRange = new NumberInRange<int>(value, min, max);
      Assert.AreEqual(expected, numberInRange.Value);
    }

    #endregion

    #region Operator tests

    #region Additionthrow

    [Test]
    [Category(Constants.OPERATOR)]
    public void AdditionIntClass()
    {
      var numberInRange = new NumberInRange<int>(6, 0, 4);
      var result = 6 + numberInRange;
      Assert.AreEqual(7, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
    public void AdditionClassInt()
    {
      var numberInRange = new NumberInRange<int>(5, 0, 4);
      var result = numberInRange + 6;
      Assert.AreEqual(1, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
    public void AdditionClassClass()
    {
      var numberInRangeA = new NumberInRange<int>(5, 0, 4);
      var numberInRangeB = new NumberInRange<int>(6, 0, 4);
      var result = numberInRangeA + numberInRangeB;
      Assert.AreEqual(1, result);
    }

    #endregion

    #region Subtraction

    [Test]
    [Category(Constants.OPERATOR)]
    public void SubtractionIntClass()
    {
      var numberInRange = new NumberInRange<int>(6, 0, 4);
      var result = 6 - numberInRange;
      Assert.AreEqual(5, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
    public void SubtractionClassInt()
    {
      var numberInRange = new NumberInRange<int>(5, 0, 4);
      var result = numberInRange - 6;
      Assert.AreEqual(4, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
    public void SubtractionClassClass()
    {
      var numberInRangeA = new NumberInRange<int>(5, 0, 4);
      var numberInRangeB = new NumberInRange<int>(6, 0, 4);
      var result = numberInRangeA - numberInRangeB;
      Assert.AreEqual(4, result);
    }

    #endregion

    #region Multiplication

    [Test]
    [Category(Constants.OPERATOR)]
    public void MultiplicationIntClass()
    {
      var numberInRange = new NumberInRange<int>(6, 0, 4);
      var result = 6 * numberInRange;
      Assert.AreEqual(6, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
    public void MultiplicationClassInt()
    {
      var numberInRange = new NumberInRange<int>(9, 0, 4);
      var result = numberInRange * 7;
      Assert.AreEqual(3, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
    public void MultiplicationClassClass()
    {
      var numberInRangeA = new NumberInRange<int>(9, 0, 4);
      var numberInRangeB = new NumberInRange<int>(7, 0, 4);
      var result = numberInRangeA * numberInRangeB;
      Assert.AreEqual(3, result);
    }

    #endregion

    #region Division

    [Test]
    [Category(Constants.OPERATOR)]
    public void DivisonIntClass()
    {
      var numberInRange = new NumberInRange<int>(7, 0, 4);
      var result = 6 / numberInRange;
      Assert.AreEqual(3, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
    public void DivisionClassInt()
    {
      var numberInRange = new NumberInRange<int>(9, 0, 4);
      var result = numberInRange / 7;
      Assert.AreEqual(2, result);
    }

    [Test]
    [Category(Constants.OPERATOR)]
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
