using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Data
{
  static class MyClass
  {
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      HashSet<TKey> seenKeys = new HashSet<TKey>();
      foreach (TSource element in source)
      {
        if (seenKeys.Add(keySelector(element)))
        {
          yield return element;
        }
      }
    }
  }

  public class PatternFinder
  {
    #region Subclasses

    public enum PatternTypes
    {
      Any,
      Numeric,
      Character
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

      public virtual bool IsEmpty => false;
      public virtual bool IsSingleCharacter => false;
      public virtual bool IsSingleCodepoint => false;

      #endregion

      #region Methods

      public abstract string ToString(string flags);

      protected static string Parens(RegexExpression exp, RegexExpression parent, string flags)
      {
        var isUnicode = flags != null && flags.IndexOf('u') != -1;

        var str = exp.ToString(flags);

        if (exp.Precedence < parent.Precedence && !exp.IsSingleCharacter && !(isUnicode && exp.IsSingleCodepoint))
          return $"(?:{str})";

        return str;
      }

      #endregion
    }

    /**
     * Represents an alternation (e.g. `foo|bar`)
     */
    private class Alternation : RegexExpression
    {
      #region Fields

      private IEnumerable<RegexExpression> m_options;

      #endregion

      #region Properties

      protected override int Precedence => 1;

      public override int Length => m_options.First().Length;

      #endregion

      public Alternation(params RegexExpression[] options) => m_options = Flatten(options.ToList()).OrderBy(x => x.Length);

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

      public override string ToString(string flags) => string.Join("|", m_options.Select(o => Parens(o, this, flags)));

      #endregion
    }

    /**
     * Represents a character class (e.g. [0-9a-z])
     */
    private class CharClass : RegexExpression
    {
      #region Properties

      protected override int Precedence => 1;

      public override int Length => 1;

      public override bool IsSingleCharacter => Set.Any(c => c.Length == 1);

      public override bool IsSingleCodepoint => true;

      public List<string> Set { get; }

      #endregion

      public CharClass(string a, string b)
      {
        Set = new List<string> { a, b };
      }

      public override string ToString(string flags)
      {
        //return this.set.toString({
        //  hasUnicodeFlag: flags && flags.indexOf('u') != -1
        //});
        return string.Empty;
      }
    }

    /**
     * Represents a concatenation (e.g. `foo`)
     */
    private class Concatenation : RegexExpression
    {
      #region Properties

      protected override int Precedence => 2;

      public override int Length => Lhs.Length + Rhs.Length;

      public RegexExpression Lhs { get; }

      public RegexExpression Rhs { get; }

      #endregion

      public Concatenation(RegexExpression lhs, RegexExpression rhs)
      {
        Lhs = lhs;
        Rhs = rhs;
      }

      #region Methods

      public override string ToString(string flags) => Parens(Lhs, this, flags) + Parens(Rhs, this, flags);

      public Literal GetLiteral(string side)
      {
        if (side == "start" && Lhs.Literal != null)
          return Lhs.GetLiteral(side);

        if (side == "end" && Rhs.Literal != null)
          return Rhs;

        return null;
      }

      public RegexExpression removeSubstring(string side = "", int len = 0)
      {
        if (side == "start" && a.removeSubstring() != null)
          a = a.removeSubstring(side, len);

        if (side == "end" && b.removeSubstring() != null)
          b = b.removeSubstring(side, len);

        return Lhs.IsEmpty ? Rhs : Rhs.IsEmpty ? Lhs as RegexExpression : new Concatenation(Lhs, Rhs);
      }

      #endregion
    }

    /**
     * Represents a repetition (e.g. `a*` or `a?`)
     */
    private class Repetition : RegexExpression
    {
      #region Properties

      public RegexExpression Expression { get; }

      public char Type { get; }

      protected override int Precedence => 3;

      public override int Length => Expression.Length;

      #endregion

      public Repetition(RegexExpression expr, char type)
      {
        Expression = expr;
        Type = type;
      }

      public override string ToString(string flags) => Parens(Expression, this, flags) + Type;
    }

    /**
     * Represents a literal (e.g. a string)
     */
    private class Literal : RegexExpression
    {
      #region Properties

      public string Value { get; }

      public override int Length => Value.Length;

      public override bool IsEmpty => Value == string.Empty;

      public override bool IsSingleCharacter => Length == 1;

      public override bool IsSingleCodepoint => IsSingleCharacter;

      public string Literal => Value;

      protected override int Precedence => 2;

      #endregion

      public Literal(string value) => Value = value;

      #region Methods

      public override string ToString(string flags)
      {
        //return jsesc(this.value, { es6: flags && flags.indexOf('u') != -1 })
        //.replace(/[\t\n\f\r\$\(\)\*\+\-\.\?\[\]\^\|]/ g, '\\$&')

        //// special handling to not escape curly braces which are part of Unicode escapes
        //.reeeace(/ (\nu\{[a-z0-9]+\})|([\{\}])/ig, (match, unicode, brace) => unicode || '\\' + brace);
        return string.Empty;
      }

      public CharClass GetCharClass() => IsSingleCodepoint ? new CharClass(Value, string.Empty) : null;

      public Literal RemoveSubstring(string side, int len)
      {
        switch (side)
        {
          case "start":
            return new Literal(this.Value.Substring(len));
          case "end":
            return new Literal(this.Value.Substring(0, this.Value.Length - len - 1));
          default:
            return this;
        }
      }

      #endregion
    }

    #endregion

    public Trie Strings { private get; set; }

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
      var acceptingStates = minimized.GetAcceptingStates().ToList();

      var result = GenerateRegex(transitions, acceptingStates, minimized.GetAutomataInfo().Item1);

      return result;
    }

    private string GenerateRegex(IReadOnlyCollection<Transition<char>> transitions, IReadOnlyCollection<int> finalStates, int numberOfState)
    {
      var B = transitions.DistinctBy(x => x.From).ToDictionary(state => state.From, state => finalStates.Contains(state.From) ? new Literal(string.Empty) as RegexExpression : null);
      var A = new RegexExpression[numberOfState, numberOfState];
      foreach (var transition in transitions)
        A[transition.From, transition.To] = A[transition.From, transition.To] != null ? Union(A[transition.From, transition.To], new Literal(transition.OnInput.ToString())) : new Literal(transition.OnInput.ToString());

      // Solve the of equations
      for (var n = numberOfState - 1; n >= 0; n--)
      {
        if (A[n, n] != null)
        {
          B[n] = Concat(Star(A[n, n]), B[n]);
          for (var j = 0; j < n; j++)
            A[n, j] = Concat(Star(A[n, n]), A[n, j]);
        }

        for (var i = 0; i < n; i++)
        {
          if (A[i, n] == null) continue;

          B[i] = Union(B[i], Concat(A[i, n], B[n]));
          for (var j = 0; j < n; j++)
            A[i, j] = Union(A[i, j], Concat(A[i, n], A[n, j]));
        }
      }

      return B[1].ToString("");
    }

    private static RegexExpression Star(RegexExpression exp) => exp != null ? new Repetition(exp, '*') : null;

    private static RegexExpression Union(RegexExpression a, RegexExpression b)
    {
      if (a == null || b == null || a == b) return a ?? b;

      // Hoist common substrings at the start and end of the options
      RegexExpression res;

      var s = RemoveCommonSubstring(a, b, "start");
      a = s.Item1;
      b = s.Item2;
      var start = s.Item3;

      var e = RemoveCommonSubstring(a, b, "end");
      a = e.Item1;
      b = e.Item2;
      var end = e.Item3;

      // If a or b is empty, make an optional group instead
      if (a.IsEmpty || b.IsEmpty)
        res = new Repetition(a.IsEmpty ? b : a, '?');

      else if (a is Repetition && ((Repetition)a).Type == '?')
        res = new Repetition(new Alternation(((Repetition)a).Expression, b), '?');
      else if (b is Repetition && ((Repetition)b).Type == '?')
        res = new Repetition(new Alternation(a, ((Repetition)b).Expression), '?');
      else
      {
        // Check if we can make a character class instead of an alternation
        var ac = a.getCharClass && a.getCharClass();
        var bc = b.getCharClass && b.getCharClass();

        res = ac && bc ? new CharClass(ac, bc) as RegexExpression : new Alternation(a, b);
      }

      if (start != null) res = new Concatenation(new Literal(start), res);
      if (end != null) res = new Concatenation(res, new Literal(end));

      return res;
    }

    private static Tuple<string, string, string> RemoveCommonSubstring(RegexExpression a, RegexExpression b, string side)
    {
      var al = a.getLiteral && a.getLiteral(side);
      var bl = b.getLiteral && b.getLiteral(side);
      if (!al || !bl)
        return new Tuple<string, string, string>(a, b, null);

      var s = CommonSubstring(al, bl, side);
      if (s == null)
        return new Tuple<string, string, string>(a, b, string.Empty);

      a = a.removeSubstring(side, s.Length);
      b = b.removeSubstring(side, s.Length);

      return new Tuple<string, string, string>(a, b, s);
    }

    private static string CommonSubstring(string a, string b, string side)
    {
      var dir = side == "start" ? 1 : -1;
      a = Array.from(a);
      b = Array.from(b);
      var ai = dir == 1 ? 0 : a.Length - 1;
      var ae = dir == 1 ? a.Length : -1;
      var bi = dir == 1 ? 0 : b.Length - 1;
      var be = dir == 1 ? b.Length : -1;
      var res = string.Empty;

      for (; ai != ae && bi != be && a[ai] == b[bi]; ai += dir, bi += dir)
        switch (dir)
        {
          case 1:
            res += a[ai];
            break;
          default:
            res = a[ai] + res;
            break;
        }

      return res;
    }

    private static RegexExpression Concat(RegexExpression a, RegexExpression b)
    {
      if (a == null || b == null) return null;

      if (a.IsEmpty) return b;

      if (b.IsEmpty) return a;

      // Combine literals
      {
        if (a is Literal literalA && b is Literal literalB)
          return new Literal(literalA.Value + literalB.Value);
      }
      {
        if (a is Literal literalA && b is Concatenation concatenationB && concatenationB.Lhs is Literal lhs)
          return new Concatenation(new Literal(literalA.Value + lhs.Value), concatenationB.Rhs);
      }
      {
        if (b is Literal literalA && a is Concatenation concatenationA && concatenationA.Rhs is Literal rhs)
          return new Concatenation(concatenationA.Lhs, new Literal(rhs.Value + literalA.Value));
      }

      return new Concatenation(a, b);
    }
  }
}
