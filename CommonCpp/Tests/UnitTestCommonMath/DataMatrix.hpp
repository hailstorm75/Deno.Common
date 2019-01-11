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
  };
}