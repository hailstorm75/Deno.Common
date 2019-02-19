using NUnit.Framework;
using UnitTestConstants;

namespace Common.Data.Tests
{
	[TestFixture]
  public class UtTrie
  {
    [Test, Category(Constants.METHOD)]
    public void LongestCommonPrefix()
    {
      // Arrange
      var data = new[] { "CommonData", "CommonLinq", "Controlz" };

      // Act
      var result = Trie.FindLongestCommonPrefix(data);

      // Assert
      Assert.AreEqual("Common", result);
    }

		[Test, Category(Constants.METHOD)]
		public void ValidateAdded()
		{
      // Arrange
      var data = new[] { "CommonData", "CommonLinq", "Controlz" };
			var trie = new Trie().AddRange(data);

			// Act
			var result = trie.Contains(data[0]);

			// Assert
			Assert.IsTrue(result);
		}
  }
}