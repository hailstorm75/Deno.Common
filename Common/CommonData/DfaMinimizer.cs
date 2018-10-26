using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
  public class DfaMinimizer<T> where T : IComparable, IComparable<T>, IEquatable<T>
  {
    private struct Transitions
    {
      public int[] From { get; set; }
      public int[] To { get; set; }
      public T[] OnInput { get; set; }

      public int Count { get; set; }

      public Transitions(int count)
      {
        Count = count;
        From = new int[Count];
        To = new int[Count];
        OnInput = new T[Count];
      }

      public void LoadTransitions(IEnumerable<Transition<T>> transitions)
      {
        var i = 0;
        foreach (var transition in transitions)
        {
          From[i] = (int)transition.From;
          To[i] = (int)transition.To;
          OnInput[i] = transition.OnInput;
          i++;
        }
      }
    }

    private struct Partition
    {
      public int[] Elements { get; set; }
      public int[] Location { get; }
      public int[] SetOf { get; set; }
      public int[] First { get; }
      public int[] Past { get; }
      public int SetCount { get; set; }

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
    }

    #region Fields

    private int[] m_marked;
    private List<int> m_touched;
    private int m_touchedCount;
    private Partition m_blocks;
    private Partition m_cords;
    private int m_stateCount; // number of states
    private int m_finalStatesCount; // number of final states
    private int m_initialState; // initial state

    private Transitions m_transitions;

    private int[] m_adjacent;
    private int[] m_offset;
    private int m_reachableCount = 0;

    #endregion

    public DfaMinimizer(int stateCount, int transitionCount, int initialState, int finalStatesCount)
    {
      m_stateCount = stateCount;
      m_finalStatesCount = finalStatesCount;
      m_initialState = initialState;

      m_blocks = new Partition(stateCount);
      m_transitions = new Transitions(transitionCount);

      m_adjacent = new int[transitionCount];
      m_offset = new int[m_stateCount + 1];
    }

    public DfaMinimizer<T> LoadTransitions(IEnumerable<Transition<T>> transitions)
    {
      m_transitions.LoadTransitions(transitions);

      Reach(m_initialState);
      RemoveUnreachable(m_transitions.From, m_transitions.To);

      return this;
    }

    public DfaMinimizer<T> SetFinalState(params int[] states) => SetFinalState(states.ToList());

    public DfaMinimizer<T> SetFinalState(IEnumerable<int> states)
    {
      foreach (var state in states)
        if (m_blocks.Location[state] < m_blocks.Past[0])
          Reach(state);

      return this;
    }

    public DfaMinimizer<T> PartitionTransions()
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

      //Array.Sort(m_cords.Elements, Compare);
      var e = m_cords.Elements.Select(x => m_transitions.OnInput[x]).ToArray();
      Array.Sort(e, m_cords.Elements);

      m_cords.SetCount = m_marked[0] = 0;
      // this code relies on the fact that cords.first[0] == 0 at this point for the first set to be correct
      var currentLabel = m_transitions.OnInput[m_cords.Elements[0]];

      for (var i = 0; i < m_transitions.Count; ++i)
      {
        var t = m_cords.Elements[i];

        if (!m_transitions.OnInput[t].Equals(currentLabel))
        {
          currentLabel = m_transitions.OnInput[t];
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

    public DfaMinimizer<T> SplitBlocksAndCoords()
    {
      MakeAdjacent(m_transitions.To);

      int b = 1, c = 0;
      while (c < m_cords.SetCount)
      {
        for (var i = m_cords.First[c]; i < m_cords.Past[c]; ++i)
          m_blocks.Mark(m_transitions.From[m_cords.Elements[i]], ref m_marked, ref m_touched, ref m_touchedCount);

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

    public Dfa<T> ToDfa()
    {
      return Dfa<T>.CreateFromTransitions(GetTransitions());
    }

    public IEnumerable<Transition<T>> GetTransitions()
    {
      for (var i = 0; i < m_transitions.Count; ++i)
        if (m_blocks.Location[m_transitions.From[i]] == m_blocks.First[m_blocks.SetOf[m_transitions.From[i]]])
          yield return new Transition<T>(m_blocks.SetOf[m_transitions.From[i]],
                                         m_blocks.SetOf[m_transitions.To[i]],
                                         m_transitions.OnInput[i]);
    }

    public IEnumerable<int> GetFinalStates()
    {
      for (var b = 0; b < m_blocks.SetCount; ++b)
        if (m_blocks.First[b] < m_finalStatesCount)
          yield return b;
    }

    public Tuple<int, int, int, int> GetAutomataInfo()
    {
      var transitionCount = 0;
      var finalStateCount = 0;

      for (var t = 0; t < m_transitions.Count; ++t)
        if (m_blocks.Location[m_transitions.From[t]] == m_blocks.First[m_blocks.SetOf[m_transitions.From[t]]])
          ++transitionCount;

      for (var b = 0; b < m_blocks.SetCount; ++b)
        if (m_blocks.First[b] < m_finalStatesCount)
          ++finalStateCount;

      return new Tuple<int, int, int, int>(m_blocks.SetCount,
                                           transitionCount,
                                           m_blocks.SetOf[m_initialState],
                                           finalStateCount);
    }

    public override string ToString()
    {
      var sb = new StringBuilder();

      var header = GetAutomataInfo();
      sb.Append($"{header.Item1} {header.Item2} {header.Item3} {header.Item4}\n");
      foreach (var transition in GetTransitions())
        sb.Append($"{transition.From} {transition.OnInput} {transition.To}\n");
      foreach (var finalState in GetFinalStates())
        sb.Append(finalState).Append('\n');

      return sb.ToString();
    }

    private void MakeAdjacent(IReadOnlyList<int> states)
    {
      for (var state = 0; state <= m_stateCount; ++state)
        m_offset[state] = 0;

      for (var transition = 0; transition < m_transitions.Count; ++transition)
        ++m_offset[states[transition]];

      for (var state = 0; state < m_stateCount; ++state)
        m_offset[state + 1] += m_offset[state];

      for (var transition = m_transitions.Count; transition-- != 0;)
        m_adjacent[--m_offset[states[transition]]] = transition;
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

    private void RemoveUnreachable(int[] tail, int[] head)
    {
      MakeAdjacent(tail);
      // walk the DFA graph marking reachable states
      for (var i = 0; i < m_reachableCount; ++i)
        for (var j = m_offset[m_blocks.Elements[i]]; j < m_offset[m_blocks.Elements[i] + 1]; ++j)
          Reach(head[m_adjacent[j]]);

      // remove unreachable states and transitions
      var count = 0;
      for (var t = 0; t < m_transitions.Count; ++t)
      {
        if (m_blocks.Location[tail[t]] >= m_reachableCount) continue;

        head[count] = head[t];
        m_transitions.OnInput[count] = m_transitions.OnInput[t];
        tail[count] = tail[t];
        ++count;
      }

      m_transitions.Count = count;
      m_blocks.Past[0] = m_reachableCount;
      m_reachableCount = 0;
    }

    public static Dfa<char> Minimize(Trie trie)
    {
      var dfaMinimizer = new DfaMinimizer<char>(trie.StateCount, trie.TransitionCount, 0, trie.WordCount);
      return dfaMinimizer.LoadTransitions(trie.Transitions())
                         .SetFinalState(trie.FinateStates)
                         .PartitionTransions()
                         .SplitBlocksAndCoords()
                         .ToDfa();
    }
  }
}
