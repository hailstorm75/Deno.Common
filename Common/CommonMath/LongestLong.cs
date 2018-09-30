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
    public LinkedList<long> Values { get; }

    public bool Positive { get; }

    #endregion

    #region Fields

    private static readonly long MAX = long.Parse(1 + new string('0', (long.MaxValue / 10).ToString().Length));

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public LongestLong() => Values = new LinkedList<long>();

    /// <summary>
    /// Variadic constructor
    /// </summary>
    /// <param name="list">Value to initialize with</param>
    public LongestLong(params long[] list)
    {
      if (!ValidateNumbers(list)) throw new ArgumentException($"Only the first element from argument {nameof(list)} can have a negative value.");
      Positive = list?.First() >= 0;
      Values = AdjustList(new LinkedList<long>(list.Reverse()));
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="list">List of values to initialize from</param>
    public LongestLong(List<long> list)
    {
      if (!ValidateNumbers(list)) throw new ArgumentException($"Only the first element from argument {nameof(list)} can have a negative value.");
      Positive = list?.First() >= 0;
      Values = list != null ? AdjustList(new LinkedList<long>(list.AsEnumerable().Reverse())) : new LinkedList<long>(new long[] { 0 });
    }

    #endregion

    #region Operators

    #region Addition

    public static LongestLong operator +(LongestLong a, long b)
    {
      var result = new List<long>();
      Add(a.Values, new LongestLong(b).Values, ref result, a.Values.Count, false);
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
      return a + b.Values.First.Value;
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
      return a - b.Values.First.Value;
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

    private static bool ValidateNumbers(IEnumerable<long> list) => list.Skip(1).All(num => num >= 0);

    private static LinkedList<long> AdjustList(LinkedList<long> list)
    {
      long carry = 0;
      list.Last.Value = System.Math.Abs(list.Last.Value);
      //foreach (var item in list)
      //{
      //  if (item < MAX)
      //  {
      //    item += carry;
      //    carry = 0;
      //    continue;
      //  }

      //  var tmp = item / MAX;
      //  item -= tmp * MAX + carry;
      //  carry = tmp;
      //}

      //var en = list.GetEnumerator();

      //while (en.MoveNext())
      //{

      //}

      if (carry != 0)
        list.AddFirst(carry);

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
    private static void Add(IReadOnlyCollection<long> x, IReadOnlyCollection<long> y, ref List<long> result, int index, bool carryover)
    {

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

      sb.Append(Positive ? string.Empty : "-").Append(string.Concat(Values));

      return sb.ToString();
    }
  }
}
