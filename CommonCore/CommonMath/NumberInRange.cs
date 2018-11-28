using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using static Common.Math.UniversalNumericOperation;

[assembly: InternalsVisibleTo("UnitTestCommonMath")]
namespace Common.Math
{
  [Serializable]
  public class NumberInRange<T> : INumberInRange<T> where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
  {
    #region Properties

    /// <inheritdoc />
    public T Value { get => m_value; protected set => this.m_value = AdjustValue(value); }
    private T m_value;

    /// <inheritdoc />
    public T Max { get; }

    /// <inheritdoc />
    public T Min { get; }

    #endregion

    #region Fields

    /// <summary>
    /// Distance between <see cref="Min"/> and <see cref="Max"/>
    /// </summary>
    private readonly T m_rangeLen;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="value">Value to hold</param>
    /// <param name="min">Range minimum</param>
    /// <param name="max">Range maximum</param>
    public NumberInRange(T value, T min, T max)
    {
      if (!default(T).IsSignedInteger()) throw new NotSupportedException($"T cannot be of type {typeof(T).Name}");
      if (IsEqual(min, GetMinValue(value))) throw new ArgumentException($"Argumnet {nameof(min)} cannot be equal to {GetMinValue(value)}");
      if (IsEqual(min, max)) throw new ArgumentException($"Argument {nameof(min)} cannot be equal to argument {nameof(max)}.");
      if (IsGreater(min, max)) throw new ArgumentException($"Argument {nameof(min)} cannot be greater than argument {nameof(max)}.");

      Max = max;
      Min = min;
      m_rangeLen = Add<T, int, T>(Abs(Abs(Min).Subtract(Abs(Max))), 1);
      Value = value;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adjusts val to fit given <see cref="Min"/> and <see cref="Max"/>
    /// </summary>
    /// <param name="val">Value to adjust</param>
    /// <returns>Value in range</returns>
    private T AdjustValue(T val)
    {
      if (IsGreaterEqual(val, Min) && IsLessEqual(val, Max)) return val;

      if (IsGreater(val, Min))
      {
        if (IsLess(Min, 0)) return Abs(val.Subtract(Min)).Modulo(m_rangeLen).Add(Min);

        var remainder = val.Modulo(m_rangeLen);
        return IsEqual(remainder, 0) ? Min : remainder.Add(Min);
      }

      //if (IsLess(val, Min))
      {
        var remainder = Abs(val.Subtract(Min)).Modulo(m_rangeLen);
        return IsEqual(remainder, 0) ? Min : m_rangeLen.Subtract(remainder).Add(Min);
      }

      //return Min.Subtract(Max);
    }

    /// <summary>
    /// Adjusts <paramref name="val"/> to fit given <paramref name="min"/> and <paramref name="max"/> range
    /// </summary>
    /// <param name="val">Value to adjust</param>
    /// <param name="min">Range minimum</param>
    /// <param name="max">Range maximum</param>
    /// <returns>Adjusted value</returns>
    public static T AdjustValue(T val, T min, T max) => new NumberInRange<T>(val, min, max).Value;

    /// <inheritdoc cref="INumberInRange{T}" />
    public override string ToString() => m_value.ToString(CultureInfo.InvariantCulture);

    #endregion

    #region Operators

    #region Addition

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator +(T a, NumberInRange<T> b) => a.Add(b.Value);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator +(NumberInRange<T> a, T b) => a + new NumberInRange<T>(b, a.Min, a.Max);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator +(NumberInRange<T> a, NumberInRange<T> b) => a.AdjustValue(a.Value.Add(a.AdjustValue(b.Value)));

    #endregion

    #region Subtraction

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator -(T a, NumberInRange<T> b) => a.Subtract(b.Value);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns></returns>
    public static T operator -(NumberInRange<T> a, T b) => a - new NumberInRange<T>(b, a.Min, a.Max);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator -(NumberInRange<T> a, NumberInRange<T> b) => a.AdjustValue(a.Value.Subtract(a.AdjustValue(b.Value)));

    #endregion

    #region Multiplication

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator *(T a, NumberInRange<T> b) => a.Multiply(b.Value);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator *(NumberInRange<T> a, T b) => a * new NumberInRange<T>(b, a.Min, a.Max);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator *(NumberInRange<T> a, NumberInRange<T> b) => a.AdjustValue(a.Value.Multiply(a.AdjustValue(b.Value)));

    #endregion

    #region Division

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator /(T a, NumberInRange<T> b) => a.Divide(b.Value);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator /(NumberInRange<T> a, T b) => a / new NumberInRange<T>(b, a.Min, a.Max);

    /// <param name="a">Left hand side val</param>
    /// <param name="b">Right hand side val</param>
    /// <returns>Result</returns>
    public static T operator /(NumberInRange<T> a, NumberInRange<T> b) => a.AdjustValue(a.Value.Divide(a.AdjustValue(b.Value)));

    #endregion

    #endregion

    #region Interfaces

    #region IFormattable implementation

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return Value.ToString(format, formatProvider);
    }

    #endregion

    #region IConvertible implementation

    public TypeCode GetTypeCode() => m_value.GetTypeCode();

    public bool ToBoolean(IFormatProvider provider = null) => ((IConvertible)m_value).ToBoolean(provider);

    public char ToChar(IFormatProvider provider = null) => ((IConvertible)m_value).ToChar(provider);

    public sbyte ToSByte(IFormatProvider provider = null) => ((IConvertible)m_value).ToSByte(provider);

    public byte ToByte(IFormatProvider provider = null) => ((IConvertible)m_value).ToByte(provider);

    public short ToInt16(IFormatProvider provider = null) => ((IConvertible)m_value).ToInt16(provider);

    public ushort ToUInt16(IFormatProvider provider = null) => ((IConvertible)m_value).ToUInt16(provider);

    public int ToInt32(IFormatProvider provider = null) => ((IConvertible)m_value).ToInt32(provider);

    public uint ToUInt32(IFormatProvider provider = null) => ((IConvertible)m_value).ToUInt32(provider);

    public long ToInt64(IFormatProvider provider = null) => ((IConvertible)m_value).ToInt64(provider);

    public ulong ToUInt64(IFormatProvider provider = null) => ((IConvertible)m_value).ToUInt64(provider);

    public float ToSingle(IFormatProvider provider = null) => ((IConvertible)m_value).ToSingle(provider);

    public double ToDouble(IFormatProvider provider = null) => ((IConvertible)m_value).ToDouble(provider);

    public decimal ToDecimal(IFormatProvider provider = null) => ((IConvertible)m_value).ToDecimal(provider);

    public DateTime ToDateTime(IFormatProvider provider = null) => ((IConvertible)m_value).ToDateTime(provider);

    public string ToString(IFormatProvider provider) => m_value.ToString(provider);

    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)m_value).ToType(conversionType, provider);

    #endregion

    #region IClonable implementation

    public object Clone() => new NumberInRange<T>(Value, Min, Max);

    #endregion

    #endregion
  }
}
