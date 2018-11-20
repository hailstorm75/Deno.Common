using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestConstants;

namespace Common.Linq.Tests
{
  [TestClass]
  public class UtLinqExtensions
  {
    [TestMethod, TestCategory(Constants.METHOD)]
    public void LinqDistinctBySimple()
    {
      // Arrange
      var data = new List<int> {1, 5, 7, 7, 8, 1, 6, 9, 10, 9};

      // Act
      var result = data.DistinctBy(x => x);

      // Assert
      CollectionAssert.AllItemsAreUnique(result.ToList());
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    public void LinqForEachDo()
    {
      // Arrange
      var data = new List<string>() { "abc", "dfg", "hjk" };
      var expected = new List<string>() {"cba", "gfd", "kjh"};

      // Act
      var result = data.ForEachDo(x => string.Concat(x.Reverse())).ToList();

      // Assert
      CollectionAssert.AreEqual(expected, result);
    }
  }
}
