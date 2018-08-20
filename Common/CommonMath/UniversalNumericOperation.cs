namespace Common.Math
{
  public static class UniversalNumericOperation
  {
    public static TResult Add<T1, T2, TResult>(T1 x, T2 y)
    {
      dynamic dx = x, dy = y;
      return (TResult)(dx + dy);
    }

    public static TResult Add<T, TResult>(params T[] ts)
    {
      dynamic res = ts[0];
      for (var i = 1; i < ts.Length; i++)
        res += (dynamic)ts[i];

      return (TResult)res;
    }

    public static TResult Subtract<T1, T2, TResult>(T1 x, T2 y)
    {
      dynamic dx = x, dy = y;
      return (TResult)(dx - dy);
    }

    public static TResult Subtract<T, TResult>(params T[] ts)
    {
      dynamic res = ts[0];
      for (var i = 1; i < ts.Length; i++)
        res -= (dynamic)ts[i];

      return (TResult)res;
    }

    public static TResult Multiply<T1, T2, TResult>(T1 x, T2 y)
    {
      dynamic dx = x, dy = y;
      return (TResult)(dx * dy);
    }

    public static TResult Multiply<T, TResult>(params T[] ts)
    {
      dynamic res = ts[0];
      for (var i = 1; i < ts.Length; i++)
        res *= (dynamic)ts[i];

      return (TResult)res;
    }

    public static TResult Divide<T1, T2, TResult>(T1 x, T2 y)
    {
      dynamic dx = x, dy = y;
      return (TResult)(dx / dy);
    }

    public static TResult Divide<T, TResult>(params T[] ts)
    {
      dynamic res = ts[0];
      for (var i = 1; i < ts.Length; i++)
        res *= (dynamic)ts[i];

      return (TResult)res;
    }
  }
}
