using System;
using System.Collections.Generic;

namespace Common.Data
{
  /// <inheritdoc />
  /// <summary>
  /// Deterministic finite automaton
  /// </summary>
  /// <typeparam name="T">Type of input symbol</typeparam>
  [Serializable]
  public abstract class BaseDfa<T> : IGraph
  {
    /// <inheritdoc />
    /// <summary>
    /// Automaton state
    /// </summary>
    [Serializable]
    protected sealed class State : INode
    {
      #region Properties

      /// <inheritdoc />
      /// <summary>
      /// <see cref="T:Common.Data.BaseDfa`1.State" /> Id
      /// </summary>
      public int Id { get; }
      /// <summary>
      /// States connected to this <see cref="State"/>
      /// </summary>
      public Dictionary<T, State> Neighbours { get; }

      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="id"></param>
      public State(int id)
      {
        Id = id;
        Neighbours = new Dictionary<T, State>();
      }

      /// <summary>
      /// Connects a <see cref="State"/> with a unique <paramref name="id"/>
      /// </summary>
      /// <param name="symbol">Input symbol</param>
      /// <param name="id">Id of new <see cref="State"/></param>
      /// <returns>Next id</returns>
      public int Connect(T symbol, int id)
      {
        if (Neighbours.ContainsKey(symbol)) return id;

        Neighbours.Add(symbol, new State(id));
        return id + 1;
      }
    }

    #region Properties

    /// <summary>
    /// Accepting states
    /// </summary>
    public abstract IEnumerable<int> FiniteStates { get; }
    /// <summary>
    /// Total number of states in automaton
    /// </summary>
    public abstract int StateCount { get; }
    /// <summary>
    /// Total number of transitions between states in automaton
    /// </summary>
    public abstract int TransitionCount { get; }

    #endregion

    #region Fields

    protected State m_root;
    protected HashSet<int> m_finateStates;
    protected HashSet<T> m_alphabet;

    #endregion

    /// <summary>
    /// Default constructor
    /// </summary>
    protected BaseDfa()
    {
      m_root = new State(0);
      m_alphabet = new HashSet<T>();
      m_finateStates = new HashSet<int>();
    }

    /// <summary>
    /// Recursively retrieves transtions between states
    /// </summary>
    /// <param name="state">Root state</param>
    /// <returns>Transtions</returns>
    protected static IEnumerable<Transition<T>> GenerateTransitions(State state)
    {
      foreach (var child in state.Neighbours)
      {
        yield return new Transition<T>(state.Id, child.Value.Id, child.Key);

        foreach (var transition in GenerateTransitions(child.Value))
          yield return transition;
      }
    }
  }
}
