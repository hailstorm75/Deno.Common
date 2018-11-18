using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestConstants;

namespace Common.Data.Tests
{
  [TestClass]
  public class UtPatternFinder
  {
    [TestMethod, TestCategory(Constants.METHOD)]
    public void FindPattern()
    {
      // Arrange
      var data = new List<string> { "234", "2301", "501", "01" };
      var finder = new PatternFinder(data);

      // Act
      var result = finder.FindPattern();

      // Assert
      var count = data.Count(x => Regex.IsMatch(x, result));
      Assert.AreEqual(data.Count, count);
    }
  }
}
