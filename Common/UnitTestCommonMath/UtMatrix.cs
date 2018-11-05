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
      // Act
      var unused = new Matrix<double>(5, 3);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeArray()
    {
      // Act
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
      // Act
      var unused = new Matrix<double>(size, false);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType1()
    {
      // Act
      var unused = new Matrix<DateTime>(5, 5);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType2()
    {
      // Act
      var unused = new Matrix<DateTime>(5, true);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(NotSupportedException))]
    public void InitializeInvalidType3()
    {
      // Act
      var unused = new Matrix<DateTime>(new[,] { { new DateTime(), } });
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    [DynamicData(nameof(DataMatrix.GetCtorExceptionData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void InitializeException(int length, int height)
    {
      // Act
      var unused = new Matrix<double>(length, height);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeNull()
    {
      // Act
      var unused = new Matrix<double>(null);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeZeroDimension()
    {
      // Act
      var unused = new Matrix<double>(new double[,] { });
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    [DynamicData(nameof(DataMatrix.GetCtorInvertableData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void InitializeInvertable(int length, int height)
    {
      // Arrange
      var matrix = new Matrix<double>(length, height);

      // Assert
      Assert.AreEqual(Matrix<double>.Type.Invertable, matrix.MatrixType);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeIdentity()
    {
      // Arrange
      var matrix = new Matrix<double>(DataMatrix.InvertableMatrix);

      // Assert
      Assert.AreEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeNonIdentity()
    {
      // Arrange
      var matrix = new Matrix<double>(new double[,] { { 1, 2, 3 }, { 1, 2, 3 } });

      // Assert
      Assert.AreNotEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [TestMethod, TestCategory(Constants.CONSTRUCTOR)]
    public void InitializeIdentity2()
    {
      // Arrange
      var matrix = new Matrix<double>(3, true);

      // Assert
      CollectionAssert.AreEqual(DataMatrix.InvertableMatrix, matrix.MatrixValues);
    }

    #endregion

    #region Methods

    [TestMethod, TestCategory(Constants.METHOD)]
    [DynamicData(nameof(DataMatrix.GetInvertData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void Invert(double[,] from, double[,] to)
    {
      // Arrange
      var matrix = new Matrix<double>(from);

      // Act
      var result = matrix.GetInverse();

      // Assert
      CollectionAssert.AreEqual(to, result.MatrixValues, new DoubleComparer());
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    [ExpectedException(typeof(Matrix<double>.InvertableMatrixOperationException))]
    public void InvertInvalid()
    {
      // Arrange
      var matrix = new Matrix<double>(2, 3);

      // Act
      var unused = matrix.GetInverse();
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    [ExpectedException(typeof(Matrix<double>.InvertableMatrixOperationException))]
    [DynamicData(nameof(DataMatrix.GetNonIdentityMatrixData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void DeterminantException(double[,] from)
    {
      // Arrange
      var matrix = new Matrix<double>(from);

      // Act
      var unused = matrix.GetDeterminant();
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    public void ToStringTest()
    {
      // Arrange
      var matrix = new Matrix<double>(3, true);

      // Act
      var result = matrix.ToString();

      // Assert
      Assert.AreEqual("1 0 0\n0 1 0\n0 0 1", result);
    }

    #endregion

    #region Operators

    #region Addition

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetAdditionInvalidClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    [ExpectedException(typeof(Matrix<double>.MatrixDimensionException))]
    public void AdditionInvalidClassClass(double[,] lhs, double[,] rhs)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      var unused = matrixA + matrixB;
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetAdditionClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void AdditionClassClass(double[,] lhs, double[,] rhs, double[,] expected)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      var result = matrixA + matrixB;

      // Assert
      CollectionAssert.AreEqual(expected, result.MatrixValues, new DoubleComparer());
    }

    #endregion

    #region Subtraction

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetSubtractionInvalidClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    [ExpectedException(typeof(Matrix<double>.MatrixDimensionException))]
    public void SubtractionInvalidClassClass(double[,] lhs, double[,] rhs)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      var unused = matrixA - matrixB;
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetSubtractionClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void SubtractionClassClass(double[,] lhs, double[,] rhs, double[,] expected)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      var result = matrixA - matrixB;

      // Assert
      CollectionAssert.AreEqual(expected, result.MatrixValues, new DoubleComparer());
    }

    #endregion

    #region Multiplication

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetMultiplicationValueClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void MultiplicationValueClass(double lhs, double[,] rhs, double[,] expected)
    {
      // Arrange
      var matrix = new Matrix<double>(rhs);

      // Act
      var result = lhs * matrix;

      // Assert
      CollectionAssert.AreEqual(expected, result.MatrixValues, new DoubleComparer());
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetMultiplicationClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void MultiplicationClassClass(double[,] lhs, double[,] rhs, double[,] expected)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      var result = matrixA * matrixB;

      // Assert
      CollectionAssert.AreEqual(expected, result.MatrixValues, new DoubleComparer());
    }

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetMultiplicationInvalidClassClassData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    [ExpectedException(typeof(Matrix<double>.MatrixDimensionException))]
    public void MultiplicationInvalidClassClass(double[,] lhs, double[,] rhs)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      var unused = matrixA * matrixB;
    }

    #endregion

    #region Division

    [TestMethod, TestCategory(Constants.OPERATOR)]
    [DynamicData(nameof(DataMatrix.GetDivisionClassValueData), typeof(DataMatrix), DynamicDataSourceType.Method)]
    public void DivisionClassValue(double[,] lhs, double rhs, double[,] expected)
    {
      // Arrange
      var matrix = new Matrix<double>(lhs);

      // Act
      var result = matrix / rhs;

      // Assert
      CollectionAssert.AreEqual(expected, result.MatrixValues, new DoubleComparer());
    }

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
