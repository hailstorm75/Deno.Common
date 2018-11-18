﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Math.Tests
{
  [TestClass]
  public class UtLongestLong
  {
    #region Initialization tests

    [TestMethod, TestCategory("Constructor")]
    public void Initialize()
    {
      try
      {
        var unused = new LongestLong();
      }
      catch (Exception e)
      {
        Assert.Fail(e.ToString());
      }
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeInvalidList()
    {
      var unused = new LongestLong(new List<long> { -1, 5, -8, 0, 7, -5 });
    }

    [TestMethod, TestCategory("Constructor")]
    [ExpectedException(typeof(ArgumentException))]
    public void InitializeInvalidVariadic()
    {
      var unused = new LongestLong(-1, 5, -8, 0, 7, -5);
    }

    #endregion

    #region Operator tests

    //[TestMethod, TestCategory("Operator")]
    //public void AdditionIntClass()
    //{
      //var longestLong = new LongestLong();
      //longestLong = longestLong + long.MaxValue;
      //longestLong = longestLong + long.MaxValue;

      //Assert.IsTrue(longestLong.Values.SequenceEqual(new LongestLong(2, 2).Values));
    //}

    // TODO

    #endregion

    #region Comparison tests

    // TODO

    #endregion

    #region To String tests

    [TestMethod, TestCategory("Method")]
    [DataRow("10223372036854775807", new[] { 1, long.MaxValue })]
    [DataRow("-10223372036854775807", new[] { -1, long.MaxValue })]
    [DataRow("9223372036854775816223372036854775807", new[] { long.MaxValue, long.MaxValue })]
    public void ToStringTest(string expected, long[] values)
    {
      var longestLong = new LongestLong(values);
      var str = longestLong.ToString();
      Assert.AreEqual(expected, str);
    }

    #endregion
  }
}
