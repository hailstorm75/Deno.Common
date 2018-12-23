#include <type_traits>

namespace Common::Math
{
  template <class T,
  class = class std::enable_if<std::is_arithmetic<T>::value && std::is_integral<T>::value, T>::type>
  class NumberInRange
  {
    /*
     * \brief Value in range
    */
    T m_value;
    /*
     * \brief Range minimum
    */
    T m_min;
    /*
     * \brief Range maximum
    */
    T m_max;

    const T m_rangeLen;

  public:
    NumberInRange<T>(const T &value, const T &min, const T &max)
      : m_value(value), m_min(min), m_max(max), m_rangeLen(max /*TODO*/)
    {
      if (min > max) throw std::invalid_argument("Argument min cannot be greater than argument max.");
    }

    ~NumberInRange<T>() {}

    const T & GetMin() const { return m_min; }

    const T & GetMax() const { return m_max; }

    const T & GetValue() const { return m_value; }
    const T & SetValue(const T & value) { m_value = AdjustValue(value); }

  private:
    const T & AdjustValue(const T & value)
    {
      if (value >= m_min && value <= m_max)
        return value;

      if (value > m_min)
      {
        if (m_min < 0)
          return ((value - m_min) % m_rangeLen) + m_min; // TODO Abs
        auto remainder = value & m_rangeLen;
        return remainder + m_min;
      }
      else
      {
        auto remainder = (value - m_min) % m_rangeLen;
        return remainder == 0 ? m_min : m_rangeLen - remainder + m_min;
      }
    }
  };
}
// namespace Common::Math
