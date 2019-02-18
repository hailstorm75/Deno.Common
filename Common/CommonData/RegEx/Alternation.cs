using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Common.Data.RegEx
{
  /// <summary>
  /// Represents an alternation of regular expression parts
  /// </summary>
  public class Alternation
    : RegularExpression, ICanSimplify
  {
    private struct Pair<T1, T2>
    {
      public T1 Item1 { get; set; }
      public T2 Item2 { get; set; }

      public Pair(T1 item1, T2 item2)
      {
        Item1 = item1;
        Item2 = item2;
      }

      public override string ToString()
        => $"{Item1}:{Item2}";
    }

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
    public Alternation(IEnumerable<RegularExpression> parts) => Parts = Flatten(parts).OrderBy(x => x.Length).Distinct().ToList();

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
        else
          yield return part;
      }
    }

    /// <inheritdoc />
    public override string ToString()
      => string.Join("|", Parts);

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

    private enum Side
    {
      Left,
      Right,
      MiddleR,
      MiddleL
    }

    private static bool HasLiteral(RegularExpression regular, Side side)
    {
      switch (regular)
      {
        case Literal _:
          return true;
        case Conjunction conjunction:
          switch (side)
          {
            case Side.Left:
              return HasLiteral(conjunction.Parts.First(), side);
            case Side.Right:
              return HasLiteral(conjunction.Parts.Last(), side);
            case Side.MiddleR:
              return HasLiteral(conjunction.Parts.Last(), Side.MiddleL);
            case Side.MiddleL:
              return HasLiteral(conjunction.Parts.First(), Side.MiddleR);
            default:
              throw new ArgumentOutOfRangeException(nameof(side), side, null);
          }
        default:
          return false;
      }
    }

    private static Literal GetLiteral(RegularExpression regular, Side side)
    {
      switch (regular)
      {
        case Literal literal:
          return literal;
        case Conjunction conjunction:
          switch (side)
          {
            case Side.Left:
              return GetLiteral(conjunction.Parts.First(), side);
            case Side.Right:
              return GetLiteral(conjunction.Parts.Last(), side);
            case Side.MiddleR:
              return GetLiteral(conjunction.Parts.Last(), Side.MiddleL);
            case Side.MiddleL:
              return GetLiteral(conjunction.Parts.First(), Side.MiddleR);
            default:
              throw new ArgumentOutOfRangeException(nameof(side), side, null);
          }
        default:
          return null;
      }
    }

    private static RegularExpression ReduceLeft(Alternation part)
    {
      var regExp = part.Parts.GroupBy(x => HasLiteral(x, Side.Left)).ToList();
      var withLiterals = regExp.Where(x => x.Key)
                               .SelectMany(x => x)
                               .Select(x => new Pair<RegularExpression, string>(x, GetLiteral(x, Side.Left).Value))
                               .OrderBy(x => x.Item2)
                               .ToList();
      if (withLiterals.Count <= 1)
        return ReduceMiddle(part);

      var selected = new List<string>();
      var prefix = Trie.FindLongestCommonPrefix(withLiterals.Where(x => x.Item2 != string.Empty).Select(x => x.Item2).ToList(), selected);

      if (prefix == string.Empty)
        return ReduceMiddle(part);

      var filtered = withLiterals.Where(x => selected.Contains(x.Item2))
                                 .ToList();

      for (var i = 0; i < filtered.Count; i++)
      {
        var item = filtered[i];
        withLiterals.Remove(item);
        if (item.Item1 is IReduceable reduceable)
          item.Item1 = reduceable.ReduceLeft(prefix);
        else
          throw new Exception("Invalid type.");
      }

      var reduction = new List<RegularExpression>
      {
        new Conjunction(new Literal(prefix), new Alternation(filtered.Select(x => x.Item1)).Simplify())
      };
      reduction.AddRange(regExp.Where(x => !x.Key).SelectMany(x => x).Union(withLiterals.Select(x => x.Item1)));

      return new Alternation(reduction).Simplify();
    }

    private static string ReverseString(string s)
    {
      return s == string.Empty
        ? string.Empty
        : new string(s.Reverse().ToArray());
    }

    private static RegularExpression ReduceRight(Alternation part)
    {
      var regExp = part.Parts.GroupBy(x => HasLiteral(x, Side.Right)).ToList();
      var withLiterals = regExp.Where(x => x.Key)
                               .SelectMany(x => x)
                               .Select(x => new Pair<RegularExpression, string>(x, GetLiteral(x, Side.Right).Value))
                               .OrderBy(x => x.Item2)
                               .ToList();
      if (withLiterals.Count <= 1) return part;

      var selected = new List<string>();
      var suffix = ReverseString(Trie.FindLongestCommonPrefix(withLiterals.Where(x => x.Item2 != string.Empty).Select(x => ReverseString(x.Item2)).ToList(), selected));

      if (suffix == string.Empty) return part;

      var filtered = withLiterals.Where(x => selected.Contains(ReverseString(x.Item2)))
                                 .ToList();
      for (var i = 0; i < filtered.Count; i++)
      {
        var item = filtered[i];
        withLiterals.Remove(item);
        if (item.Item1 is IReduceable reduceable)
          item.Item1 = reduceable.ReduceLeft(suffix);
        else
          throw new Exception("Invalid type.");
      }

      var reduction = new List<RegularExpression>
      {
        new Conjunction(new Literal(suffix), new Alternation(filtered.Select(x => x.Item1)).Simplify())
      };
      reduction.AddRange(regExp.Where(x => !x.Key).SelectMany(x => x).Union(withLiterals.Select(x => x.Item1)));

      return new Alternation(reduction).Simplify();
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
          for (var j = 1; j + i < len; j++)
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

      var regExp = part.Parts.GroupBy(x => HasLiteral(x, Side.MiddleR)).ToList();
      var withLiterals = regExp.Where(x => x.Key).SelectMany(x => x).ToList();
      if (withLiterals.Count <= 1) return ReduceRight(part);
      var root = FindRoot(withLiterals.Select(x => GetLiteral(x, Side.MiddleR).Value).ToList());
      if (root == string.Empty) return ReduceRight(part);

      var leftSplit = new List<RegularExpression>();
      var rightSplit = new List<RegularExpression>();
      foreach (var literal in withLiterals)
      {
        if (literal is IReduceable reduceable)
        {
          var temp = reduceable.ReduceMiddle(root);
          leftSplit.Add(temp.Item1);
          rightSplit.Add(temp.Item2);
        }
        else
          throw new Exception("Invalid type.");
      }

      return new Conjunction(new Alternation(leftSplit).Simplify(), new Conjunction(new Literal(root), new Alternation(rightSplit).Simplify()));
    }

    public RegularExpression Simplify(CancellationToken ct = default)
    {
      for (var i = 0; i < Parts.Count; i++)
      {
        ct.ThrowIfCancellationRequested();
        if (Parts[i] is ICanSimplify alt)
          Parts[i] = alt.Simplify(ct);
      }

      return ReduceLeft(this);
    }

    #endregion
  }
}
