using System;
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
    private abstract class RegularExpression
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
      public abstract bool Substitute(int @from, RegularExpression substitution);

      #endregion
    }

    private interface IReduceable
    {
      RegularExpression ReduceLeft(string prefix);
      RegularExpression ReduceRight(string suffix);
    }

    private class Conjunction : RegularExpression, IReduceable
    {
      #region Properties

      /// <inheritdoc />
      public override int Length => 2;

      /// <inheritdoc />
      public override bool Solved
      {
        get
        {
          foreach (var option in Parts)
            if (!option.Solved)
              return false;

          return true;
        }
      }

      public RegularExpression[] Parts { get; }

      #endregion

      public Conjunction(RegularExpression left, RegularExpression right)
      {
        Parts = new[]
        {
          left,
          right
        };
      }

      #region Methods

      public RegularExpression ReduceLeft(string prefix)
      {
        switch (Parts.First())
        {
          case Conjunction conj:
            {
              Parts[0] = conj.ReduceLeft(prefix);
              return this;
            }
          case Alternation _:
            throw new Exception("Part is an alternation.");
          case Literal literal:
            {
              literal.ReduceLeft(prefix);
              return this;
            }
          default:
            return this;
        }
      }

      public RegularExpression ReduceRight(string suffix)
      {
        switch (Parts.Last())
        {
          case Conjunction conj:
            {
              Parts[1] = conj.ReduceRight(suffix);
              return this;
            }
          case Alternation _:
            throw new Exception("Part is an alternation.");
          case Literal literal:
            {
              literal.ReduceLeft(suffix);
              return this;
            }
          default:
            return this;
        }
      }

      public RegularExpression AppendRight(string toAppend)
      {
        switch (Parts.Last())
        {
          case Conjunction conj:
            return new Conjunction(Parts[0], conj.AppendRight(toAppend));
          case Alternation _:
            return new Conjunction(Parts[0], new Conjunction(Parts[1], new Literal(toAppend)));
          case Literal literal:
            return new Conjunction(Parts[0], new Literal(literal.Value + toAppend));
          default:
            return this;
        }
      }

      public override bool Substitute(int from, RegularExpression substitution)
      {
        var result = true;
        foreach (var option in Parts)
          result &= option.Substitute(from, substitution);
        return result;
      }

      public override string ToString()
      {
        var sb = new StringBuilder();
        foreach (var part in Parts)
        {
          switch (part)
          {
            case Literal _:
              sb.Append(part);
              break;
            case Conjunction conjunction:
              {
                if (conjunction.Parts[0] is Literal)
                  sb.Append(part);
                else
                  sb.Append($"({part})");
                break;
              }
            default:
              sb.Append($"({part})");
              break;
          }
        }

        return sb.ToString();
      }

      #endregion
    }

    /// <summary>
    /// Represents an alternation of regular expression parts
    /// </summary>
    private class Alternation : RegularExpression
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

      public List<RegularExpression> Parts { get; private set; }

      #endregion

      /// <summary>
      /// Default constructor
      /// </summary>
      /// <param name="parts">Parts to alternate between</param>
      public Alternation(IEnumerable<RegularExpression> parts) => Parts = Flatten(parts).OrderBy(x => x.Length).ToList();

      #region Methods

      /// <summary>
      /// Flattens nested Alternations
      /// </summary>
      /// <param name="parts">Parts to flatten</param>
      /// <returns>Result</returns>
      private static IEnumerable<RegularExpression> Flatten(IEnumerable<RegularExpression> parts)
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
      public override bool Substitute(int from, RegularExpression substitution)
      {
        var result = true;
        for (var i = 0; i < Parts.Count; i++)
        {
          var option = Parts[i];
          if (option.Solved)
            continue;

          if (option is Literal lit)
          {
            if (@from != lit.From)
            {
              result = false;
              continue;
            }

            if (!(substitution is Literal))
            {
              Parts[i] = lit.Join(substitution);
              continue;
            }

            result &= Parts[i].Substitute(from, substitution);
            continue;
          }

          if ((substitution is Alternation alternation && alternation.Length < 2)
           || (substitution is Conjunction conjunction && conjunction.Parts.Last() is Literal))
            result &= Parts[i].Substitute(from, substitution);
          else
            Parts[i] = new Conjunction(substitution, option);
        }

        return result;
      }

      public static bool HasLiteral(RegularExpression regular, bool left)
      {
        switch (regular)
        {
          case Literal _:
            return true;
          case Conjunction conjunction:
            return HasLiteral(left ? conjunction.Parts.First() : conjunction.Parts.Last(), left);
          case Alternation _:
          default:
            return false;
        }
      }

      public static Literal GetLiteral(RegularExpression regular, bool left)
      {
        switch (regular)
        {
          case Literal literal:
            return literal;
          case Conjunction conjunction:
            return GetLiteral(left ? conjunction.Parts.First() : conjunction.Parts.Last(), left);
          default:
            return null;
        }
      }

      private static RegularExpression ReduceLeft(Alternation part)
      {
        var regExp = part.Parts.GroupBy(x => HasLiteral(x, true)).ToList();
        var withLiterals = regExp.Where(x => x.Key).SelectMany(x => x).ToList();
        if (withLiterals.Count <= 1) return ReduceRight(part);
        var prefix = Trie.FindCommonSubString(withLiterals.Select(x => GetLiteral(x, true).Value).ToList());

        if (prefix == string.Empty) return ReduceRight(part);

        for (var i = 0; i < withLiterals.Count; ++i)
        {
          if (withLiterals[i] is IReduceable reduceable)
            withLiterals[i] = reduceable.ReduceLeft(prefix);
          else
            throw new Exception("Invalid type.");
        }

        var reduction = new List<RegularExpression> { new Conjunction(new Literal(prefix), ReduceLeft(new Alternation(withLiterals))) };
        reduction.AddRange(regExp.Where(x => !x.Key).SelectMany(x => x));

        return new Alternation(reduction);
      }

      private static RegularExpression ReduceRight(Alternation part)
      {
        var regExp = part.Parts.GroupBy(x => HasLiteral(x, false)).ToList();
        var withLiterals = regExp.Where(x => x.Key).SelectMany(x => x).ToList();
        if (withLiterals.Count <= 1) return ReduceMiddle(part);
        var suffix = Trie.FindCommonSubString(withLiterals.Select(x => GetLiteral(x, false).Value).ToList());

        if (suffix == string.Empty) return ReduceMiddle(part);

        for (var i = 0; i < withLiterals.Count; ++i)
        {
          if (withLiterals[i] is IReduceable reduceable)
            withLiterals[i] = reduceable.ReduceRight(suffix);
          else
            throw new Exception("Invalid type.");
        }

        var reduction = new List<RegularExpression> { new Conjunction(ReduceLeft(new Alternation(withLiterals)), new Literal(suffix)) };
        reduction.AddRange(regExp.Where(x => !x.Key).SelectMany(x => x));

        return new Alternation(reduction);
      }

      private static RegularExpression ReduceMiddle(Alternation part)
      {
        string FindRoot(IReadOnlyList<string> strings)
        {
          var n = strings.Count;
          var s = strings.First();
          var len = s.Length;

          var temp = string.Empty;
          var result = string.Empty;

          for (var i = 0; i < len; i++)
          {
            for (var j = 1; j < len; j++)
            {
              var stem = s.Substring(i, j);
              var k = 1;

              for (; k < n; k++)
                if (!strings[k].Contains(stem))
                  goto BREAK;

              if (k == n && temp.Length < stem.Length)
                temp = stem;
            }

            BREAK:
            if (string.IsNullOrEmpty(temp)) continue;

            i += temp.Length - 1;
            result = temp;
            temp = string.Empty;
          }

          return result;
        }

        RegularExpression SplitByRoot(string rootString, IEnumerable<Literal> strings)
        {
          var left = new List<Literal>();
          var right = new List<Literal>();

          foreach (var @string in strings)
          {
            var split = @string.Value.Split(rootString, 2);
            left.Add(new Literal(split.First()));
            right.Add(new Literal(split.Last()));
          }

          return new Conjunction(new Alternation(left), new Conjunction(new Literal(rootString), new Alternation(right)));
        }

        var regExp = part.Parts;
        var literalValues = regExp.OfType<Literal>().ToList();
        if (literalValues.Count <= 1) return part;
        var root = FindRoot(literalValues.Select(x => x.Value).ToList());
        if (root == string.Empty) return part;

        return SplitByRoot(root, literalValues);
      }

      public RegularExpression Reduce()
      {
        for (var i = 0; i < Parts.Count; i++)
          if (Parts[i] is Alternation alt)
            Parts[i] = alt.Reduce();
        return ReduceLeft(this);
      }

      #endregion
    }

    /// <summary>
    /// Represents a literal
    /// </summary>
    private class Literal : RegularExpression, IReduceable
    {
      #region Properties

      /// <summary>
      /// Literal
      /// </summary>
      public string Value { get; private set; }

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
      public override bool Substitute(int from, RegularExpression substitution)
      {
        if (Solved)
          return true;
        if (from != From)
          return false;

        Value = $"{substitution}{Value}";

        return Solved = true;
      }

      public RegularExpression Join(RegularExpression substitution)
      {
        if (Solved) return this;

        Solved = true;

        if (substitution is Conjunction conjunction)
          return conjunction.AppendRight(Value);
        if (substitution is Literal literal)
        {
          Value = $"{substitution}{Value}";
          return this;
        }

        return new Conjunction(substitution, this);
      }

      #endregion

      public RegularExpression ReduceLeft(string prefix)
      {
        Value = Value.Substring(prefix.Length);
        return this;
      }

      public RegularExpression ReduceRight(string suffix)
      {
        Value = Value.Substring(0, Value.Length - suffix.Length);
        return this;
      }
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

      return result.ToString();
    }

    #region Static Methods

    private static RegularExpression NewRegEx(IEnumerable<RegularExpression> regex)
    {
      var items = regex.ToList();
      return items.Count == 1 ? items.First() : new Alternation(items);
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
    private static RegularExpression GenerateRegex(IEnumerable<Transition<char>> transitions, int initialState, int stateCount)
    {
      // Latest substitutions eqautions
      // Initialization note:
      //    The initial state accepts Epsilon characters - string.Empty.
      //    It is the initial state - no need to set the 'From' property
      var substitutions = new Dictionary<int, RegularExpression> { { initialState, new Literal(string.Empty) { Solved = true } } };
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
        var substituted = new Dictionary<int, RegularExpression>();                                 // Stores newly substituted equations
        foreach (var solution in substitutions)                                                   // Tries to eliminate equations using substituted equations
        {
          foreach (var x in toEliminate)
          {
            if (x.Value is Literal literal)
            {
              if (literal.From != solution.Key)
                continue;

              substituted.Add(x.Key, literal.Join(solution.Value));
            }
            else if (x.Value.Substitute(solution.Key, solution.Value))
              substituted.Add(x.Key, x.Value);
          }
        }

        if (substituted.Count > 0) substitutions.Clear();                  // Clears substitutions if they are obsolete
        foreach (var solution in substituted)                              // Updates dictionaries
        {
          substitutions.Add(solution.Key, solution.Value);                 // Updates list of substitutions
          toEliminate.Remove(solution.Key);                                // Eliminating equations
        }

        substitutionsCount += substituted.Count;                           // Increases number of solved equations
      }

      // The remaining equation holds the solution
      switch (substitutions.FirstOrDefault().Value)
      {
        case Alternation alternation:
          return alternation.Reduce();
        default:
          return substitutions.FirstOrDefault().Value;
      }
    }

    #endregion
  }
}
