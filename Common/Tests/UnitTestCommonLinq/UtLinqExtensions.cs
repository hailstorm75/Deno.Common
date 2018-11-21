using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnitTestConstants;

namespace Common.Linq.Tests
{
  [TestFixture]
  public class UtLinqExtensions
  {
    [Test, Category(Constants.EXTENSION)]
    public void RemoveDuplicates()
    {
      // Arrange
      var data = new List<int> { 1, 4, 8, 1, 9, 7, 5, 8, 3 };

      // Act
      var result = data.DistinctBy(x => x);

      // Assert
      CollectionAssert.AllItemsAreUnique(result.ToList());
    }

    [Test, Category(Constants.EXTENSION)]
    public void ApplyForEarch()
    {
      // Arrange
      var data = new List<int> { 1, 4, 8, 1, 9, 7, 5, 8, 3 };
      var expected = new List<int> { -1, -4, -8, -1, -9, -7, -5, -8, -3 };

      // Act
      var result = data.ForEachDo(x => x * -1).ToList();

      // Assert
      CollectionAssert.AreEqual(expected, result);
    }
  }
}