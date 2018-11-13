using System.Collections.Generic;

namespace Common.Data.Tests.Data
{
  internal class DataDfaMinimizer
  {
    internal static IEnumerable<object[]> GetTrieData()
    {
      yield return new object[]
      {
        new [] { "234","2301","501", "01" },
        "6 8 4 1\n4 2 3\n3 3 5\n5 4 1\n5 0 0\n0 1 1\n4 5 2\n2 0 0\n4 0 0\n1\n"
      };
    }
  }
}
