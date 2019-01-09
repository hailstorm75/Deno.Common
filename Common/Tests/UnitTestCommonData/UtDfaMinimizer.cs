using System.Linq;
using Common.Data.Tests.Data;
using NUnit.Framework;
using UnitTestConstants;

namespace Common.Data.Tests
{
  [TestFixture]
  public class UtDfaMinimizer
  {
    [Test, TestCaseSource(typeof(DataDfaMinimizer), nameof(DataDfaMinimizer.GetTrieData))]
    [Category(Constants.METHOD)]
    public void TestMinimize(string[] strings, string expected)
    {
      var trie = new Trie();
      trie.AddRange(strings);

      var min = DfaMinimizer<char>.Minimize(trie);

      var result = min.ToString();
      Assert.AreEqual(expected, result);
    }
  }
}