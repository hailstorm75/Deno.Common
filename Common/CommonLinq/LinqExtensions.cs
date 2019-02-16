using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

    public static IEnumerable<TOut> ForEachDo<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> func)
      => source.Select(func);

    public static void Sort<T>(this ObservableCollection<T> observable) where T : IComparable<T>, IEquatable<T>
    {
      var sorted = observable.OrderBy(x => x).ToList();

      var ptr = 0;
      while (ptr < sorted.Count)
      {
        if (!observable[ptr].Equals(sorted[ptr]))
        {
          var t = observable[ptr];
          observable.RemoveAt(ptr);
          observable.Insert(sorted.IndexOf(t), t);
        }
        else ptr++;
      }
    }

    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
    {
      return source
        .Select((x, i) => new { Index = i, Value = x })
        .GroupBy(x => x.Index / chunkSize)
        .Select(x => x.Select(v => v.Value));
    }
  }
}
