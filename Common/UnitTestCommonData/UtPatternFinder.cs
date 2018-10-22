using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Data.Tests
{
  [TestClass]
  public class UtPatternFinder
  {
    [TestMethod, TestCategory("Method")]
    public void FindPattern()
    {
      var data = new List<string> { "testa", "0testb", "7testc", "5testx" };
      var finder = new PatternFinder(data);
      var result = finder.FindPattern().Result();
    }
  }
}
