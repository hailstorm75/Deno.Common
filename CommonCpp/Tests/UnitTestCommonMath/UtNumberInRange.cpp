#include "../catch.hpp"
#include "../../CommonMath/NumberInRange.hpp"
#include "DataNumberInRange.hpp"

using namespace Common::Math;
using namespace Common::Math::Tests;

// INITIALIZATION

TEMPLATE_TEST_CASE("Class be initialized", "[Constructor][Template]", unsigned short, unsigned, unsigned long, short, int, long)
{
  REQUIRE_NOTHROW(NumberInRange<TestType>(0, 1, 2));
}

TEST_CASE("Initialize with invalid arguments", "[Constructor]")
{
  REQUIRE_THROWS_AS(NumberInRange<int>(0, 5, 1), std::invalid_argument);
}

TEMPLATE_TEST_CASE("Value is adjusted to range", "[Constructor][Template]", short, int, long)
{
  for (const auto & [value, min, max, expected] : DataNumberInRange<TestType>::GetCtorAdjustRangeData())
  {
    SECTION("Initialize with Value: " + std::to_string(value) + ", Min: " + std::to_string(min) + ", Max: " + std::to_string(max))
    {
      auto numberInRange = NumberInRange<TestType>(value, min, max);
      REQUIRE(numberInRange.GetValue() == expected);
    }
  }
}

// OPERATORS

TEST_CASE("Add Int to Class", "[Operator]")
{
  // Arrange
  const auto numberInRange = NumberInRange<int>(5, 0, 4);

  // Act
  auto result = numberInRange + 6;

  // Assert
  REQUIRE(result == 1);
}

TEST_CASE("Add Class to Class", "[Operator]")
{
  // Arrange
  const auto numberInRangeA = NumberInRange<int>(5, 0, 4);
  const auto numberInRangeB = NumberInRange<int>(6, 0, 4);

  // Act
  auto result = numberInRangeA + numberInRangeB;

  // Assert
  REQUIRE(result == 1);
}

TEST_CASE("Subtract Int from Class", "[Operator]")
{
  // Arrange
  const auto numberInRange = NumberInRange<int>(5, 0, 4);

  // Act
  auto result = numberInRange - 6;

  // Assert
  REQUIRE(result == 4);
}

TEST_CASE("Subtract Class from Class", "[Operator]")
{
  // Arrange
  const auto numberInRangeA = NumberInRange<int>(5, 0, 4);
  const auto numberInRangeB = NumberInRange<int>(6, 0, 4);

  // Act
  auto result = numberInRangeA - numberInRangeB;

  // Assert
  REQUIRE(result == 4);
}

TEST_CASE("Multiply Class by Int", "[Operator]")
{
  // Arrange
  const auto numberInRange = NumberInRange<int>(9, 0, 4);

  // Act
  auto result = numberInRange * 7;

  // Assert
  REQUIRE(result == 3);
}

TEST_CASE("Multiply Class by Class", "[Operator]")
{
  // Arrange
  const auto numberInRangeA = NumberInRange<int>(9, 0, 4);
  const auto numberInRangeB = NumberInRange<int>(7, 0, 4);

  // Act
  auto result = numberInRangeA * numberInRangeB;

  // Assert
  REQUIRE(result == 3);
}

TEST_CASE("Divide Class by Int", "[Operator]")
{
  // Arrange
  const auto numberInRange = NumberInRange<int>(9, 0, 4);

  // Act
  auto result = numberInRange / 7;

  // Assert
  REQUIRE(result == 2);
}

TEST_CASE("Divide Class by Class", "[Operator]")
{
  // Arrange
  const auto numberInRangeA = NumberInRange<int>(9, 0, 4);
  const auto numberInRangeB = NumberInRange<int>(7, 0, 4);

  // Act
  auto result = numberInRangeA / numberInRangeB;

  // Assert
  REQUIRE(result == 2);
}
