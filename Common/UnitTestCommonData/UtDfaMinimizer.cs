using System.Linq;
using UnitTestConstants;
using Common.Data.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Data.Tests
{
  [TestClass]
  public class UtDfaMinimizer
  {
    [TestMethod, TestCategory(Constants.METHOD)]
    [DynamicData(nameof(DataDfaMinimizer.GetTrieData), typeof(DataDfaMinimizer), DynamicDataSourceType.Method)]
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

    [TestMethod, TestCategory(Constants.METHOD)]
    [DynamicData(nameof(DataDfaMinimizer.GetTrieData), typeof(DataDfaMinimizer), DynamicDataSourceType.Method)]
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
