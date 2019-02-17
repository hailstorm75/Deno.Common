using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Common.Data.RegEx;

namespace Common.Data
{
  /// <summary>
  /// Generator of patterns for a set of strings
  /// </summary>
  public class PatternGenerator
  {
    #region Subclasses

    public enum PatternTypes
    {
      Any,
      Numeric,
      Character
    }

    private enum Side
    {
      Start,
      End,
      Unknown
    }

    public class Pattern
    {
      public List<PatternPart> Patterns { get; set; }
    }

    public abstract class PatternPart
    {
      public PatternTypes Type { get; private set; }

      protected PatternPart(PatternTypes type)
      {
        Type = type;
      }

      public abstract override string ToString();
    }

    public class Variable : PatternPart
    {
      public Variable(PatternTypes type) : base(type)
      {
      }

      public override string ToString()
      {
        switch (Type)
        {
          case PatternTypes.Any:
            return "*";
          case PatternTypes.Numeric:
            return "#";
          case PatternTypes.Character:
            return "@";
          default:
            return string.Empty;
        }
      }
    }

    public class Constant : PatternPart
    {
      public dynamic Value { get; private set; }

      public Constant(PatternTypes type, dynamic value) : base(type)
      {
        Value = value;
      }

      public override string ToString()
      {
        return $"\"{Value.ToString()}\"";
      }
    }

    #endregion

    /// <summary>
    /// Set of strings
    /// </summary>
    private Trie Strings { get; set; }

    private IEnumerable<PatternPart> FoundPatterns { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="strings">Set of strings to analyze</param>
    public PatternGenerator() { }

    public PatternGenerator LoadStrings(IEnumerable<string> strings)
    {
      OnOperationChanged(OperationArgs.OperationTypes.Loading);
      Strings = new Trie();
      Strings.AddRange(strings);

      return this;
    }

    /// <summary>
    /// Finds pattern based on <see cref="Strings"/>
    /// </summary>
    /// <returns>RegEx</returns>
    public RegularExpression FindPattern(CancellationToken ct = default)
    {
      try
      {
        OnOperationChanged(OperationArgs.OperationTypes.Minimizing);
        var minimized = DfaMinimizer<char>.Minimize(Strings, ct);

        OnOperationChanged(OperationArgs.OperationTypes.ExtractingData);
        var transitions = minimized.GetTransitions(ct).ToList();
        var info = minimized.GetAutomataInfo(ct);

        OnOperationChanged(OperationArgs.OperationTypes.Generating);
        var result = GenerateRegex(transitions, info.Item3, info.Item1, ct);

        OnOperationChanged(OperationArgs.OperationTypes.Finished);
        return result;
      }
      catch (OperationCanceledException)
      {
        OnOperationChanged(OperationArgs.OperationTypes.Cancelled);
        return null;
      }
    }

    #region Static Methods

    private static RegularExpression NewRegEx(IEnumerable<RegularExpression> regex)
    {
      var items = regex.ToList();
      return items.Count == 1
        ? items.First()
        : new Alternation(items);
    }

    /// <summary>
    /// Generates regex from <paramref name="transitions"/> of a DFA
    /// </summary>
    /// <param name="transitions">List of transitions in the DFA</param>
    /// <param name="initialState">The initial state of the DFA</param>
    /// <param name="stateCount">Total number of states in the DFA</param>
    /// <remarks>
    /// The algorithm is based on the Brzozowski Algebraic method described here: https://qntm.org/algo/
    /// </remarks>
    /// <returns>RegEx</returns>
    private static RegularExpression GenerateRegex(IEnumerable<Transition<char>> transitions, int initialState, int stateCount, CancellationToken ct = default)
    {
      // Latest substitutions eqautions
      // Initialization note:
      //    The initial state accepts Epsilon characters - string.Empty.
      //    It is the initial state - no need to set the 'From' property
      var substitutions = new Dictionary<int, RegularExpression>
      {
        {
          initialState,
          new Literal(string.Empty) { Solved = true }
        }
      };
      ct.ThrowIfCancellationRequested();

      // Total number of substituted equations, 1 because of initial state
      var substitutionsCount = 1;

      // Remaining equations to eliminate
      var toEliminate = transitions
        .GroupBy(t => t.To)
        .ToDictionary(
          grouping => grouping.Key,
          grouping => NewRegEx(grouping.Select(x => new Literal(x.OnInput.ToString())
          {
            Solved = false,
            From = x.From
          }))
        );

      /*
       * Note: the keys of the dictionaries above are from the 'To' property of 'Transition'
       * For example: 3 = 5a, where 3 is 'To', 5 is 'From' and 'a' is OnInput
       * Refer to the link in the remarks section
       */

      // Eliminate all equations
      while (substitutionsCount != stateCount)
      {
        ct.ThrowIfCancellationRequested();

        var substituted = new Dictionary<int, RegularExpression>();      // Stores newly substituted equations
        foreach (var solution in substitutions)                          // Tries to eliminate equations using substituted equations
        {
          ct.ThrowIfCancellationRequested();

          foreach (var x in toEliminate)
          {
            ct.ThrowIfCancellationRequested();

            if (x.Value is Literal literal)
            {
              if (literal.From != solution.Key)
                continue;

              substituted.Add(x.Key, literal.Join(solution.Value));
            }
            else if (x.Value.Substitute(solution.Key, solution.Value))
            {
              if (substituted.ContainsKey(x.Key))
                substituted[x.Key] = x.Value;
              else
                substituted.Add(x.Key, x.Value);
            }
          }
        }

        if (substituted.Count > 0) substitutions.Clear();               // Clears substitutions if they are obsolete
        foreach (var solution in substituted)                           // Updates dictionaries
        {
          ct.ThrowIfCancellationRequested();
          substitutions.Add(solution.Key, solution.Value);              // Updates list of substitutions
          toEliminate.Remove(solution.Key);                             // Eliminating equations
        }

        substitutionsCount += substituted.Count;                        // Increases number of solved equations
      }

      ct.ThrowIfCancellationRequested();

      // The remaining equation holds the solution
      switch (substitutions.FirstOrDefault().Value)
      {
        case Alternation alternation:
          return alternation.Simplify(ct);
        case Conjunction conjunction:
          return conjunction.Simplify(ct);
        default:
          return substitutions.FirstOrDefault().Value;
      }
    }

    #endregion

    public class OperationArgs
      : EventArgs
    {
      public OperationTypes OperationType { get; }

      public enum OperationTypes
      {
        Loading,
        Minimizing,
        ExtractingData,
        Generating,
        Finished,
        Cancelled
      }

      public OperationArgs(OperationTypes operationType)
        => OperationType = operationType;
    }

    public event EventHandler<OperationArgs> OperationChanged;

    protected virtual void OnOperationChanged(OperationArgs.OperationTypes type)
      => OperationChanged?.Invoke(this, new OperationArgs(type));
  }
}
