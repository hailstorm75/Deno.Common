using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Common.Data
{
  [Serializable]
  public class Dfa<T>
  {
    [Serializable]
    public sealed class State
    {
      #region Properties

      public readonly int Id;
      public Dictionary<T, State> Children { get; set; }

      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="id"></param>
      public State(int id)
      {
        Id = id;
        Children = new Dictionary<T, State>();
      }

      public int Insert(T c, int id)
      {
        if (Children.ContainsKey(c)) return id;

        Children.Add(c, new State(id));
        return id + 1;
      }
    }

    #region Properties

    public virtual IEnumerable<int> FinateStates { get; }
    public virtual int StateCount { get; }
    public virtual int TransitionCount { get; }

    #endregion

    #region Fields

    protected State m_root;
    protected HashSet<int> m_finateStates;
    protected HashSet<T> m_alphabet;

    #endregion

    public Dfa(int rootId = 0)
    {
      m_root = new State(rootId);
      m_alphabet = new HashSet<T>();
      m_finateStates = new HashSet<int>();
    }

    public static Dfa<T> CreateFromTransitions(List<Transition<T>> transitions, List<int> finalStates, int initialState)
    {
      var dfa = new Dfa<T>();
      var root = transitions.Where(x => x.From == initialState);
      foreach (var transition in root)
        dfa.m_root.Insert(transition.OnInput, transition.To);

      return dfa;
    }

    protected static IEnumerable<Transition<T>> GenerateTransitions(State state)
    {
      foreach (var child in state.Children)
      {
        yield return new Transition<T>(state.Id, child.Value.Id, child.Key);

        foreach (var transition in GenerateTransitions(child.Value))
          yield return transition;
      }
    }
  }
}
