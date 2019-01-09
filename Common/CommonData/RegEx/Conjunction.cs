using System;
using System.Linq;
using System.Text;

namespace Common.Data.RegEx
{
  internal class Conjunction : RegularExpression, IReduceable
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
}
