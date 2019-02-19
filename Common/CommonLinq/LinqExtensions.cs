using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Common.Linq
{
  public static class LinqExtensions
  {
		/// <summary>
		/// Returns distinct elements from a sequence by using the default equality comparer to compare values by a given key.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <typeparam name="TKey">The type of keys by which the elements from <paramref name="source"/> will be compared by.</typeparam>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="keySelector">A retrieve function to select each sequence items given key.</param>
		/// <returns>An System.Collections.Generic.IEnumerable`1 that contains distinct elements from the source sequence.</returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      var seenKeys = new HashSet<TKey>();
      foreach (var element in source)
        if (seenKeys.Add(keySelector(element)))
          yield return element;
    }

    public static IEnumerable<TOut> ForEachDo<TSource, TOut>(this IEnumerable<TSource> source, Func<TSource, TOut> func)
      => source.Select(func);

		/// <summary>
		/// Sorts the elements of an ObservableCollection in ascending order according to a key.
		/// </summary>
		/// <typeparam name="T">The type of the elements of source.</typeparam>
		/// <param name="observable">A sequence of values to order.</param>
		/// <exception cref="ArgumentNullException"/>
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

		/// <summary>
		/// Splits elements of a sequence into smaller sequences of given size.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="chunkSize">Size to split into.</param>
		/// <returns>An IEnumerable of IEnumerables each of which contain a range of sequential values of type <typeparamref name="T"/>.</returns>
		public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
			=> source
				.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / chunkSize)
				.Select(x => x.Select(v => v.Value));

		/// <summary>
		/// Creates an IEnumerable instance from a single <paramref name="item"/> of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Item type</typeparam>
		/// <param name="item">Item to enumerate</param>
		/// <returns>Enumerable instance</returns>
		public static IEnumerable<T> FromSingleItem<T>(this T item)
		{
			yield return item;
		}
	}
}
