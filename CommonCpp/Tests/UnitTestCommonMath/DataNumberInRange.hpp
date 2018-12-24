#pragma once
#include <type_traits>
#include <vector>
#include <tuple>

namespace Common::Math::Tests
{
  template <typename T, std::enable_if_t<std::is_arithmetic<T>::value && std::is_integral<T>::value>* = nullptr>
  class DataNumberInRange
  {
  public:
    static std::vector<std::tuple<T, T, T>> GetCtorArgumentExceptionData()
    {
      return
      {
        std::make_tuple(0, 1, 0),
        std::make_tuple(0, 0, 0),
      };
    }

    static std::vector<std::tuple<T, T, T, T>> GetCtorAdjustRangeData()
    {
      return
      {
        std::make_tuple(5, 0, 4, 0),
        std::make_tuple(0, 0, 4, 0),
        std::make_tuple(-23, -8, -4, -8)
      };
    }
  };
}
