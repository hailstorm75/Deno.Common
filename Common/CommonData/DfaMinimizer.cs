﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
  /// <summary>
  /// Minimizes <see cref="BaseDfa{T}"/> instances
  /// </summary>
  /// <typeparam name="T">Type of input symbols.<para/>Must match the type of <see cref="BaseDfa{T}"/></typeparam>
  public class DfaMinimizer<T> where T
    : IComparable, IComparable<T>, IEquatable<T>
  {
    #region Nested types

    private class Transitions
    {
      public struct Pair<TKey>
      {
        public Func<int, TKey> Get { get; }
        public Action<int, TKey> Set { get; }

        public Pair(Func<int, TKey> get, Action<int, TKey> set)
        {
          Get = get;
          Set = set;
        }
      }

      #region Fields

      private readonly List<Transition<T>> m_transitions;

      #endregion

      #region Properties

      public int Count { get; set; }
      public Pair<int> From { get; }
      public Pair<int> To { get; }
      public Pair<T> OnInput { get; }

      #endregion

      #region Constructors

      public Transitions(int count, IEnumerable<Transition<T>> transitions)
      {
        Count = count;
        m_transitions = transitions.ToList();

        From = new Pair<int>(GetFrom, SetFrom);
        To = new Pair<int>(GetTo, SetTo);
        OnInput = new Pair<T>(GetOnInput, SetOnInput);
      }

      #endregion

      #region Methods

      // TODO Optimize
      private int GetFrom(int i) => m_transitions[i].From;

      // TODO Optimize
      private void SetFrom(int i, int val) => m_transitions[i].From = val;

      // TODO Optimize
      private int GetTo(int i) => m_transitions[i].To;

      // TODO Optimize
      private void SetTo(int i, int val) => m_transitions[i].To = val;

      // TODO Optimize
      private T GetOnInput(int i) => m_transitions[i].OnInput;

      // TODO Optimize
      private void SetOnInput(int i, T val) => m_transitions[i].OnInput = val;

      #endregion
    }

    private struct Partition
    {
      #region Properties

      public int[] Elements { get; }
      public int[] Location { get; }
      public int[] SetOf { get; }
      public int[] First { get; }
      public int[] Past { get; }
      public int SetCount { get; set; }

      #endregion

      #region Constructors

      public Partition(int elementCount)
      {
        SetCount = elementCount == 0 ? 0 : 1;
        Elements = new int[elementCount];
        Location = new int[elementCount];
        SetOf = new int[elementCount];
        First = new int[elementCount];
        Past = new int[elementCount];

        for (var i = 0; i < elementCount; ++i)
        {
          Elements[i] = Location[i] = i;
          SetOf[i] = 0;
        }

        if (SetCount == 0) return;
        First[0] = 0;
        Past[0] = elementCount;
      }

      #endregion

      #region Methods

      public void Mark(int element, ref int[] marked, ref List<int> touched, ref int touchedCount)
      {
        var set = SetOf[element];
        var i = Location[element];
        var j = First[set] + marked[set];

        Elements[i] = Elements[j];
        Location[Elements[i]] = i;
        Elements[j] = element;
        Location[element] = j;

        if (marked[set]++ != 0) return;

        if (touched.Count > touchedCount)
          touched[touchedCount] = set;
        else
          touched.Add(set);

        touchedCount++;
      }

      public void Split(ref int[] marked, IReadOnlyList<int> touched, ref int touchedCount)
      {
        while (touchedCount != 0)
        {
          var set = touched[--touchedCount];
          var firstUnmarked = First[set] + marked[set];
          if (firstUnmarked == Past[set])
          {
            marked[set] = 0;
            continue;
          }

          // Make the smaller half a new set
          // If same size, then make a new set out of unmarked
          if (marked[set] <= Past[set] - firstUnmarked)
          {
            First[SetCount] = First[set];
            Past[SetCount] = First[set] = firstUnmarked;
          }
          else
          {
            Past[SetCount] = Past[set];
            First[SetCount] = Past[set] = firstUnmarked;
          }

          for (var i = First[SetCount]; i < Past[SetCount]; ++i)
            SetOf[Elements[i]] = SetCount;

          marked[SetCount++] = 0;
          marked[set] = 0;
        }
      }

      #endregion
    }

    #endregion

    #region Fields

    private int m_touchedCount;
    private int[] m_marked;
    private List<int> m_touched;
    private Partition m_blocks;
    private Partition m_cords;
    private readonly int m_initialState;
    private readonly int m_stateCount;
    private int m_finalStatesCount;

    private Transitions m_transitions;

    private int m_reachableCount;
    private readonly int[] m_adjacent;
    private readonly int[] m_offset;

    #endregion

    // TODO Make private
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="stateCount"></param>
    /// <param name="transitionCount"></param>
    /// <param name="initialState"></param>
    /// <param name="finalStatesCount"></param>
    public DfaMinimizer(int stateCount, int transitionCount, int initialState, int finalStatesCount)
    {
      m_stateCount = stateCount;
      m_finalStatesCount = finalStatesCount;
      m_initialState = initialState;

      m_blocks = new Partition(stateCount);

      m_adjacent = new int[transitionCount];
      m_offset = new int[m_stateCount + 1];
    }

    #region Methods

    // TODO Make private
    public DfaMinimizer<T> LoadTransitions(List<Transition<T>> transitions)
    {
      m_transitions = new Transitions(transitions.Count, transitions);

      Reach(m_initialState);
      RemoveUnreachable(m_transitions.From, m_transitions.To);

      return this;
    }

    // TODO Make private
    public DfaMinimizer<T> SetFinalState(params int[] states) => SetFinalState(states.ToList());

    // TODO Make private
    public DfaMinimizer<T> SetFinalState(IEnumerable<int> states)
    {
      foreach (var state in states)
        if (m_blocks.Location[state] < m_blocks.Past[0])
          Reach(state);

      return this;
    }

    private DfaMinimizer<T> PartitionTransions()
    {
      m_finalStatesCount = m_reachableCount;

      RemoveUnreachable(m_transitions.To, m_transitions.From);

      m_touched = new List<int>(m_transitions.Count + 1);
      m_marked = new int[m_transitions.Count + 1];
      m_marked[0] = m_finalStatesCount;

      if (m_finalStatesCount != 0)
      {
        m_touched.Add(0);
        m_touchedCount++;
        m_blocks.Split(ref m_marked, m_touched, ref m_touchedCount);
      }

      m_cords = new Partition(m_transitions.Count);

      if (m_transitions.Count == 0) return this;

      var e = m_cords.Elements.Select(x => m_transitions.OnInput.Get(x)).ToArray();
      Array.Sort(e, m_cords.Elements);

      m_cords.SetCount = m_marked[0] = 0;

      var currentLabel = m_transitions.OnInput.Get(m_cords.Elements[0]);

      for (var i = 0; i < m_transitions.Count; ++i)
      {
        var t = m_cords.Elements[i];

        if (!m_transitions.OnInput.Get(t).Equals(currentLabel))
        {
          currentLabel = m_transitions.OnInput.Get(t);
          m_cords.Past[m_cords.SetCount++] = i;
          m_cords.First[m_cords.SetCount] = i;
          m_marked[m_cords.SetCount] = 0;
        }

        m_cords.SetOf[t] = m_cords.SetCount;
        m_cords.Location[t] = i;
      }

      m_cords.Past[m_cords.SetCount++] = m_transitions.Count;

      return this;
    }

    private DfaMinimizer<T> SplitBlocksAndCoords()
    {
      MakeAdjacent(m_transitions.To);

      int b = 1, c = 0;
      while (c < m_cords.SetCount)
      {
        for (var i = m_cords.First[c]; i < m_cords.Past[c]; ++i)
          m_blocks.Mark(m_transitions.From.Get(m_cords.Elements[i]), ref m_marked, ref m_touched, ref m_touchedCount);

        m_blocks.Split(ref m_marked, m_touched, ref m_touchedCount); ++c;

        while (b < m_blocks.SetCount)
        {
          for (var i = m_blocks.First[b]; i < m_blocks.Past[b]; ++i)
            for (var j = m_offset[m_blocks.Elements[i]]; j < m_offset[m_blocks.Elements[i] + 1]; ++j)
              m_cords.Mark(m_adjacent[j], ref m_marked, ref m_touched, ref m_touchedCount);

          m_cords.Split(ref m_marked, m_touched, ref m_touchedCount); ++b;
        }
      }

      return this;
    }

    // TODO Make private
    public DfaMinimizer<T> Process() => PartitionTransions().SplitBlocksAndCoords();

    /// <summary>
    /// Retrieve minimized transitions
    /// </summary>
    /// <returns>Transitions</returns>
    public IEnumerable<Transition<T>> GetTransitions()
    {
      for (var i = 0; i < m_transitions.Count; ++i)
        if (m_blocks.Location[m_transitions.From.Get(i)] == m_blocks.First[m_blocks.SetOf[m_transitions.From.Get(i)]])
          yield return new Transition<T>(m_blocks.SetOf[m_transitions.From.Get(i)],
                                         m_blocks.SetOf[m_transitions.To.Get(i)],
                                         m_transitions.OnInput.Get(i));
    }

    private IEnumerable<int> GetAcceptingStates()
    {
      for (var b = 0; b < m_blocks.SetCount; ++b)
        if (m_blocks.First[b] < m_finalStatesCount)
          yield return b;
    }

    /// <summary>
    /// Retrieves information about the minimized automata
    /// </summary>
    /// <remarks>
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///       State count
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Transition count
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Initial state
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Final state count
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <returns>Information</returns>
    public Tuple<int, int, int, int> GetAutomataInfo()
    {
      var transitionCount = 0;
      var finalStateCount = 0;

      for (var t = 0; t < m_transitions.Count; ++t)
        if (m_blocks.Location[m_transitions.From.Get(t)] == m_blocks.First[m_blocks.SetOf[m_transitions.From.Get(t)]])
          ++transitionCount;

      for (var b = 0; b < m_blocks.SetCount; ++b)
        if (m_blocks.First[b] < m_finalStatesCount)
          ++finalStateCount;

      return new Tuple<int, int, int, int>(m_blocks.SetCount,
                                           transitionCount,
                                           m_blocks.SetOf[m_initialState],
                                           finalStateCount);
    }

    /// <summary>
    /// Generates string representation of DFA information and its transitions
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      var sb = new StringBuilder();

      var header = GetAutomataInfo();
      sb.Append($"{header.Item1} {header.Item2} {header.Item3} {header.Item4}\n");
      foreach (var transition in GetTransitions())
        sb.Append($"{transition.From} {transition.OnInput} {transition.To}\n");
      foreach (var finalState in GetAcceptingStates())
        sb.Append(finalState).Append('\n');

      return sb.ToString();
    }

    private void MakeAdjacent(Transitions.Pair<int> states)
    {
      for (var state = 0; state <= m_stateCount; ++state)
        m_offset[state] = 0;

      for (var transition = 0; transition < m_transitions.Count; ++transition)
        ++m_offset[states.Get(transition)];

      for (var state = 0; state < m_stateCount; ++state)
        m_offset[state + 1] += m_offset[state];

      for (var transition = m_transitions.Count; transition-- != 0;)
        m_adjacent[--m_offset[states.Get(transition)]] = transition;
    }

    private void Reach(int state)
    {
      var i = m_blocks.Location[state];
      if (i < m_reachableCount) return;

      m_blocks.Elements[i] = m_blocks.Elements[m_reachableCount];
      m_blocks.Location[m_blocks.Elements[i]] = i;
      m_blocks.Elements[m_reachableCount] = state;
      m_blocks.Location[state] = m_reachableCount++;
    }

    private void RemoveUnreachable(Transitions.Pair<int> tail, Transitions.Pair<int> head)
    {
      MakeAdjacent(tail);
      // walk the DFA graph marking reachable states
      for (var i = 0; i < m_reachableCount; ++i)
        for (var j = m_offset[m_blocks.Elements[i]]; j < m_offset[m_blocks.Elements[i] + 1]; ++j)
          Reach(head.Get(m_adjacent[j]));

      // remove unreachable states and transitions
      var count = 0;
      for (var t = 0; t < m_transitions.Count; ++t)
      {
        if (m_blocks.Location[tail.Get(t)] >= m_reachableCount) continue;

        head.Set(count, head.Get(t));
        m_transitions.OnInput.Set(count, m_transitions.OnInput.Get(t));
        tail.Set(count, tail.Get(t));

        ++count;
      }

      m_transitions.Count = count;
      m_blocks.Past[0] = m_reachableCount;
      m_reachableCount = 0;
    }

    /// <summary>
    /// Minimizes given <paramref name="trie"/>
    /// </summary>
    /// <param name="trie">Trie to minimize</param>
    /// <returns></returns>
    public static DfaMinimizer<char> Minimize(Trie trie)
    {
      var dfaMinimizer = new DfaMinimizer<char>(trie.StateCount, trie.TransitionCount, 0, trie.WordCount);
      return dfaMinimizer.LoadTransitions(trie.GetTransitions().ToList())
        .SetFinalState(trie.FiniteStates)
        .Process();
    }

    #endregion
  }
}
