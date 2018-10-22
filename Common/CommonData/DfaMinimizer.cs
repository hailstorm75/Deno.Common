using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
  public class DfaMinimizer
  {
    #region Fields

    private int[] m_marked;
    private List<int> m_touched;
    private int m_touchedCount;
    private Partition m_blocks;
    private Partition m_cords;
    private int m_stateCount; // number of states
    private int m_transitionCount; // number of transitions
    private int m_finalStatesCount; // number of final states
    private int m_initialState; // initial state
    private int[] m_transitionFrom; // tails of transitions (i.e. to state)
    private int[] m_transitionOnInput; // labels of transitions (i.e. on what input)
    private int[] m_transitionTo; // heads of transitions (i.e. from state

    private List<Dfa<char>.Transition> m_transitions;

    private int[] m_adjacent;
    private int[] m_offset;
    private int m_reachableCount = 0;

    #endregion

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

    public DfaMinimizer(int stateCount, int transitionCount, int initialState, int finalStatesCount)
    {
      m_stateCount = stateCount;
      m_transitionCount = transitionCount;
      m_finalStatesCount = finalStatesCount;
      m_initialState = initialState;

      m_blocks = new Partition(stateCount);

      m_transitionFrom = new int[m_transitionCount];
      m_transitionOnInput = new int[m_transitionCount];
      m_transitionTo = new int[m_transitionCount];

      m_transitions = new List<Dfa<char>.Transition>(m_transitionCount);

      m_adjacent = new int[m_transitionCount];
      m_offset = new int[m_stateCount + 1];
    }

    public DfaMinimizer LoadTransitions(IEnumerable<Trie.Transition> transitions)
    {
      var i = 0;
      foreach (var transition in transitions)
      {
        m_transitionFrom[i] = (int)transition.From;
        m_transitionOnInput[i] = int.Parse(transition.OnInput.ToString());
        m_transitionTo[i] = (int)transition.To;
        ++i;
      }

      Reach(m_initialState);
      RemoveUnreachable(m_transitionFrom, m_transitionTo);

      return this;
    }

    public DfaMinimizer SetFinalState(params int[] states)
    {
      return SetFinalState(states);
    }

    public DfaMinimizer SetFinalState(IEnumerable<int> states)
    {
      foreach (var state in states)
        if (m_blocks.Location[state] < m_blocks.Past[0])
          Reach(state);

      return this;
    }

    public DfaMinimizer PartitionTransions()
    {
      m_finalStatesCount = m_reachableCount;
      RemoveUnreachable(m_transitionTo, m_transitionFrom);

      m_touched = new List<int>(m_transitionCount + 1);
      m_marked = new int[m_transitionCount + 1];
      m_marked[0] = m_finalStatesCount;

      if (m_finalStatesCount != 0)
      {
        m_touched.Add(0);
        m_touchedCount++;
        m_blocks.Split(ref m_marked, m_touched, ref m_touchedCount);
      }

      m_cords = new Partition(m_transitionCount);
      if (m_transitionCount != 0)
      {
        //Array.Sort(m_cords.Elements, Compare);
        var e = m_cords.Elements.Select(x => m_transitionOnInput[x]).ToArray();
        Array.Sort(e, m_cords.Elements);

        m_cords.SetCount = m_marked[0] = 0;
        // this code relies on the fact that cords.first[0] == 0 at this point for the first set to be correct
        var currentLabel = m_transitionOnInput[m_cords.Elements[0]];

        for (var i = 0; i < m_transitionCount; ++i)
        {
          var t = m_cords.Elements[i];

          if (m_transitionOnInput[t] != currentLabel)
          {
            currentLabel = m_transitionOnInput[t];
            m_cords.Past[m_cords.SetCount++] = i;
            m_cords.First[m_cords.SetCount] = i;
            m_marked[m_cords.SetCount] = 0;
          }

          m_cords.SetOf[t] = m_cords.SetCount;
          m_cords.Location[t] = i;
        }

        m_cords.Past[m_cords.SetCount++] = m_transitionCount;
      }

      return this;
    }

    public DfaMinimizer SplitBlocksAndCoords()
    {
      MakeAdjacent(m_transitionTo);

      int b = 1, c = 0;
      while (c < m_cords.SetCount)
      {
        for (int i = m_cords.First[c]; i < m_cords.Past[c]; ++i)
          m_blocks.Mark(m_transitionFrom[m_cords.Elements[i]], ref m_marked, ref m_touched, ref m_touchedCount);

        m_blocks.Split(ref m_marked, m_touched, ref m_touchedCount); ++c;

        while (b < m_blocks.SetCount)
        {
          for (int i = m_blocks.First[b]; i < m_blocks.Past[b]; ++i)
          {
            for (var j = m_offset[m_blocks.Elements[i]]; j < m_offset[m_blocks.Elements[i] + 1]; ++j)
              m_cords.Mark(m_adjacent[j], ref m_marked, ref m_touched, ref m_touchedCount);
          }


          m_cords.Split(ref m_marked, m_touched, ref m_touchedCount); ++b;
        }
      }

      return this;
    }

    public Dfa<char> ToDfa()
    {
      IEnumerable<Dfa<char>.Transition> Generate()
      {
        for (var t = 0; t < m_transitionCount; ++t)
          if (m_blocks.Location[m_transitionFrom[t]] == m_blocks.First[m_blocks.SetOf[m_transitionFrom[t]]])
            yield return new Dfa<char>.Transition((ulong)m_blocks.SetOf[m_transitionFrom[t]],
                                                  (ulong)m_blocks.SetOf[m_transitionTo[t]],
                                                         m_transitionOnInput[t].ToString()[0]);
      }

      return Dfa<char>.CreateFromTransitions(Generate());
    }

    public override string ToString()
    {
      var mo = 0;
      var fo = 0;
      for (var t = 0; t < m_transitionCount; ++t)
        if (m_blocks.Location[m_transitionFrom[t]] == m_blocks.First[m_blocks.SetOf[m_transitionFrom[t]]])
          ++mo;
      for (var b = 0; b < m_blocks.SetCount; ++b)
        if (m_blocks.First[b] < m_finalStatesCount)
          ++fo;

      var sb = new StringBuilder();
      sb.Append(m_blocks.SetCount)
        .Append(" ")
        .Append(mo)
        .Append(" ")
        .Append(m_blocks.SetOf[m_initialState])
        .Append(" ")
        .Append($"{fo}\n");

      for (var t = 0; t < m_transitionCount; ++t)
        if (m_blocks.Location[m_transitionFrom[t]] == m_blocks.First[m_blocks.SetOf[m_transitionFrom[t]]])
          sb.Append(m_blocks.SetOf[m_transitionFrom[t]].ToString())
            .Append(" ")
            .Append(m_transitionOnInput[t].ToString())
            .Append(" ")
            .Append(m_blocks.SetOf[m_transitionTo[t]].ToString())
            .Append('\n');

      for (var b = 0; b < m_blocks.SetCount; ++b)
        if (m_blocks.First[b] < m_finalStatesCount)
          sb.Append(b).Append('\n');

      return sb.ToString();
    }

    private int Compare(int lhs, int rhs) => m_transitionOnInput[lhs] < m_transitionOnInput[rhs] ? 1 : 0;

    private void MakeAdjacent(IReadOnlyList<int> states)
    {
      for (var state = 0; state <= m_stateCount; ++state)
        m_offset[state] = 0;

      for (var transition = 0; transition < m_transitionCount; ++transition)
        ++m_offset[states[transition]];

      for (var state = 0; state < m_stateCount; ++state)
        m_offset[state + 1] += m_offset[state];

      for (var transition = m_transitionCount; transition-- != 0;)
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
      for (var t = 0; t < m_transitionCount; ++t)
      {
        if (m_blocks.Location[tail[t]] >= m_reachableCount) continue;

        head[count] = head[t];
        m_transitionOnInput[count] = m_transitionOnInput[t];
        tail[count] = tail[t];
        ++count;
      }

      m_transitionCount = count;
      m_blocks.Past[0] = m_reachableCount;
      m_reachableCount = 0;
    }

    public static Dfa<char> Minimize(Trie trie)
    {
      var dfaMinimizer = new DfaMinimizer((int)trie.StateCount, (int)trie.TransitionCount, 0, trie.WordCount);
      return dfaMinimizer.LoadTransitions(trie.Transitions())
                         .SetFinalState(trie.FinateStates.Select(x => (int)x))
                         .PartitionTransions()
                         .SplitBlocksAndCoords()
                         .ToDfa();
    }
  }
}
