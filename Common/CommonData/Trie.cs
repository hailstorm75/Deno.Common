﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Common.Data
{
  /// <inheritdoc />
  /// <summary>
  /// Digital tree
  /// </summary>
  [Serializable]
  public class Trie : BaseDfa<char>
  {
    #region Fields

    private int m_stateCount;

    #endregion

    #region Properties

    /// <inheritdoc />
    public override IEnumerable<int> FiniteStates => m_finateStates.Select(x => x);

    /// <inheritdoc />
    public override int StateCount => m_stateCount;

    /// <inheritdoc />
    public override int TransitionCount => m_stateCount - 1;

    /// <summary>
    /// Total number of words in <see cref="Trie"/>
    /// </summary>
    public int WordCount => m_finateStates.Count;

    #endregion

    /// <summary>
    /// Default constructor
    /// </summary>
    public Trie()
    {
      m_stateCount = 1;
    }

    #region Methods

    public string FindCommonPrefix()
    {
      string GetNext(State state)
      {
        if (state.Neighbours.Count != 1)
          return string.Empty;

        return state.Neighbours.First().Key + GetNext(state.Neighbours.First().Value);
      }

      return GetNext(m_root);
    }

    public static string FindCommonPrefix(IReadOnlyList<string> strings)
    {
      var minLength = strings.Min(x => x.Length);
      var prefix = new StringBuilder();

      for (var i = 0; i < minLength; i++)
      {
        var current = strings[0][i];

        for (var j = 1; j < strings.Count; j++)
          if (strings[j][i] != current)
            return prefix.ToString();

        prefix.Append(current);
      }

      return prefix.ToString();
    }

    /// <summary>
    /// Determines whether the <see cref="Trie"/> contains a specific <paramref name="word"/>
    /// </summary>
    /// <param name="word">Word to search for</param>
    /// <returns>True if contains</returns>
    public bool Constains(string word)
    {
      var root = m_root;

      foreach (var character in word)
      {
        if (!root.Neighbours.TryGetValue(character, out var node))
          return false;

        root = node;
      }

      return m_finateStates.Contains(root.Id);
    }

    /// <summary>
    /// Adds a range of <paramref name="words"/> to the <see cref="Trie"/>
    /// </summary>
    /// <param name="words">Words to add</param>
    /// <returns>This instance</returns>
    public Trie AddRange(IEnumerable<string> words)
    {
      foreach (var word in words)
        Add(word);

      return this;
    }

    /// <summary>
    /// Adds a <paramref name="word"/> to the <see cref="Trie"/>
    /// </summary>
    /// <param name="word">Word to add</param>
    /// <returns>This instance</returns>
    public Trie Add(string word)
    {
      var root = m_root;

      foreach (var character in word)
      {
        m_stateCount = root.Connect(character, m_stateCount);
        root = root.Neighbours[character];

        if (m_alphabet.Contains(character)) continue;

        m_alphabet.Add(character);
      }

      m_finateStates.Add(root.Id);

      return this;
    }

    /// <summary>
    /// Retrieves collection of transitions between all nodes
    /// </summary>
    /// <returns>Transitions between nodes</returns>
    public IEnumerable<Transition<char>> GetTransitions() => GetTransitions(m_root);

    #endregion
  }
}
