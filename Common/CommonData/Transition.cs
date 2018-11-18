using System;

namespace Common.Data
{
  [Serializable]
  public sealed class Transition<T>
  {
    #region Properties

    public int From { get; set; }
    public int To { get; set; }
    public T OnInput { get; set; }

    #endregion

    public Transition(int from, int to, T onInput)
    {
      From = from;
      To = to;
      OnInput = onInput;
    }
  }
}
