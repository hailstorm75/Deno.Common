namespace Common.Data.RegEx
{
  /// <summary>
  /// Base class for regular expression parts
  /// </summary>
  internal abstract class RegularExpression
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
}
