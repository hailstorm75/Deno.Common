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
    public void InstanceMinimize(string[] strings, string expected)
    {
      var trie = new Trie();
      trie.AddRange(strings);

      var min = new DfaMinimizer<char>(trie.StateCount, trie.TransitionCount, 0, trie.WordCount);
      min.LoadTransitions(trie.Transitions().ToList())
        .SetFinalState(trie.FinateStates)
        .Process();

      var result = min.ToString();
      Assert.AreEqual(expected, result);
    }

    [Test, TestCaseSource(typeof(DataDfaMinimizer), nameof(DataDfaMinimizer.GetTrieData))]
    [Category(Constants.METHOD)]
    public void StaticMinimize(string[] strings, string expected)
    {
      var trie = new Trie();
      trie.AddRange(strings);

      var min = DfaMinimizer<char>.Minimize(trie);

      var result = min.ToString();
      Assert.AreEqual(expected, result);
    }
  }
}