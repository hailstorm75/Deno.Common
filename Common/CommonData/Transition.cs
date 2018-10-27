namespace Common.Data
{
  public sealed class Transition<T>
  {
    public int From { get; }
    public int To { get; }
    public T OnInput { get; }

    public Transition(int from, int to, T onInput)
    {
      From = from;
      To = to;
      OnInput = onInput;
    }
  }
}
