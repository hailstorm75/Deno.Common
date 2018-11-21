using System.Collections.Generic;

namespace Common.Math.Tests.Data
{
  internal class DataNumberInRange
  {
    /// <remarks>
    /// 0. Value
    /// 1. Min
    /// 2. Max
    /// 3. Expected
    /// </remarks>
    public static IEnumerable<object[]> GetCtorAdjustRangeData()
    {
      yield return new object[] { 5, 0, 4, 0 };
      yield return new object[] { -23, -8, -4, -8 };
      yield return new object[] { -24, -8, -4, -4 };
      yield return new object[] { -9, -7, -3, -4 };
      yield return new object[] { 9, -7, -3, -6 };
      yield return new object[] { 0, 0, 4, 0 };
    }

    /// <remarks>
    /// 0. Value
    /// 1. Min
    /// 2. Max
    /// </remarks>
    public static IEnumerable<object[]> GetCtorArgumentExceptionData()
    {
      yield return new object[] { 0, 1, 0 };
      yield return new object[] { 0, 0, 0 };
      yield return new object[] { 0, int.MinValue, 0 };
    }
  }
}
