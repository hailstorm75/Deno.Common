using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common.Data.Tests.Data
{
  internal static class DataPatternFinder
  {
    public static IEnumerable<object[]> GetDataToSearchPatternFor()
    {
      yield return new object[]
      {
        new List<string>
        {
          "1a5",
          "2a6",
          "3a7"
        }.ToArray()
      };
      yield return new object[]
      {
        new List<string>
        {
          "234",
          "2301",
          "501",
          "01"
        }.ToArray()
      };
      yield return new object[]
      {
        new List<string>
        {
          "testa",
          "testb",
          "testc",
          "testf",
          "testc",
          "testn"
        }.ToArray()
      };
      yield return new object[]
      {
        new List<string>
        {
          "atest",
          "btest",
          "ctest",
          "ftest",
          "ctest",
          "ntest"
        }.ToArray()
      };
      yield return new object[]
      {
        new List<string>
        {
          @"abc(1).txt",
          @"abc(2).txt",
          @"abc(3).txt",
          @"abc(4).txt"
        }
      };
      yield return new object[]
      {
        new List<string>
        {
          "1. testA",
          "2. testB",
          "3. testC",
          "4. testD",
          "5. testE",
          "6. testF"
        }.ToArray()
      };
      yield return new object[]
      {
        new List<string>
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
        }.ToArray()
      };
    }
  }
}
