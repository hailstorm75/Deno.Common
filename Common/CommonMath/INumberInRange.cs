using System;

namespace Common.Math
{
  public interface INumberInRange : ICloneable, IConvertible, IFormattable, IComparable<int>, IComparable<INumberInRange>, IEquatable<int>, IEquatable<INumberInRange>
  {
    /// <summary>
    /// Range maximum
    /// </summary>
    int Max { get; }
    /// <summary>
    /// Range minimum
    /// </summary>
    int Min { get; }
    /// <summary>
    /// Value in range
    /// </summary>
    int Value { get; }

    /// <summary>
    /// Converts the <see cref="Value"/> of this instance to its equivalent string representation.
    /// </summary>
    /// <returns></returns>
    string ToString();
  }
}