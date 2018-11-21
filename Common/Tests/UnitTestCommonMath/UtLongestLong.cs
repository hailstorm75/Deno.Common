using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnitTestConstants;

namespace Common.Math.Tests
{
  [TestFixture]
  public class UtLongestLong
  {
    #region Initialization tests

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void Initialize()
    {
      var unused = new LongestLong();
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeInvalidList()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var unused = new LongestLong(new List<long> { -1, 5, -8, 0, 7, -5 });
      });
    }

    [Test]
    [Category(Constants.CONSTRUCTOR)]
    public void InitializeInvalidVariadic()
    {
      Assert.Throws<ArgumentException>(() =>
      {
        var unused = new LongestLong(-1, 5, -8, 0, 7, -5);
      });
    }

    #endregion

    #region To String tests

    [Test]
    [Category(Constants.METHOD)]
    [TestCase("10223372036854775807", new[] { 1, long.MaxValue })]
    [TestCase("-10223372036854775807", new[] { -1, long.MaxValue })]
    [TestCase("9223372036854775816223372036854775807", new[] { long.MaxValue, long.MaxValue })]
    public void ToStringTest(string expected, long[] values)
    {
      var longestLong = new LongestLong(values);
      var str = longestLong.ToString();
      Assert.AreEqual(expected, str);
    }

    #endregion
  }
}