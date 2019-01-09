#pragma once
#include <type_traits>
#include <stdexcept>
#include <sstream>

#define NAMEOF(x) std::string(#x)

namespace Common::Math
{
  /**
   * \brief Keeps an integer value in a given range
   * \tparam T Type of value. Anything but integer types are prohibited.
   */
  template <typename T, std::enable_if_t<std::is_arithmetic<T>::value && std::is_integral<T>::value>* = nullptr>
  class NumberInRange
  {
    /**
     * \brief Range minimum
    */
    const T m_min;
    /**
     * \brief Range maximum
    */
    const  T m_max;
    /**
    * \brief Distance between the Minimum and Maximum
    */
    const T m_rangeLen;

    /**
     * \brief Value in range
    */
    T m_value;

    static T Abs(const T & value)
    {
      if (value >= 0)
        return value;
      return value * -1;
    }

    static T CalcRangeLen(const T & min, const T & max)
    {
      const T a = Abs(min);
      const T b = Abs(max);

      return (a > b ? a - b : b - a) + 1;
    }

  public:
    /**
     * \brief Default constructor
     * \param value Value to hold
     * \param min Range minimum
     * \param max Range maximum
     */
    NumberInRange(const T & value, const T & min, const T & max)
      : m_min(min), m_max(max), m_rangeLen(CalcRangeLen(min, max)), m_value(AdjustValue(value))
    {
      static_assert(!std::is_same<T, double>::value && !std::is_same<T, float>::value, "NumberInRange: T cannot be of a floating point type.");
      if (min == max) throw std::invalid_argument("Argument " + NAMEOF(min) + " cannot be equal to argument " + NAMEOF(max) + ".");
      if (min > max) throw std::invalid_argument("Argument " + NAMEOF(min) + " cannot be greater than argument " + NAMEOF(max) + ".");
    }

    /**
     * \brief Getter for the Min property
     * \return Range Minimum
     */
    const T & GetMin() const { return m_min; }

    /**
     * \brief Getter for the Max property
     * \return Range Maximum
     */
    const T & GetMax() const { return m_max; }

    /**
     * \brief Getter for the Value property
     * \return Value
     */
    const T & GetValue() const { return m_value; }
    /**
     * \brief Setter for the Value property
     * \param value New value to set
     */
    void SetValue(const T & value) { m_value = AdjustValue(value); }

    static T AdjustValue(const T & value, const T & min, const T & max)
    {
      return NumberInRange<T>(value, min, max).GetValue();
    }

    T operator +(const T & other) const noexcept
    {
      return AdjustValue(m_value + AdjustValue(other, m_min, m_max), m_min, m_max);
    }
    T operator +(const NumberInRange<T> & other) const noexcept
    {
      return AdjustValue(m_value + AdjustValue(other.m_value, m_min, m_max), m_min, m_max);
    }

    T operator -(const T & other) const noexcept
    {
      return AdjustValue(m_value - AdjustValue(other, m_min, m_max), m_min, m_max);
    }
    T operator -(const NumberInRange<T> & other) const noexcept
    {
      return AdjustValue(m_value - AdjustValue(other.m_value, m_min, m_max), m_min, m_max);
    }

    T operator *(const T & other) const noexcept
    {
      return AdjustValue(m_value * AdjustValue(other, m_min, m_max), m_min, m_max);
    }
    T operator *(const NumberInRange<T> & other) const noexcept
    {
      return AdjustValue(m_value * AdjustValue(other.m_value, m_min, m_max), m_min, m_max);
    }

    T operator /(const T & other) const
    {
      return AdjustValue(m_value / AdjustValue(other, m_min, m_max), m_min, m_max);
    }
    T operator /(const NumberInRange<T> & other) const
    {
      return AdjustValue(m_value / AdjustValue(other.m_value, m_min, m_max), m_min, m_max);
    }

    T operator %(const T & other) const
    {
      return AdjustValue(m_value % AdjustValue(other, m_min, m_max), m_min, m_max);
    }
    T operator %(const NumberInRange<T> & other) const
    {
      return AdjustValue(m_value % AdjustValue(other.m_value, m_min, m_max), m_min, m_max);
    }

    std::string ToString() const
    {
      std::ostringstream oss;
      oss << m_value;
      return oss.str();
    }

  private:
    T AdjustValue(const T & value) const noexcept
    {
      if (value >= m_min && value <= m_max)
        return value;

      if (value > m_min)
      {
        if (m_min < 0)
          return (Abs(value - m_min) % m_rangeLen) + m_min; // TODO Abs
        auto remainder = value % m_rangeLen;
        return remainder + m_min;
      }

      auto remainder = Abs(value - m_min) % m_rangeLen;
      return remainder == 0 ? m_min : m_rangeLen - remainder + m_min;
    }
  };
}
