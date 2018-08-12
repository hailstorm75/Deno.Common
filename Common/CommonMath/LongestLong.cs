using System;
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
    public List<long> Values { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public LongestLong() => Values = new List<long>();

    /// <summary>
    /// Variadic constructor
    /// </summary>
    /// <param name="values">Value to initialize with</param>
    public LongestLong(params long[] values) => Values = values.ToList();

    /// <summary>
    ///
    /// </summary>
    /// <param name="list"></param>
    public LongestLong(List<long> list) => Values = list ?? new List<long>();

    #endregion

    #region Operators

    #region Addition

    public static LongestLong operator +(LongestLong a, long b)
    {
      var result = new List<long>();
      Add(a.Values, new List<long>() { b }, ref result, a.Values.Count, false);
      return new LongestLong(result);
    }

    public static LongestLong operator +(LongestLong a, LongestLong b)
    {
      var result = new List<long>();
      if (a.Values.Count > b.Values.Count)
        Add(a.Values, b.Values, ref result, 0, false);
      else
        Add(b.Values, a.Values, ref result, 0, false);

      return new LongestLong(result);
    }

    public static long operator +(long a, LongestLong b)
    {
      // TODO This can overflow
      return a + b.Values[0];
    }

    #endregion

    #region Subtraction

    public static LongestLong operator -(LongestLong a, long b)
    {
      throw new NotImplementedException();
    }

    public static LongestLong operator -(LongestLong a, LongestLong b)
    {
      throw new NotImplementedException();
    }

    public static long operator -(long a, LongestLong b)
    {
      // TODO This can underflow
      return a - b.Values[0];
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

    /// <summary>
    /// Recurvely adds list <paramref name="x"/> with list <paramref name="y"/>
    /// </summary>
    /// <param name="x">Left hand side argument</param>
    /// <param name="y">Right hand side argument</param>
    /// <param name="result">Operation result</param>
    /// <param name="index">Recursion iteration index</param>
    /// <param name="carryover">Calculation carryover</param>
    private static void Add(IReadOnlyList<long> x, IReadOnlyList<long> y, ref List<long> result, int index, bool carryover) {
      // We should actually be working base max value of long.
      // For sake of example, let us say that y is the shorter number
      // We should consider each pair of longs individually
      // Index should ALREADY be the last
      // Before we start we should deal with any carry overs
      // IE DO WE NEED TO ADD ANOTHER LIST TO THE START

      if (index >= 0)
      {
        var temp = y[index] + x[index];

        if (temp <= long.MaxValue && temp >= long.MinValue)
        {
          result.Insert(0, temp + (carryover ? 1 : 0));
          Add(x, y, ref result, --index, false);
        }
        else
        {
          result.Insert(0, temp - long.MaxValue + (carryover ? 1 : 0));
          Add(x, y, ref result, --index, true);
        }
      }
      else
      {
        //bring down remaining from the longer one, which we are taking to be x
        if (x.Count == y.Count && carryover)
          result.Insert(0, 1);
        else if (x.Count > y.Count)
          for (int i = x.Count - y.Count; i > 0; i--)
            result.Insert(0, x[i - 1]);
      }
    }

    private List<long> Multiply(List<long> x, List<long> y) {
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

    public int CompareTo(LongestLong y) {
      if (Values.Count > y.Values.Count) return 1;
      if (Values.Count > y.Values.Count) return -1;

      for (int i = Values.Count - 1; i >= 0; --i)
      {
        if (Values[i] == y.Values[i]) continue;
        return Values[i] > y.Values[i] ? 1 : -1;
      }

      return 0;
    }

    public override string ToString() {
      throw new NotImplementedException();
    }
  }
}
