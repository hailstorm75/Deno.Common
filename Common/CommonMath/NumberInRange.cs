using System;
using System.Diagnostics;

namespace Common.Math
{
  [Serializable]
  public class NumberInRange : INumberInRange
  {
    #region Properties

    /// <summary>
    /// Value in range
    /// </summary>
    public int Value { get => value; protected set => this.value = AdjustValue(value); }
    private int value;

    /// <summary>
    /// Range maximum
    /// </summary>
    public int Max { get; }

    /// <summary>
    /// Range minimum
    /// </summary>
    public int Min { get; }

    #endregion

    #region Fields

    /// <summary>
    /// Distance between <see cref="Min"/> and <see cref="Max"/>
    /// </summary>
    private readonly int rangeLen;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="value">Value to hold</param>
    /// <param name="min">Range minimum</param>
    /// <param name="max">Range maximum</param>
    public NumberInRange(int value, int min = int.MinValue + 1, int max = int.MaxValue)
    {
      if (min == int.MinValue) throw new ArgumentException($"Argumnet {nameof(min)} cannot be equal to {int.MinValue}");
      if (min == max) throw new ArgumentException($"Argument {nameof(min)} cannot be equal to argument {nameof(max)}.");
      if (min > max) throw new ArgumentException($"Argument {nameof(min)} cannot be greater than argument {nameof(max)}.");

      Max = max;
      Min = min;
      rangeLen = System.Math.Abs(System.Math.Abs(Min) - System.Math.Abs(Max)) + 1;
      Value = value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adjusts value to fit given <see cref="Min"/> and <see cref="Max"/>
    /// </summary>
    /// <param name="val">Value to adjust</param>
    /// <returns>Value in range</returns>
    private int AdjustValue(int val)
    {
      if (val >= Min && val <= Max) return val;

      if (val > Min)
      {
        if (Min < 0) return System.Math.Abs(val - Min) % rangeLen + Min;

        var remainder = val % rangeLen;
        return remainder == 0 ? Min : remainder + Min;
      }

      if (val < Min)
      {
        var remainder = System.Math.Abs(val - Min) % rangeLen;
        return remainder == 0 ? Min : rangeLen - remainder + Min;
      }

      return Min - Max;
    }

    /// <summary>
    /// Converts the <see cref="Value"/> of this instance to its equivalent string representation.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => value.ToString();

    #endregion

    #region Operators

    #region Addition

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator +(int a, NumberInRange b) => (a + b.Value);

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator +(NumberInRange a, int b) => (a + new NumberInRange(b, a.Min, a.Max));

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator +(NumberInRange a, NumberInRange b) => a.AdjustValue(a.Value + a.AdjustValue(b.Value));

    #endregion

    #region Subtraction

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator -(int a, NumberInRange b) => (a - b.Value);

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns></returns>
    public static int operator -(NumberInRange a, int b) => (a - new NumberInRange(b, a.Min, a.Max));

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator -(NumberInRange a, NumberInRange b) => a.AdjustValue(a.Value - a.AdjustValue(b.Value));

    #endregion

    #region Multiplication

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator *(int a, NumberInRange b) => (a * b.Value);

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator *(NumberInRange a, int b) => (a * new NumberInRange(b, a.Min, a.Max));

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator *(NumberInRange a, NumberInRange b) => a.AdjustValue(a.Value * a.AdjustValue(b.Value));

    #endregion

    #region Division

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator /(int a, NumberInRange b) => (a / b.Value);

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator /(NumberInRange a, int b) => (a / new NumberInRange(b, a.Min, a.Max));

    /// <param name="a">Left hand side value</param>
    /// <param name="b">Right hand side value</param>
    /// <returns>Result</returns>
    public static int operator /(NumberInRange a, NumberInRange b) => a.AdjustValue(a.Value / a.AdjustValue(b.Value));

    #endregion

    #endregion

    #region Interfaces

    #region IFormattable implementation

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return Value.ToString(format, formatProvider);
    }

    #endregion

    #region IComparable implementation

    public int CompareTo(int other) => value.CompareTo(other);

    public int CompareTo(INumberInRange other) => value.CompareTo(other.Value);

    #endregion

    #region IEquatable implementation

    public bool Equals(int other) => value.Equals(other);

    public bool Equals(INumberInRange other)
    {
      Debug.Assert(other != null, nameof(other) + " != null");
      return Min.Equals(other.Min)
          && Max.Equals(other.Max)
          && Value.Equals(other.Value);
    }

    #endregion

    #region IConvertible implementation

    public TypeCode GetTypeCode() => value.GetTypeCode();

    public bool ToBoolean(IFormatProvider provider) => ((IConvertible)value).ToBoolean(provider);

    public char ToChar(IFormatProvider provider) => ((IConvertible)value).ToChar(provider);

    public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)value).ToSByte(provider);

    public byte ToByte(IFormatProvider provider) => ((IConvertible)value).ToByte(provider);

    public short ToInt16(IFormatProvider provider) => ((IConvertible)value).ToInt16(provider);

    public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)value).ToUInt16(provider);

    public int ToInt32(IFormatProvider provider) => ((IConvertible)value).ToInt32(provider);

    public uint ToUInt32(IFormatProvider provider) => ((IConvertible)value).ToUInt32(provider);

    public long ToInt64(IFormatProvider provider) => ((IConvertible)value).ToInt64(provider);

    public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)value).ToUInt64(provider);

    public float ToSingle(IFormatProvider provider) => ((IConvertible)value).ToSingle(provider);

    public double ToDouble(IFormatProvider provider) => ((IConvertible)value).ToDouble(provider);

    public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)value).ToDecimal(provider);

    public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)value).ToDateTime(provider);

    public string ToString(IFormatProvider provider) => value.ToString(provider);

    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)value).ToType(conversionType, provider);

    #endregion

    #region IClonable implementation

    public object Clone() => new NumberInRange(Value, Min, Max);

    #endregion

    #endregion
  }
}
