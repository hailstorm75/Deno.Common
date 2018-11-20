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
      // Act
      var unsued = new NumberInRange<int>(0, 5, 10);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeUnsupportedException()
    {
      // Act
      var unused = new NumberInRange<float>(0, 0, 5);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    [DynamicData(nameof(DataNumberInRange.GetCtorArgumentExceptionData), typeof(DataNumberInRange), DynamicDataSourceType.Method)]
    public void InitializeArgumentException(int value, int min, int max)
    {
      // Act
      var unused = new NumberInRange<int>(value, min, max);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [DynamicData(nameof(DataNumberInRange.GetCtorAdjustRangeData), typeof(DataNumberInRange), DynamicDataSourceType.Method)]
    public void InitializeAdjustToRange(int value, int min, int max, int expected)
    {
      // Act
      var numberInRange = new NumberInRange<int>(value, min, max);

      // Assert
      Assert.AreEqual(expected, numberInRange.Value);
    }

    #endregion

    #region Static method tests

    [TestMethod, TestCategory(Constants.METHOD)]
    [DynamicData(nameof(DataNumberInRange.GetCtorAdjustRangeData), typeof(DataNumberInRange), DynamicDataSourceType.Method)]
    public void AdjustValueTest(int value, int min, int max, int expected)
    {
      // Act
      var result = NumberInRange<int>.AdjustValue(value, min, max);

      // Assert
      Assert.AreEqual(expected, result);
    }

    #endregion

    #region Operator tests

    #region Additionthrow

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void AdditionIntClass()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(6, 0, 4);

      // Act
      var result = 6 + numberInRange;

      // Assert
      Assert.AreEqual(7, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void AdditionClassInt()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(5, 0, 4);

      // Act
      var result = numberInRange + 6;

      // Assert
      Assert.AreEqual(1, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void AdditionClassClass()
    {
      // Arrange
      var numberInRangeA = new NumberInRange<int>(5, 0, 4);
      var numberInRangeB = new NumberInRange<int>(6, 0, 4);

      // Act
      var result = numberInRangeA + numberInRangeB;

      // Assert
      Assert.AreEqual(1, result);
    }

    #endregion

    #region Subtraction

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void SubtractionIntClass()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(6, 0, 4);
      
      // Act
      var result = 6 - numberInRange;

      // Assert
      Assert.AreEqual(5, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void SubtractionClassInt()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(5, 0, 4);

      // Act
      var result = numberInRange - 6;

      // Assert
      Assert.AreEqual(4, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void SubtractionClassClass()
    {
      // Arrange
      var numberInRangeA = new NumberInRange<int>(5, 0, 4);
      var numberInRangeB = new NumberInRange<int>(6, 0, 4);

      // Act
      var result = numberInRangeA - numberInRangeB;

      // Assert
      Assert.AreEqual(4, result);
    }

    #endregion

    #region Multiplication

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void MultiplicationIntClass()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(6, 0, 4);

      // Act
      var result = 6 * numberInRange;

      // Assert
      Assert.AreEqual(6, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void MultiplicationClassInt()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(9, 0, 4);

      // Act
      var result = numberInRange * 7;

      // Assert
      Assert.AreEqual(3, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void MultiplicationClassClass()
    {
      // Arrange
      var numberInRangeA = new NumberInRange<int>(9, 0, 4);
      var numberInRangeB = new NumberInRange<int>(7, 0, 4);

      // Act
      var result = numberInRangeA * numberInRangeB;

      // Assert
      Assert.AreEqual(3, result);
    }

    #endregion

    #region Division

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void DivisonIntClass()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(7, 0, 4);

      // Act
      var result = 6 / numberInRange;

      // Assert
      Assert.AreEqual(3, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void DivisionClassInt()
    {
      // Arrange
      var numberInRange = new NumberInRange<int>(9, 0, 4);

      // Act
      var result = numberInRange / 7;

      // Assert
      Assert.AreEqual(2, result);
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    public void DivisionClassClass()
    {
      // Arrange
      var numberInRangeA = new NumberInRange<int>(9, 0, 4);
      var numberInRangeB = new NumberInRange<int>(7, 0, 4);

      // Act
      var result = numberInRangeA / numberInRangeB;

      // Assert
      Assert.AreEqual(2, result);
    }

    #endregion

    #endregion

    #region Interface tests

    #region IConvertible

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(100000, true)]
    [DataRow(100, true)]
    [DataRow(1, true)]
    [DataRow(0, false)]
    public void ConvertToBoolean(int value, bool expected)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToBoolean();

      // Assert
      Assert.AreEqual(expected, result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToChar(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToChar();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToChar(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToSByte(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToSByte();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToSByte(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToByte(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToByte();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToByte(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToInt16(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToInt16();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToInt16(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToUInt16(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToUInt16();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToUInt16(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToInt32(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToInt32();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToInt32(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToUInt32(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToUInt32();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToUInt32(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToInt64(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToInt64();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToInt64(null), result);
    }

    [TestMethod, TestCategory(Constants.CONVERSION)]
    [DataRow(1)]
    public void ConvertToUInt64(int value)
    {
      // Arrange
      var nur = new NumberInRange<int>(value, int.MinValue + 1, int.MaxValue);

      // Act
      var result = nur.ToUInt64();

      // Assert
      Assert.AreEqual(((IConvertible)value).ToUInt64(null), result);
    }

    #endregion

    #endregion
  }
}
