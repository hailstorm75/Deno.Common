using System;
using System.Linq;
using System.Collections.Generic;

namespace Common.Linq
{
  public static class LinqExtensions
  {
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      var seenKeys = new HashSet<TKey>();
      foreach (var element in source)
        if (seenKeys.Add(keySelector(element)))
          yield return element;
    }

    public static IEnumerable<TOut> ForEachDo<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> func) => source.Select(func);
  }
}
