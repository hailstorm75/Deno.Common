using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Common.Math
{
  [Serializable]
  public class LongestLong
  {
    #region Properties

    /// <summary>
    /// Number container
    /// </summary>
    public List<long> Values { get; }

    public bool Positive { get; }

    #endregion

    #region Fields

    public static readonly long MAX_VALUE = long.Parse("1" + new string('0', (long.MaxValue / 10).ToString().Length));

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public LongestLong() => Values = new List<long>();

    /// <summary>
    /// Variadic constructor
    /// </summary>
    /// <param name="list">Value to initialize with</param>
    public LongestLong(params long[] list)
    {
      if (!ValidateNumbers(list)) throw new ArgumentException($"Only the first element from argument {nameof(list)} can have a negative value.");
      Positive = list?.First() >= 0;
      Values = AdjustList(list.Reverse().ToList());
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="list">List of values to initialize from</param>
    public LongestLong(List<long> list)
    {
      if (!ValidateNumbers(list)) throw new ArgumentException($"Only the first element from argument {nameof(list)} can have a negative value.");
      Positive = list?.First() >= 0;
      // TODO Optimize
      Values = list != null ? AdjustList(list.AsEnumerable().Reverse().ToList()) : new List<long>{ 0 };
    }

    #endregion

    #region Operators

    #region Addition

    public static LongestLong operator +(LongestLong a, long b) => a + new LongestLong(b);

    public static LongestLong operator +(LongestLong a, LongestLong b)
    {
      var result = new List<long>();
      Add(b.Values, a.Values, ref result, 0, false);

      return new LongestLong(result);
    }

    public static long operator +(long a, LongestLong b)
    {
      // TODO This can overflow
      return a + b.Values.First();
    }

    #endregion

    #region Subtraction

    public static LongestLong operator -(LongestLong a, long b) => a - new LongestLong(b);

    public static LongestLong operator -(LongestLong a, LongestLong b)
    {
      throw new NotImplementedException();
    }

    public static long operator -(long a, LongestLong b)
    {
      // TODO This can underflow
      return a - b.Values.First();
    }

    #endregion

    #region Multiplication

    public static LongestLong operator *(LongestLong a, long b)
    {
      throw new NotImplementedException();
    }

    public static LongestLong operator *(LongestLong a, LongestLong b)
    {
      throw new NotImplementedException();
    }

    public static long operator *(long a, LongestLong b)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Division

    public static LongestLong operator /(LongestLong a, long b)
    {
      throw new NotImplementedException();
    }

    public static LongestLong operator /(LongestLong a, LongestLong b)
    {
      throw new NotImplementedException();
    }

    public static long operator /(long a, LongestLong b)
    {
      throw new NotImplementedException();
    }

    #endregion

    #endregion

    #region Methods

    private static void LeftArgLargest(ref LongestLong x, ref LongestLong y)
    {
      if (x.Values.Count > y.Values.Count) return;

      var tmp = x;
      x = y;
      y = tmp;
    }

    private static bool ValidateNumbers(IEnumerable<long> list) => list.Skip(1).All(num => num >= 0);

    private static List<long> AdjustList(List<long> list)
    {
      long carry = 0;

      list[list.Count - 1] = System.Math.Abs(list.Last());
      for (var i = 0; i < list.Count; ++i)
      {
        if (list[i] < MAX_VALUE)
        {
          list[i] += carry;
          carry = 0;
          continue;
        }

        var tmp = list[i] / MAX_VALUE;
        list[i] -= tmp * MAX_VALUE - carry;
        carry = tmp;
      }

      if (carry != 0)
        list.Add(carry);

      return list;
    }

    /// <summary>
    /// Recurvely adds list <paramref name="x"/> with list <paramref name="y"/>
    /// </summary>
    /// <param name="x">Left hand side argument</param>
    /// <param name="y">Right hand side argument</param>
    /// <param name="result">Operation result</param>
    /// <param name="index">Recursion iteration index</param>
    /// <param name="carryover">Calculation carryover</param>
    private static void Add(LongestLong x, LongestLong y)
    {
      LeftArgLargest(ref x, ref y);
    }

    private List<long> Multiply(List<long> x, List<long> y)
    {
      if (x.Count > y.Count) throw new ArgumentException($"List {nameof(x)} must contain less elements than {nameof(y)}");

      // Multiplying is just adding y times
      // Under the assumption that the number of elements in y is less than the number of elements in x
      // So now we actually need to access the number represented by list Y.
      var result = new List<long>();

      // TODO This solution is invalid
      //for (int i = 0; i < y.Count; i++)
      //  for (int j = 0; j < y[i] * (int)System.Math.Pow(long.MaxValue, (y.Count - i - 1)); j++)
      //    product = Adding(product, Adding(x, x, x.Count, false), product.Count, false);

      return result;
    }

    #endregion

    public override string ToString()
    {
      var sb = new StringBuilder
      {
        Capacity = 1 + Values.Count
      };

      sb.Append(Positive ? string.Empty : "-").Append(string.Concat(Values.AsEnumerable().Reverse()));

      return sb.ToString();
    }
  }
}
