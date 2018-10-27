using System.Collections.Generic;
using System.Linq;

namespace Common.Data
{
  /// <summary>
  /// Digital tree
  /// </summary>
  public class Trie : Dfa<char>
  {
    public override IEnumerable<int> FinateStates => m_finateStates.Select(x => x);

    private int m_stateCount;
    public override int StateCount => m_stateCount;

    public override int TransitionCount => m_stateCount - 1;

    public int WordCount => m_finateStates.Count;

    /// <summary>
    /// Default constructor
    /// </summary>
    public Trie()
    {
      m_stateCount = 1;
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
        m_stateCount = root.Insert(character, m_stateCount);
        root = root.Children[character];

        if (m_alphabet.Contains(character)) continue;

        m_alphabet.Add(character);
      }

      m_finateStates.Add(root.Id);

      return this;
    }

    public IEnumerable<Transition<char>> Transitions() => GenerateTransitions(m_root);

    #endregion
  }
}
