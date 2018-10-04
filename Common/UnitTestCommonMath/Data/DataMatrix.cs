using System.Collections.Generic;

namespace Common.Math.Tests.Data
{
  internal static class DataMatrix
  {
    public static double[,] InvertableMatrix =
    {
      {1, 0, 0},
      {0, 1, 0},
      {0, 0, 1}
    };

    public static IEnumerable<object[]> GetInvertData()
    {
      return new List<object[]>
      {
        //new object[]
        //{
        //  new double[,]
        //  {
        //    { 3, 3.5 },
        //    { 3.2, 3.6 },
        //  },
        //  new double[,]
        //  {
        //    { -9, 8.75 },
        //    { 8, -7.5 },
        //  }
        //},
        new object[]
        {
          new double[,]
          {
            { 1, 2, 3 },
            { 0, 1, 4 },
            { 5, 6, 0 },
          },
          new double[,]
          {
            { -24, 18, 5 },
            { 20, -15, -4 },
            { -5, 4, 1 }
          }
        }
      };
    }

    public static IEnumerable<object[]> GetCtorExceptionData()
    {
      return new List<object[]>
      {
        new object[] {-5, 5},
        new object[] {5, -5},
        new object[] {-5, 5},
        new object[] {0, 0},
        new object[] {0, 5},
        new object[] {5, 0},
      };
    }

    public static IEnumerable<object[]> GetCtorInvertableData()
    {
      return new List<object[]>
      {
        new object[]{4, 4},
        new object[]{10, 10},
        new object[]{100, 100},
        new object[]{1000, 1000},
      };
    }
  }
}
