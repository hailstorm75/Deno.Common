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
    public int Value { get => _mValue; protected set => _mValue = AdjustValue(value); }
    private int _mValue;

    /// <summary>
    /// Range maximum
    /// </summary>
    public int Max { get; }

    /// <summary>
    /// Range minimum
    /// </summary>
    public int Min { get; }

    /// <summary>
    /// Distance between <see cref="Min"/> and <see cref="Max"/>
    /// </summary>
    private int RangeLen { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="value">Value to hold</param>
    /// <param name="min">Range minimum</param>
    /// <param name="max">Range maximum</param>
    public NumberInRange(int value, int min = int.MinValue, int max = int.MaxValue)
    {
      if (min == max) throw new ArgumentException($"Argument {nameof(min)} cannot be equal to argument {nameof(max)}.");
      if (min > max) throw new ArgumentException($"Argument {nameof(min)} cannot be greater than argument {nameof(max)}.");
      Max = max;
      Min = min;
      RangeLen = System.Math.Abs(Min - Max);
      Value = value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adjusts value to fit given <see cref="Min"/> and <see cref="Max"/>
    /// </summary>
    /// <param name="value">Value to adjust</param>
    /// <returns>Value in range</returns>
    private int AdjustValue(int value)
    {
      if (value > Max)
      {
        var fits = System.Math.Abs(value / RangeLen);
        value -= fits * RangeLen;
        if (Max < 0) value += Min;
      }
      else if (value < Min)
      {
        var fits = System.Math.Abs(value / RangeLen);
        value += fits * RangeLen;
        if (Min < 0) value += Max;
      }

      return value;
    }

    /// <summary>
    /// Converts the <see cref="Value"/> of this instance to its equivalent string representation.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _mValue.ToString();

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

    public int CompareTo(int other) => _mValue.CompareTo(other);

    public int CompareTo(INumberInRange other) => _mValue.CompareTo(other.Value);

    #endregion

    #region IEquatable implementation

    public bool Equals(int other) => _mValue.Equals(other);

    public bool Equals(INumberInRange other)
    {
      Debug.Assert(other != null, nameof(other) + " != null");
      return Min.Equals(other.Min)
          && Max.Equals(other.Max)
          && Value.Equals(other.Value);
    }

    #endregion

    #region IConvertible implementation

    public TypeCode GetTypeCode() => _mValue.GetTypeCode();

    public bool ToBoolean(IFormatProvider provider) => ((IConvertible)_mValue).ToBoolean(provider);

    public char ToChar(IFormatProvider provider) => ((IConvertible)_mValue).ToChar(provider);

    public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)_mValue).ToSByte(provider);

    public byte ToByte(IFormatProvider provider) => ((IConvertible)_mValue).ToByte(provider);

    public short ToInt16(IFormatProvider provider) => ((IConvertible)_mValue).ToInt16(provider);

    public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)_mValue).ToUInt16(provider);

    public int ToInt32(IFormatProvider provider) => ((IConvertible)_mValue).ToInt32(provider);

    public uint ToUInt32(IFormatProvider provider) => ((IConvertible)_mValue).ToUInt32(provider);

    public long ToInt64(IFormatProvider provider) => ((IConvertible)_mValue).ToInt64(provider);

    public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)_mValue).ToUInt64(provider);

    public float ToSingle(IFormatProvider provider) => ((IConvertible)_mValue).ToSingle(provider);

    public double ToDouble(IFormatProvider provider) => ((IConvertible)_mValue).ToDouble(provider);

    public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)_mValue).ToDecimal(provider);

    public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)_mValue).ToDateTime(provider);

    public string ToString(IFormatProvider provider) => _mValue.ToString(provider);

    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)_mValue).ToType(conversionType, provider);

    #endregion

    #region IClonable implementation

    public object Clone() => new NumberInRange(Value, Min, Max);

    #endregion

    #endregion
  }
}
