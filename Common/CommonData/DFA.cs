using System.Collections.Generic;

namespace Common.Data
{
  public class Dfa<T>
  {
    public sealed class State
    {
      #region Properties

      public readonly int Id;
      public Dictionary<T, State> Children { get; set; }

      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="value">Node value</param>
      /// <param name="depth">Node depth</param>
      /// <param name="parent">Node parent</param>
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

    protected HashSet<int> m_finateStates;
    protected HashSet<T> m_alphabet;

    #endregion

    public Dfa()
    {
      m_alphabet = new HashSet<T>();
      m_finateStates = new HashSet<int>();
    }

    public static Dfa<T> CreateFromTransitions(IEnumerable<Transition<T>> transitions)
    {
      var dfa = new Dfa<T>();
      foreach (var transition in transitions)
      {
        var t = transition;
      }

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
