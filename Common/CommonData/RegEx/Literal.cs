namespace Common.Data.RegEx
{
  /// <summary>
  /// Represents a literal
  /// </summary>
  internal class Literal : RegularExpression, IReduceable
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
}
