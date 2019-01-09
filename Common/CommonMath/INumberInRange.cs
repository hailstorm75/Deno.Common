using System;

namespace Common.Math
{
  public interface INumberInRange<out T> : ICloneable, IConvertible, IFormattable
  {
    /// <summary>
    /// Range maximum
    /// </summary>
    T Max { get; }

    /// <summary>
    /// Range minimum
    /// </summary>
    T Min { get; }

    /// <summary>
    /// Value in range
    /// </summary>
    T Value { get; }

    /// <summary>
    /// Converts the <see cref="Value"/> property of this instance to its equivalent string representation.
    /// </summary>
    /// <returns></returns>
    string ToString();
  }
}
