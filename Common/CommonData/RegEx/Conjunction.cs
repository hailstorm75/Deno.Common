using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Data.RegEx
{
  public class Conjunction : RegularExpression, IReduceable, ICanSimplify
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

    public RegularExpression Simplify(CancellationToken ct = default)
    {
      if (Parts.First() == Parts.Last() && Parts.First() is Alternation alt)
        return alt.Simplify(ct);
      if (Parts.First() is ICanSimplify conjL)
        Parts[0] = conjL.Simplify(ct);

      if (Parts.Last() is ICanSimplify conjR)
        Parts[1] = conjR.Simplify(ct);

      if (Parts[0] == null || Parts[0] is Literal litL && litL.Length == 0)
        return Parts[1];
      if (Parts[1] == null || Parts[1] is Literal litR && litR.Length == 0)
        return Parts[0];
      return this;
    }

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
            if (literal.Length == 0)
              return Parts.Last();
            return this;
          }
        default:
          return this;
      }
    }

    public Tuple<RegularExpression, RegularExpression> ReduceMiddle(string root)
    {
      return ReduceMiddle(root, 1);
    }

    private Tuple<RegularExpression, RegularExpression> ReduceMiddle(string root, int index)
    {
      switch (Parts[index])
      {
        case Conjunction conj:
          {
            var temp = conj.ReduceMiddle(root, index == 1 ? 0 : 1);
            if (index == 1)
            {
              var left = temp.Item1.Length > 0 ? new Conjunction(Parts[0], temp.Item1) : Parts[0];
              return new Tuple<RegularExpression, RegularExpression>(left, temp.Item2);
            }

            var right = temp.Item2.Length > 0 ? new Conjunction(temp.Item2, Parts[1]) : Parts[1];
            return new Tuple<RegularExpression, RegularExpression>(temp.Item1, right);
          }
        case Alternation _:
          throw new Exception("Part is an alternation.");
        case Literal literal:
          {
            var temp = literal.ReduceMiddle(root);
            if (index == 1)
            {
              var left = temp.Item1.Length > 0 ? new Conjunction(Parts[0], temp.Item1) : Parts[0];
              return new Tuple<RegularExpression, RegularExpression>(left, temp.Item2);
            }

            var right = temp.Item2.Length > 0 ? new Conjunction(temp.Item2, Parts[1]) : Parts[1];
            return new Tuple<RegularExpression, RegularExpression>(temp.Item1, right);
          }
        default:
          return null;
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
            if (literal.Length == 0)
            {
              return Parts.First();
            }
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

      switch (Parts.First())
      {
        case Literal _:
          sb.Append(Parts.First());
          break;
        case Conjunction conjunction:
          sb.Append($"{Parts.First()}");
          break;
        case Alternation alternation:
          sb.Append(alternation.Length > 1 ? $"({Parts.First()})" : $"{Parts.First()}");
          break;
      }
      switch (Parts.Last())
      {
        case Literal _:
          sb.Append(Parts.Last());
          break;
        case Conjunction conjunction:
          sb.Append($"{Parts.Last()}");
          break;
        case Alternation alternation:
          sb.Append(alternation.Length > 1 ? $"({Parts.Last()})" : $"{Parts.Last()}");
          break;

      }

      return sb.ToString();
    }

    #endregion
  }
}
