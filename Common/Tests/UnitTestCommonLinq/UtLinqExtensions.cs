using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
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

    [Test, Category(Constants.EXTENSION)]
    [TestCase(23, 8)]
    [TestCase(30, 7)]
    [TestCase(15, 3)]
    public void ChunkByN(int count, int chunkSize)
    {
      // Arrange
      var data = new List<int>(Enumerable.Range(0, count));

      // Act
      var result = data.ChunkBy(chunkSize).ToList();

      // Assert
      for (var i = 0; i < result.Count - 1; i++)
        if (result[i].Count() != chunkSize)
          Assert.Fail();

      if (result.Last().Count() > chunkSize)
        Assert.Fail();
    }
  }
}