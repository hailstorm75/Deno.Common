using System;
using System.Linq;
using Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Data.Tests
{
  [TestClass]
  public class UtTrie
  {
    [TestMethod]
    public void TestMethod1()
    {
      var trie = new Trie();
      trie.Add("234").Add("2301").Add("501").Add("01");

      var min = new DfaMinimizer(11, 10, 0, 4);
      min.LoadTransitions(trie.Transitions())
         .SetFinalState(trie.FinateStates.Select(x => (int)x))
         .PartitionTransions()
         .SplitBlocksAndCoords();

      var result = min.ToString();
      Assert.AreEqual("6 8 4 1\n4 2 3\n3 3 5\n5 4 1\n5 0 0\n0 1 1\n4 5 2\n2 0 0\n4 0 0\n1\n", result);
    }

    [TestMethod]
    public void TestMethod2()
    {
      var trie = new Trie();
      trie.Add("234").Add("2301").Add("501").Add("01");

      var result = DfaMinimizer.Minimize(trie);
    }
  }
}
