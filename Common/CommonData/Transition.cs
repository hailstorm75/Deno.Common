using System;

namespace Common.Data
{
  /// <summary>
  /// Transition from state to state
  /// </summary>
  /// <typeparam name="T">Type of on input symbol</typeparam>
  [Serializable]
  public sealed class Transition<T>
  {
    #region Properties

    /// <summary>
    /// Transition From
    /// </summary>
    public int From { get; set; }
    /// <summary>
    /// Transition To
    /// </summary>
    public int To { get; set; }
    /// <summary>
    /// Accepted input
    /// </summary>
    public T OnInput { get; set; }

    #endregion

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="from">Transition start</param>
    /// <param name="to">Trantision end</param>
    /// <param name="onInput">Accepted input</param>
    public Transition(int from, int to, T onInput)
    {
      From = from;
      To = to;
      OnInput = onInput;
    }
  }
}
