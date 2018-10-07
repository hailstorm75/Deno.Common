namespace Common.Math
{
  internal static class UniversalNumericOperation
  {
    #region Extensions

    #region Validators

    public static bool IsNumber<T>(this T value)
    {
      var type = typeof(T);

      return type == typeof(short)
          || type == typeof(int)
          || type == typeof(long)
          || type == typeof(ushort)
          || type == typeof(uint)
          || type == typeof(ulong)
          || type == typeof(float)
          || type == typeof(double)
          || type == typeof(decimal);
    }

    public static bool IsInteger<T>(this T value)
    {
      var type = typeof(T);
      return type == typeof(short)
          || type == typeof(int)
          || type == typeof(long)
          || type == typeof(ushort)
          || type == typeof(uint)
          || type == typeof(ulong);
    }

    public static bool IsSignedInteger<T>(this T value)
    {
      var type = typeof(T);
      return type == typeof(short)
          || type == typeof(int)
          || type == typeof(long);
    }

    public static bool IsUnsignedInteger<T>(this T value)
    {
      var type = typeof(T);
      return type == typeof(ushort)
          || type == typeof(uint)
          || type == typeof(ulong);
    }

    public static bool IsDecimal<T>(this T value)
    {
      var type = typeof(T);
      return type == typeof(float)
          || type == typeof(double)
          || type == typeof(decimal);
    }

    #endregion

    #region Operators

    public static T Add<T>(this T x, T y) => Add<T, T>(x, y);
    public static T Subtract<T>(this T x, T y) => Subtract<T, T>(x, y);
    public static T Multiply<T>(this T x, T y) => Multiply<T, T>(x, y);
    public static T Divide<T>(this T x, T y) => Divide<T, T>(x, y);
    public static T Modulo<T>(this T x, T y) => Modulo<T, T, T>(x, y);

    #endregion

    #endregion

    #region Operators

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
        res /= (dynamic)ts[i];

      return (TResult)res;
    }

    public static TResult Modulo<T1, T2, TResult>(T1 x, T2 y) => (TResult)((dynamic)x % (dynamic)y);

    public static bool IsEqual<T1, T2>(T1 x, T2 y) => (dynamic)x == (dynamic)y;
    public static bool IsLess<T1, T2>(T1 x, T2 y) => (dynamic)x < (dynamic)y;
    public static bool IsLessEqual<T1, T2>(T1 x, T2 y) => (dynamic)x <= (dynamic)y;
    public static bool IsGreater<T1, T2>(T1 x, T2 y) => (dynamic)x > (dynamic)y;
    public static bool IsGreaterEqual<T1, T2>(T1 x, T2 y) => (dynamic)x >= (dynamic)y;

    #endregion

    #region Methods

    public static T Abs<T>(T value) => System.Math.Abs((dynamic)value);

    public static T GetMaxValue<T>(T value)
    {
      var type = typeof(T).Name;
      switch (type)
      {
        case "Int16": return (T)(dynamic) short.MaxValue;
        case "Int32": return (T)(dynamic) int.MaxValue;
        case "Int64": return (T)(dynamic) long.MaxValue;
        case "Single": return (T)(dynamic) float.MaxValue;
        case "Double": return (T)(dynamic) double.MaxValue;
        default: return default(T);
      }
    }

    public static T GetMinValue<T>(T value)
    {
      var type = typeof(T).Name;
      switch (type)
      {
        case "Int16": return (T)(dynamic) short.MinValue;
        case "Int32": return (T)(dynamic) int.MinValue;
        case "Int64": return (T)(dynamic) long.MinValue;
        case "Single": return (T)(dynamic) float.MinValue;
        case "Double": return (T)(dynamic) double.MinValue;
        default: return default(T);
      }
    }

    #endregion
  }
}
