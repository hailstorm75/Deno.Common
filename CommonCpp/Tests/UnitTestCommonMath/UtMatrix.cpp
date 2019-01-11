#include<vector>
#include "../catch.hpp"
#include "../../CommonMath/Matrix.hpp"
#include "DataMatrix.hpp"

using namespace Common::Math;
using namespace Common::Math::Tests;

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

TEST_CASE("Class won't initialize with invalid sizes.", "[Constructor][Template]")
{
  for (const auto &[length, height] : DataMatrix<double>::GetCtorExceptionData())
  {
    SECTION("Initialize with Length: " + std::to_string(length) + ", Height: " + std::to_string(height))
    {
      REQUIRE_THROWS_AS(Matrix<double>(length, height), std::invalid_argument);
    }
  }
}
