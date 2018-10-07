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
      return new List<object[]>
      {
        new object[] { 5, 0, 4, 0 },
        new object[] { -23, -8, -4, -8 },
        new object[] { -24, -8, -4, -4 },
        new object[] { -9, -7, -3, -4 },
        new object[] { 9, -7, -3, -6 },
      };
    }

    /// <remarks>
    /// 0. Value
    /// 1. Min
    /// 2. Max
    /// </remarks>
    public static IEnumerable<object[]> GetCtorArgumentExceptionData()
    {
      return new List<object[]>
      {
        new object[] { 0, 1, 0 },
        new object[] { 0, 0, 0 },
        new object[] { 0, int.MinValue, 0 }
      };
    }
  }
}
