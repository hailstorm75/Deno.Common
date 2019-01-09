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
          "1a5",
          "2a6",
          "3a7"
        }
      };
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
      yield return new object[]
      {
        new []
        {
          "atest",
          "btest",
          "ctest",
          "ftest",
          "ctest",
          "ntest"
        }
      };
      yield return new object[]
      {
        new []
        {
          "test1.txt",
          "test2.txt",
          "test3.txt",
          "test4.txt"
        }
      };
      yield return new object[]
      {
        new []
        {
          "1. testA",
          "2. testB",
          "3. testC",
          "4. testD",
          "5. testE",
          "6. testF"
        }
      };
      yield return new object[]
      {
        new []
        {
          "Azero1go5",
          "Azero1go6",
          "Azero2go5",
          "Azero2go6",
          "Azero3go5",
          "Azero3go6",
          "Bzero1go5",
          "Bzero1go6",
          "Bzero2go5",
          "Bzero2go6",
          "Bzero3go5",
          "Bzero3go6",
          "Czero1go5",
          "Czero1go6",
          "Czero2go5",
          "Czero2go6",
          "Czero3go5",
          "Czero3go6",
        }
      };
    }
  }
}
