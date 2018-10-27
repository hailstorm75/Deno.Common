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
      //var data = new List<string> { "testa", "0testb", "7testc", "5testx" };
      var data = new List<string> { "234", "2301", "501", "01" };
      var finder = new PatternFinder(data);
      var result = finder.FindPattern();
    }
  }
}
