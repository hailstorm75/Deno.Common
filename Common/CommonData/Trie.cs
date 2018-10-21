using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Data
{
  /// <summary>
  /// Digital tree
  /// </summary>
  public class Trie : Dfa<char>
  {
    private readonly State m_root;

    public override IEnumerable<ulong> FinateStates => m_finateStates.Select(x => x);

    private ulong m_stateCount;
    public override ulong StateCount => m_stateCount;

    private ulong m_transitionCount;
    public override ulong TransitionCount => m_transitionCount;

    public int WordCount => m_finateStates.Count;

    /// <summary>
    /// Default constructor
    /// </summary>
    public Trie()
    {
      m_stateCount = 0;
      m_root = new State(m_stateCount++);
    }

    #region Methods

    public bool Search(string word)
    {
      var root = m_root;

      foreach (var character in word)
      {
        if (!root.Children.TryGetValue(character, out var node))
          return false;

        root = node;
      }

      return m_finateStates.Contains(root.Id);
    }

    public Trie AddRange(IEnumerable<string> words)
    {
      foreach (var word in words)
        Add(word);

      return this;
    }

    public Trie Add(string word)
    {
      var root = m_root;

      foreach (var character in word)
      {
        root.Insert(character, ref m_stateCount);
        root = root.Children[character];

        if (m_alphabet.Contains(character)) continue;

        m_transitionCount++;
        m_alphabet.Add(character);
      }

      m_finateStates.Add(root.Id);

      return this;
    }

    public IEnumerable<Transition> Transitions() => GenerateTransitions(m_root);

    #endregion
  }
}
