using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common.Data.RegEx
{
  /// <summary>
  /// Represents a literal
  /// </summary>
  public class Literal
    : RegularExpression, IReduceable, IEquatable<RegularExpression>
  {
    #region Properties

    /// <summary>
    /// Literal
    /// </summary>
    public string Value { get; private set; }

    /// <inheritdoc />
    public override int Length
      => Value.Length;

    /// <summary>
    /// Variable to solve
    /// </summary>
    public int From { get; set; }

    #endregion

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="value">Literal value</param>
    public Literal(string value)
      => Value = value;

    #region Methods

    /// <inheritdoc />
    public override string ToString()
      => Regex.Escape(Value);

    /// <inheritdoc />
    public override bool Substitute(int from, RegularExpression substitution)
    {
      if (Solved)
        return true;
      if (from != From)
        return false;

      Value = $"{(substitution is Literal lit ? lit.Value : substitution.ToString())}{Value}";

      return Solved = true;
    }

    public RegularExpression Join(RegularExpression substitution)
    {
      if (Solved) return this;

      Solved = true;

      switch (substitution)
      {
        case Conjunction conjunction:
          return conjunction.AppendRight(Value);
        case Literal literal:
          Value = $"{literal.Value}{Value}";
          return this;
        default:
          return new Conjunction(substitution, this);
      }
    }

    #endregion

    public RegularExpression ReduceLeft(string prefix)
    {
      Value = Value.Substring(prefix.Length);
      return this;
    }

    public Tuple<RegularExpression, RegularExpression> ReduceMiddle(string root)
    {
      var split = Value.Split(new[] { root }, 2, StringSplitOptions.None);
      return new Tuple<RegularExpression, RegularExpression>(new Literal(split.FirstOrDefault()), new Literal(split.LastOrDefault()));
    }

    public RegularExpression ReduceRight(string suffix)
    {
      Value = Value.Substring(0, Value.Length - suffix.Length);
      return this;
    }

    private bool Equals(Literal other)
      => string.Equals(Value, other.Value) && Solved == other.Solved && (Solved || From == other.From);

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((Literal)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ (Solved ? 0 : From);
      }
    }

    public bool Equals(RegularExpression other)
      => Equals(other as Literal);
  }
}
