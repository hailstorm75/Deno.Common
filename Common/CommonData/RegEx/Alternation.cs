using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Data.RegEx
{
  /// <summary>
  /// Represents an alternation of regular expression parts
  /// </summary>
  internal class Alternation : RegularExpression
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
          var split = @string.Value.Split(new[] { rootString }, 2, StringSplitOptions.None);
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
}
