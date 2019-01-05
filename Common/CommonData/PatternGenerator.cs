using System.Linq;
using System.Collections.Generic;
using System.Text;

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

    /// <summary>
    /// Base class for regular expression parts
    /// </summary>
    private abstract class RegexExpression
    {
      #region Properties

      /// <summary>
      /// Number of elements in regular expression
      /// </summary>
      public abstract int Length { get; }
      /// <summary>
      /// True if equation solved
      /// </summary>
      public virtual bool Solved { get; set; }

      #endregion

      #region Methods

      /// <summary>
      /// Substitutes equation
      /// </summary>
      /// <param name="from">Number of state</param>
      /// <param name="substitution">Substitution <paramref name="from"/> state</param>
      /// <returns>True if the state was <paramref name="from"/> searched state</returns>
      public abstract bool Substitue(int @from, RegexExpression substitution);

      #endregion
    }

    private class Conjunction : RegexExpression
    {
      #region Properties

      /// <inheritdoc />
      public override int Length => Parts.Count;

      /// <inheritdoc />
      public override bool Solved
      {
        get
        {
          foreach (var option in Parts)
            if (!option.Solved) return false;

          return true;
        }
      }

      public List<RegexExpression> Parts { get; }

      #endregion

      public Conjunction(IEnumerable<RegexExpression> parts) => Parts = parts.ToList();

      #region Methods

      public override bool Substitue(int from, RegexExpression substitution)
      {
        var result = true;
        foreach (var option in Parts)
          result &= option.Substitue(from, substitution);
        return result;
      }

      public override string ToString()
      {
        switch (Parts.Count)
        {
          case 0: return string.Empty;
          case 1: return Parts.First().ToString();
          default:
            {
              var sb = new StringBuilder();
              foreach (var part in Parts)
              {
                switch (part)
                {
                  case Literal _:
                    sb.Append(part);
                    break;
                  case Alternation alternation:
                    var result = alternation.Simplify();
                    sb.Append(result is Alternation ? $"({result})" : $"{result}");
                    break;
                  default:
                    sb.Append($"({part})");
                    break;
                }
              }

              return sb.ToString();
            }
        }
      }

      #endregion
    }

    /// <summary>
    /// Represents an alternation of regular expression parts
    /// </summary>
    private class Alternation : RegexExpression
    {
      #region Properties

      /// <inheritdoc />
      public override int Length => Parts.Count;

      /// <inheritdoc />
      public override bool Solved
      {
        get
        {
          foreach (var option in Parts)
            if (!option.Solved) return false;

          return true;
        }
      }

      public List<RegexExpression> Parts { get; }

      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="parts">Parts to alternate between</param>
      public Alternation(IEnumerable<RegexExpression> parts) => Parts = Flatten(parts).OrderBy(x => x.Length).ToList();

      #region Methods

      /// <summary>
      /// Flattens nested Alternations
      /// </summary>
      /// <param name="parts">Parts to flatten</param>
      /// <returns>Result</returns>
      private static IEnumerable<RegexExpression> Flatten(IEnumerable<RegexExpression> parts)
      {
        foreach (var part in parts)
        {
          if (part is Alternation alt)
            foreach (var subOption in Flatten(alt.Parts))
              yield return subOption;

          yield return part;
        }
      }

      /// <inheritdoc />
      public override string ToString() => string.Join("|", Parts);

      /// <inheritdoc />
      public override bool Substitue(int from, RegexExpression substitution)
      {
        var result = true;
        for (var i = 0; i < Parts.Count; i++)
        {
          var option = Parts[i];
          if (option.Solved)
            continue;

          if (option is Literal lit && from != lit.From)
            result = false;

          if (substitution is Literal literal
            || (substitution is Alternation alternation && alternation.Length < 2)
            || (substitution is Conjunction conjunction && conjunction.Parts.Last() is Literal))
            result &= Parts[i].Substitue(from, substitution);
          else
            Parts[i] = new Conjunction(new[] { substitution, option });
        }

        return result;
      }

      public RegexExpression Simplify()
      {
        var items = new List<RegexExpression>();
        var prefix = Trie.FindCommonSubString(Parts.Select(x => x.ToString()).ToList());
        var suffix = Trie.FindCommonSubString(Parts.Select(x => new string(x.ToString().Reverse().ToArray())).ToList());
        var root = Parts.Select(x => x.ToString());

        if (prefix != string.Empty)
        {
          root = root.Select(x => x.ToString()).Select(x => x.Substring(prefix.Length));
          items.Add(new Literal(prefix));
        }

        if (suffix != string.Empty)
        {
          root = root.Select(x => x.ToString()).Select(x => x.Substring(0, x.Length - suffix.Length));
          items.Add(new Alternation(root.Select(x => new Literal(x))));
          items.Add(new Literal(suffix));
        }
        else
          items.Add(new Alternation(root.Select(x => new Literal(x))));

        return items.Count == 1
          ? items.First()             // Returning alternation
          : new Conjunction(items);
      }

      #endregion
    }

    /// <summary>
    /// Represents a literal
    /// </summary>
    private class Literal : RegexExpression
    {
      #region Properties

      /// <summary>
      /// Literal
      /// </summary>
      private string Value { get; set; }

      /// <inheritdoc />
      public override int Length => Value.Length;

      /// <summary>
      /// Variable to solve
      /// </summary>
      public int From { get; set; }

      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="value">Literal value</param>
      public Literal(string value) => Value = value;

      #region Methods

      /// <inheritdoc />
      public override string ToString() => Value;

      /// <inheritdoc />
      public override bool Substitue(int from, RegexExpression substitution)
      {
        if (Solved)
          return true;
        if (from != From)
          return false;

        Value = $"{substitution}{Value}";

        return Solved = true;
      }

      #endregion
    }

    #endregion

    /// <summary>
    /// Set of strings
    /// </summary>
    private Trie Strings { get; }

    private IEnumerable<PatternPart> FoundPatterns { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="strings">Set of strings to analyze</param>
    public PatternGenerator(IEnumerable<string> strings)
    {
      Strings = new Trie();
      Strings.AddRange(strings);
    }

    /// <summary>
    /// Finds pattern based on <see cref="Strings"/>
    /// </summary>
    /// <returns>RegEx</returns>
    public string FindPattern()
    {
      var minimized = DfaMinimizer<char>.Minimize(Strings);
      var transitions = minimized.GetTransitions().ToList();
      var info = minimized.GetAutomataInfo();

      var result = GenerateRegex(transitions, info.Item3, info.Item1);

      return result;
    }

    #region Static Methods

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
    private static string GenerateRegex(IEnumerable<Transition<char>> transitions, int initialState, int stateCount)
    {
      // Latest substitutions eqautions
      // Initialization note:
      //    The initial state accepts Epsilon characters - string.Empty.
      //    It is the initial state - no need to set the 'From' property
      var substitutions = new Dictionary<int, RegexExpression> { { initialState, new Literal(string.Empty) { Solved = true } } };
      // Total number of substituted equations, 1 because of initial state
      var substitutionsCount = 1;
      // Remaining equations to eliminate
      var toEliminate = transitions
        .GroupBy(t => t.To)
        .ToDictionary<IGrouping<int, Transition<char>>, int, RegexExpression>(
          grouping => grouping.Key,
          grouping => new Alternation(grouping.Select(x => new Literal(x.OnInput.ToString())
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
        var substituted = new Dictionary<int, RegexExpression>();                                 // Stores newly substituted equations
        foreach (var solution in substitutions)                                                   // Tries to eliminate equations using substituted equations
          substituted = toEliminate.Where(x => x.Value.Substitue(solution.Key, solution.Value))   // Selects those which were solved completely
                                   .ToDictionary(x => x.Key, x => x.Value);                         // Creates dictionary

        if (substituted.Count > 0) substitutions.Clear();                  // Clears substitutions if they are obsolete
        foreach (var solution in substituted)                              // Updates dictionaries
        {
          substitutions.Add(solution.Key, solution.Value);                 // Updates list of substitutions
          toEliminate.Remove(solution.Key);                                // Eliminating equations
        }

        substitutionsCount += substituted.Count;                           // Increases number of solved equations
      }

      // The remaining equation holds the solution
      return (substitutions.FirstOrDefault().Value as Alternation)?.Simplify().ToString();
    }

    #endregion
  }
}
