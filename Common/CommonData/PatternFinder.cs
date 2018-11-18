using System;
using System.Linq;
using System.Collections.Generic;
using Common.Linq;

namespace Common.Data
{
  public class PatternFinder
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

    private abstract class RegexExpression
    {
      #region Properties

      protected abstract int Precedence { get; }
      public abstract int Length { get; }
      public virtual bool Solved { get; set; }

      #endregion

      #region Methods

      public abstract string ToString(string flags);

      public abstract bool Solve(int to, RegexExpression solution);

      #endregion
    }

    private class Alternation : RegexExpression
    {
      #region Fields

      private List<RegexExpression> m_options;

      #endregion

      #region Properties

      protected override int Precedence => 1;

      public override int Length => m_options.Count;

      public override bool Solved
      {
        get
        {
          foreach (var option in m_options)
            if (!option.Solved) return false;

          return true;
        }
      }

      #endregion

      public Alternation(IEnumerable<RegexExpression> options) => m_options = Flatten(options).OrderBy(x => x.Length).ToList();

      #region Methods

      private static IEnumerable<RegexExpression> Flatten(IEnumerable<RegexExpression> options)
      {
        foreach (var option in options)
        {
          if (option is Alternation alt)
            foreach (var subOption in Flatten(alt.m_options))
              yield return subOption;

          yield return option;
        }
      }

      public override string ToString(string flags) => string.Join("|", m_options);

      public override string ToString() => this.ToString(string.Empty);

      public override bool Solve(int to, RegexExpression solution)
      {
        var result = true;
        foreach (var option in m_options)
          result &= option.Solve(to, solution);
        return result;
      }

      #endregion
    }

    private class Literal : RegexExpression
    {
      #region Properties

      private string Value { get; set; }

      public override int Length => Value.Length;

      protected override int Precedence => 2;

      public int From { private get; set; }

      #endregion

      public Literal(string value) => Value = value;

      #region Methods

      public override string ToString(string flags) => Value;

      public override string ToString() => ToString(string.Empty);

      public override bool Solve(int to, RegexExpression solution)
      {
        if (Solved)
          return true;
        if (to != From)
          return false;
        if (solution is Alternation al && al.Length > 1)
          Value = $"({solution}){Value}";
        else
          Value = $"{solution}{Value}";

        return Solved = true;
      }

      #endregion
    }

    #endregion

    private Trie Strings { get; }

    private IEnumerable<PatternPart> FoundPatterns { get; set; }

    public PatternFinder(IEnumerable<string> strings)
    {
      Strings = new Trie();
      Strings.AddRange(strings);
    }

    public string FindPattern()
    {
      var minimized = DfaMinimizer<char>.Minimize(Strings);
      var transitions = minimized.GetTransitions().ToList();
      var info = minimized.GetAutomataInfo();

      var result = GenerateRegex(transitions, info.Item3, info.Item1);

      return result;
    }

    #region Static Methods

    private static string GenerateRegex(IEnumerable<Transition<char>> transitions, int initialState, int stateCount)
    {
      var solved = new Dictionary<int, RegexExpression> { { initialState, new Literal(string.Empty) { Solved = true } } };
      var solvedCount = 1;
      var toSolve = transitions
        .GroupBy(t => t.To)
        .ToDictionary<IGrouping<int, Transition<char>>, int, RegexExpression>(
          grouping => grouping.Key,
          grouping => new Alternation(grouping.Select(x => new Literal(x.OnInput.ToString())
          {
            Solved = false,
            From = x.From
          }))
        );

      while (solvedCount != stateCount)
      {
        var resolved = new Dictionary<int, RegexExpression>();
        foreach (var solution in solved)
          resolved = toSolve.Where(x => x.Value.Solve(solution.Key, solution.Value)).ToDictionary(x=> x.Key, x=> x.Value);

        if (resolved.Count > 0) solved.Clear();
        foreach (var solution in resolved)
        {
          solved.Add(solution.Key, solution.Value);
          toSolve.Remove(solution.Key);
        }

        solvedCount += resolved.Count;
      }

      return solved.FirstOrDefault().Value.ToString();
    }

    #endregion
  }
}
