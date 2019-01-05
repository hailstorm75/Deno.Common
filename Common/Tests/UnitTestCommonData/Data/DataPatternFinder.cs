using System.Collections.Generic;

namespace Common.Data.Tests.Data
{
  internal static class DataPatternFinder
  {
    public static IEnumerable<object[]> GetDataToSearchPatternFor()
    {
      yield return new object[]
      {
        new []
        {
          "234",
          "2301",
          "501",
          "01"
        }
      };
      yield return new object[]
      {
        new []
        {
          "testa",
          "testb",
          "testc",
          "testf",
          "testc",
          "testn"
        }
      };
      // TODO
      //yield return new object[]
      //{
      //  new []
      //  {
      //    "atest",
      //    "btest",
      //    "ctest",
      //    "ftest",
      //    "ctest",
      //    "ntest"
      //  }
      //};
      //yield return new object[]
      //{
      //  new []
      //  {
      //    "1. testA",
      //    "2. testB",
      //    "3. testC",
      //    "4. testD",
      //    "5. testE",
      //    "6. testF"
      //  }
      //};
    }
  }
}
