#include<vector>
#include "../catch.hpp"
#include "../../CommonMath/Matrix.hpp"
#include "DataMatrix.hpp"

using namespace Common::Math;
using namespace Common::Math::Tests;

template <typename T, typename = std::enable_if_t<std::is_arithmetic<T>::value && (std::is_floating_point<T>::value || std::is_integral<T>::value)>>
bool CompareVectors(std::vector<T> a, std::vector<T> b)
{
  if (a.size() != b.size()) return false;

  for (size_t i = 0; i < a.size(); i++)
    if (a[i] != Approx(b[i]))
      return false;

  return true;
}

template <typename T, typename = std::enable_if_t<std::is_arithmetic<T>::value && (std::is_floating_point<T>::value || std::is_integral<T>::value)>>
bool Compare2DVectors(std::vector<std::vector<T>> a, std::vector<std::vector<T>> b)
{
  if (a.size() != b.size()) return false;

  for (size_t i = 0; i < a.size(); i++)
    if (!CompareVectors(a[i], b[i]))
      return false;

  return true;
}

// INITIALIZATION

TEMPLATE_TEST_CASE("Class can be initialized with given size", "[Constructor][Template]", unsigned short, unsigned, unsigned long, short, int, long, double, float)
{
  REQUIRE_NOTHROW(Matrix<TestType>(5, 3));
}

TEMPLATE_TEST_CASE("Class can be initialized from given array", "[Constructor][Template]", unsigned short, unsigned, unsigned long, short, int, long, double, float)
{
  std::vector<std::vector<TestType>> data(2);
  data[0] = { 1, 2, 5 };
  data[1] = { 3, 5, 8 };

  REQUIRE_NOTHROW(Matrix<TestType>(data));
}

TEMPLATE_TEST_CASE("Class won't initialize with invalid size.", "[Constructor][Template]", unsigned short, unsigned, unsigned long, short, int, long, double, float)
{
  REQUIRE_THROWS_AS(Matrix<TestType>(-1, false), std::invalid_argument);
}

TEMPLATE_TEST_CASE("Class won't initialize with invalid sizes.", "[Constructor][Template]", short, int, long, double, float)
{
  for (const auto &[length, height] : DataMatrix<TestType>::GetCtorExceptionData())
  {
    SECTION("Initialize with Length: " + std::to_string(length) + ", Height: " + std::to_string(height))
    {
      REQUIRE_THROWS_AS(Matrix<TestType>(length, height), std::invalid_argument);
    }
  }
}

TEMPLATE_TEST_CASE("Cannot add matricies with non-matching dimensions", "[Operator][Template]", unsigned short, unsigned, unsigned long, short, int, long, double, float)
{
  for (const auto &[dataLeft, dataRight] : DataMatrix<TestType>::GetAdditionInvalidClassClassData())
  {
    SECTION("Matrix A size: [" + std::to_string(dataLeft.size()) + ", " + std::to_string(dataLeft[0].size()) + "]. Matrix B size: [" + std::to_string(dataRight.size()) + ", " + std::to_string(dataRight[0].size()) + "].")
    {
      const Matrix<TestType> matrixA(dataLeft);
      const Matrix<TestType> matrixB(dataRight);

      REQUIRE_THROWS_AS(matrixA + matrixB, MatrixDimensionException);
    }
  }
}

TEMPLATE_TEST_CASE("Add two matricies together", "[Operator][Template]", short, int, long, double, float)
{
  for (const auto &[dataLeft, dataRight, expected] : DataMatrix<TestType>::GetAdditionClassClassData())
  {
    SECTION("Adding matricies...")
    {
      const Matrix<TestType> matrixA(dataLeft);
      const Matrix<TestType> matrixB(dataRight);

      const auto result = matrixA + matrixB;

      REQUIRE(Compare2DVectors<TestType>(result.GetMatrixValues(), expected));
    }
  }
}
