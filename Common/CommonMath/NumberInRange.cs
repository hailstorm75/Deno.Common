using System;

namespace Common.Math
{
  // TODO Generalize for all numeric types
  public class NumberInRange
  {
    #region Properties

    /// <summary>
    /// Value in range
    /// </summary>
    public int Value { get; protected set; }

    /// <summary>
    /// Range maximum
    /// </summary>
    public int Max { get; protected set; }

    /// <summary>
    /// Range minimum
    /// </summary>
    public int Min { get; protected set; }

    #endregion

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public NumberInRange(int value, int min = int.MinValue, int max = int.MaxValue)
    {
      if (min == max) throw new ArgumentException($"Argument {nameof(min)} cannot be equal to argument {nameof(max)}.");
      if (min > max) throw new ArgumentException($"Argument {nameof(min)} cannot be greater than argument {nameof(max)}.");
      SetValue(value);
    }

    /// <summary>
    /// Sets the <see cref="Value"/> property
    /// </summary>
    /// <param name="value">Value to set</param>
    public void SetValue(int value)
    {
      Value = value > Max ? (value - Max) :
              value < Min ? (value + Max) :
              value;
    }

    #region Operators

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int operator +(int a, NumberInRange b) => (a + b.Value);

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int operator -(int a, NumberInRange b) => (a - b.Value);

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int operator +(NumberInRange a, int b) => (b + a.Value) % a.Max - a.Min;

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int operator -(NumberInRange a, int b) => (b - a.Value) % a.Max - a.Min;

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int operator +(NumberInRange a, NumberInRange b) => (b.Value + a.Value) % a.Max - a.Min;

    /// <summary>
    ///
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int operator -(NumberInRange a, NumberInRange b) => (b.Value - a.Value) % a.Max - a.Min;

    #endregion
  }
}
