using System;
using System.Collections;
using Common.Math.Matricies;
using Common.Math.Tests.Data;
using NUnit.Framework;
using UnitTestConstants;

namespace Common.Math.Tests
{
  [TestFixture]
  public class UtMatrix
  {
    #region Initialization

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeLengthHeight()
    {
      // Act
      var unused = new Matrix<double>(5, 3);
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeArray()
    {
      // Act
      var unused = new Matrix<double>(new double[,]
      {
        { 1, 2, 5 },
        { 3, 5, 8 }
      });
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    [TestCase(0)]
    [TestCase(-1)]
    public void InitializeInvlidSize(int size)
    {
      // Assert
      Assert.Throws<ArgumentException>(() =>
      {
        var unused = new Matrix<double>(size, false);
      });
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeInvalidType1()
    {
      // Assert
      Assert.Throws<NotSupportedException>(() =>
      {
        var unused = new Matrix<DateTime>(5, 5);
      });
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeInvalidType2()
    {
      // Assert
      Assert.Throws<NotSupportedException>(() =>
      {
        var unused = new Matrix<DateTime>(5, true);
      });
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeInvalidType3()
    {
      // Assert
      Assert.Throws<NotSupportedException>(() =>
      {
        var unused = new Matrix<DateTime>(new[,] { { new DateTime(), } });
      });
    }

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetCtorExceptionData))]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeException(int length, int height)
    {
      // Assert
      Assert.Throws<ArgumentException>(() =>
      {
        var unused = new Matrix<double>(length, height);
      });
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeNull()
    {
      // Assert
      Assert.Throws<ArgumentException>(() =>
      {
        var unused = new Matrix<double>(null);
      });
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeZeroDimension()
    {
      // Assert
      Assert.Throws<ArgumentException>(() =>
      {
        var unused = new Matrix<double>(new double[,] { });
      });
    }

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetCtorInvertableData))]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeInvertable(int length, int height)
    {
      // Arrange
      var matrix = new Matrix<double>(length, height);

      // Assert
      Assert.AreEqual(Matrix<double>.Type.Invertable, matrix.MatrixType);
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeIdentity()
    {
      // Arrange
      var matrix = new Matrix<double>(DataMatrix.InvertableMatrix);

      // Assert
      Assert.AreEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeNonIdentity()
    {
      // Arrange
      var matrix = new Matrix<double>(new double[,] { { 1, 2, 3 }, { 1, 2, 3 } });

      // Assert
      Assert.AreNotEqual(Matrix<double>.Type.Invertable | Matrix<double>.Type.Identity, matrix.MatrixType);
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeIdentity2()
    {
      // Arrange
      var matrix = new Matrix<double>(3, true);

      // Assert
      CollectionAssert.AreEqual(DataMatrix.InvertableMatrix, matrix.MatrixValues);
    }

    #endregion

    #region Methods

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetInvertData))]
    [Category(Constants.METHOD)]
    public void Invert(double[,] from, double[,] to)
    {
      // Arrange
      var matrix = new Matrix<double>(from);

      // Act
      var result = matrix.GetInverse();

      // Assert
      CollectionAssert.AreEqual(to, result.MatrixValues, new DoubleComparer());
    }

    [Test]
    [Category(Constants.METHOD)]
    public void InvertInvalid()
    {
      // Arrange
      var matrix = new Matrix<double>(2, 3);

      // Act
      Assert.Throws<Matrix<double>.InvertableMatrixOperationException>(() =>
      {
        var unused = matrix.GetInverse();
      });
    }

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetNonIdentityMatrixData))]
    [Category(Constants.METHOD)]
    public void DeterminantException(double[,] from)
    {
      // Arrange
      var matrix = new Matrix<double>(from);

      // Act
      Assert.Throws<Matrix<double>.InvertableMatrixOperationException>(() =>
      {
        var unused = matrix.GetDeterminant();
      });
    }

    [Test]
    [Category(Constants.METHOD)]
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

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetAdditionInvalidClassClassData))]
    [Category(Constants.OPERATOR)]
    public void AdditionInvalidClassClass(double[,] lhs, double[,] rhs)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      Assert.Throws<Matrix<double>.MatrixDimensionException>(() =>
      {
        var unused = matrixA + matrixB;
      });
    }

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetAdditionClassClassData))]
    [Category(Constants.OPERATOR)]
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

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetSubtractionInvalidClassClassData))]
    [Category(Constants.OPERATOR)]
    public void SubtractionInvalidClassClass(double[,] lhs, double[,] rhs)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      Assert.Throws<Matrix<double>.MatrixDimensionException>(() =>
      {
        var unused = matrixA - matrixB;
      });
    }

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetSubtractionClassClassData))]
    [Category(Constants.OPERATOR)]
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

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetMultiplicationValueClassData))]
    [Category(Constants.OPERATOR)]
    public void MultiplicationValueClass(double lhs, double[,] rhs, double[,] expected)
    {
      // Arrange
      var matrix = new Matrix<double>(rhs);

      // Act
      var result = lhs * matrix;

      // Assert
      CollectionAssert.AreEqual(expected, result.MatrixValues, new DoubleComparer());
    }

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetMultiplicationClassClassData))]
    [Category(Constants.OPERATOR)]
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

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetMultiplicationInvalidClassClassData))]
    [Category(Constants.OPERATOR)]
    public void MultiplicationInvalidClassClass(double[,] lhs, double[,] rhs)
    {
      // Arrange
      var matrixA = new Matrix<double>(lhs);
      var matrixB = new Matrix<double>(rhs);

      // Act
      Assert.Throws<Matrix<double>.MatrixDimensionException>(() =>
      {
        var unused = matrixA * matrixB;
      });
    }

    #endregion

    #region Division

    [Test, TestCaseSource(typeof(DataMatrix), nameof(DataMatrix.GetDivisionClassValueData))]
    [Category(Constants.OPERATOR)]
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