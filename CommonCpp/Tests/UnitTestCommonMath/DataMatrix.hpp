#pragma once
#include <tuple>

namespace Common::Math::Tests
{
  template <typename T, typename = std::enable_if_t<std::is_arithmetic<T>::value && (std::is_floating_point<T>::value || std::is_integral<T>::value)>>
  class DataMatrix
  {
  public:
    static std::vector<std::tuple<int, int>> GetCtorExceptionData()
    {
      return
      {
        std::make_tuple(-5, 5),
        std::make_tuple(5, -5),
        std::make_tuple(-5, 5),
        std::make_tuple(0, 0),
        std::make_tuple(0, 5),
        std::make_tuple(5, 0)
      };
    }

    static std::vector<std::tuple<std::vector<std::vector<T>>, std::vector<std::vector<T>>>> GetAdditionInvalidClassClassData()
    {
      return
      {
        std::make_tuple(
          std::vector<std::vector<T>>
          {
            std::vector<T> {1, 1, 1},
            std::vector<T> {1, 1, 1}
          },
          std::vector<std::vector<T>>
          {
            std::vector<T> {1, 1},
            std::vector<T> {1, 1},
            std::vector<T> {1, 1}
          })
      };
    }

    static std::vector<std::tuple<std::vector<std::vector<T>>, std::vector<std::vector<T>>, std::vector<std::vector<T>>>> GetAdditionClassClassData()
    {
      return
      {
        std::make_tuple(
          std::vector<std::vector<T>>
          {
            std::vector<T> {0, 1, 2},
            std::vector<T> {9, 8, 7}
          },
          std::vector<std::vector<T>>
          {
            std::vector<T> {6, 5, 4},
            std::vector<T> {3, 4, 5}
          },
          std::vector<std::vector<T>>
          {
            std::vector<T> {6, 6, 6},
            std::vector<T> {12, 12, 12}
          }),
        std::make_tuple(
          std::vector<std::vector<T>>
          {
            std::vector<T> {5, 2},
            std::vector<T> {4, 9},
            std::vector<T> {10, -3}
          },
          std::vector<std::vector<T>>
          {
            std::vector<T> {-11, 0},
            std::vector<T> {7, 1},
            std::vector<T> {-6, -8}
          },
          std::vector<std::vector<T>>
          {
            std::vector<T> {-6, 2},
            std::vector<T> {11, 10},
            std::vector<T> {4, -11}
          })
      };
    }
  };
}