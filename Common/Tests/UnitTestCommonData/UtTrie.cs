using System.Collections.Generic;
using NUnit.Framework;

namespace Common.Data.Tests
{
  [TestFixture]
  public class UtTrie
  {
    [Test]
    public void LongestCommonPrefix()
    {
      // Arrange
      var data = new[] { "CommonData", "CommonLinq", "Controlz" };

      // Act
      var result = Trie.FindLongestCommonPrefix(data);

      // Assert
      Assert.AreEqual("Common", result);
    }
  }
}