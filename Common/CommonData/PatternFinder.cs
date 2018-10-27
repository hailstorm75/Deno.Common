using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Data
{
  static class MyClass
  {
    public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      HashSet<TKey> seenKeys = new HashSet<TKey>();
      foreach (TSource element in source)
      {
        if (seenKeys.Add(keySelector(element)))
        {
          yield return element;
        }
      }
    }
  }
  public class PatternFinder
  {
    #region Subclasses

    public enum PatternTypes
    {
      Any,
      Numeric,
      Character
    }

    public class Pattern
    {
      public List<PatternPart> Patterns { get; set; }
    }

    public abstract class PatternPart
    {
      public PatternTypes Type { get; private set; }

      protected PatternPart(PatternTypes type)
      {
        Type = type;
      }

      public abstract override string ToString();
    }

    public class Variable : PatternPart
    {
      public Variable(PatternTypes type) : base(type)
      {
      }

      public override string ToString()
      {
        switch (Type)
        {
          case PatternTypes.Any:
            return "*";
          case PatternTypes.Numeric:
            return "#";
          case PatternTypes.Character:
            return "@";
          default:
            return string.Empty;
        }
      }
    }

    public class Constant : PatternPart
    {
      public dynamic Value { get; private set; }

      public Constant(PatternTypes type, dynamic value) : base(type)
      {
        Value = value;
      }

      public override string ToString()
      {
        return $"\"{Value.ToString()}\"";
      }
    }

    #endregion

    public Trie Strings { private get; set; }

    private IEnumerable<PatternPart> FoundPatterns { get; set; }

    public PatternFinder(IEnumerable<string> strings)
    {
      Strings.AddRange(strings);
    }

    public PatternFinder FindPattern()
    {
      var minimized = DfaMinimizer<char>.Minimize(Strings);
      var transitions = minimized.GetTransitions().ToList();
      var acceptingStates = minimized.GetAcceptingStates().ToList();

      var result = GenerateRegex(transitions, acceptingStates);

      return this;
    }

    

    private string GenerateRegex(IReadOnlyCollection<Transition<char>> transitions, IEnumerable<int> finalStates)
    {
      for (var i = 0; i < transitions.DistinctBy(x => x.From).Count(); ++i)
      {


      }

      return string.Empty;
    }
  }
}
