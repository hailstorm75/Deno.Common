using System.Collections.Generic;

namespace Common.Data
{
  public class Dfa<T>
  {
    public sealed class Transition
    {
      public ulong From { get; }
      public ulong To { get; }
      public T OnInput { get; }

      public Transition(ulong from, ulong to, T onInput)
      {
        From = from;
        To = to;
        OnInput = onInput;
      }
    }

    public sealed class State
    {
      #region Properties

      public readonly ulong Id;
      public Dictionary<T, State> Children { get; set; }

      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="value">Node value</param>
      /// <param name="depth">Node depth</param>
      /// <param name="parent">Node parent</param>
      /// <param name="id"></param>
      public State(ulong id)
      {
        Id = id;
        Children = new Dictionary<T, State>();
      }

      public State Insert(T c, ref ulong id)
      {
        if (!Children.ContainsKey(c))
          Children.Add(c, new State(id++));

        return this;
      }
    }

    #region Properties

    public virtual IEnumerable<ulong> FinateStates { get; }
    public virtual ulong StateCount { get; }
    public virtual ulong TransitionCount { get; }

    #endregion

    #region Fields

    protected HashSet<ulong> m_finateStates;
    protected HashSet<T> m_alphabet;

    #endregion

    public Dfa()
    {
      m_alphabet = new HashSet<T>();
      m_finateStates = new HashSet<ulong>();
    }

    public static Dfa<T> CreateFromTransitions(IEnumerable<Transition> transitions)
    {
      var dfa = new Dfa<T>();
      foreach (var transition in transitions)
      {
        var t = transition;
      }

      return dfa;
    }

    protected static IEnumerable<Transition> GenerateTransitions(State state)
    {
      foreach (var child in state.Children)
      {
        yield return new Transition(state.Id, child.Value.Id, child.Key);

        foreach (var transition in GenerateTransitions(child.Value))
          yield return transition;
      }
    }
  }
}
