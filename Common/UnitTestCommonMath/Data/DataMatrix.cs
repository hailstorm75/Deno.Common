using System;
using System.Collections.Generic;

namespace Common.Math.Tests.Data
{
  internal static class DataMatrix
  {
    public static readonly double[,] InvertableMatrix =
    {
      {1, 0, 0},
      {0, 1, 0},
      {0, 0, 1}
    };

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

    public static IEnumerable<object[]> GetInvertData()
    {
      return new List<object[]>
      {
        new object[]
        {
          new double[,]
          {
            { 4, 7 },
            { 2, 6 },
          },
          new double[,]
          {
            { 0.6, -0.7 },
            { -0.2, 0.4 },
          }
        },
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
        },
        new object[]
        {
          new double[,]
          {
            { 1, 1, 1, 0 },
            { 0, 3, 1, 2 },
            { 2, 3, 1, 0 },
            { 1, 0, 2, 1 },
          },
          new double[,]
          {
            { -3, -0.5, 1.5, 1 },
            { 1, 0.25, -0.25, -0.5 },
            { 3, 0.25, -1.25, -0.5 },
            { -3, 0, 1, 1 },
          }
        }
      };
    }

    internal static IEnumerable<object[]> GetNonIdentityMatrixData()
    {
      return new List<object[]>
      {
        new object[]
        {
          new double[,]
          {
            { 1, 1 }
          }
        },
        new object[]
        {
          new double[,]
          {
            { 1 },
            { 1 }
          }
        },
        new object[]
        {
          new double[,]
          {
            { 4, 2, 7 },
            { 8, 7, 0 }
          }
        },
        new object[]
        {
          new double[,]
          {
            { 4, 8 },
            { 2, 7 },
            { 7, 0 }
          }
        }
      };
    }
  }
}
