using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Data.Tests.Data;
using NUnit.Framework;
using UnitTestConstants;

namespace Common.Data.Tests
{
  [TestFixture]
  public class UtPatternFinder
  {
    [Test, TestCaseSource(typeof(DataPatternFinder), nameof(DataPatternFinder.GetDataToSearchPatternFor))]
    [Category(Constants.METHOD)]
    public void FindPattern(string[] data)
    {
      // Arrange
      var finder = new PatternGenerator(data);

      // Act
      var result = finder.FindPattern();

      // Assert
      var count = data.Count(x => Regex.IsMatch(x, result));
      Assert.AreEqual(data.Length, count, result);
    }
  }
}