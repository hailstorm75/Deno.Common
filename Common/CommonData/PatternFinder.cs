using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
  public class PatternFinder
  {
    #region Subclasses

    public enum PatternTypes
    {
      Any,
      Numeric,
      Character
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

    public ICollection<string> Strings { private get; set; }

    private IEnumerable<PatternPart> FoundPatterns { get; set; }

    public PatternFinder(ICollection<string> strings)
    {
      Strings = strings;
    }

    public PatternFinder FindPattern()
    {
      switch (Strings.Count)
      {
        case 1:
          FoundPatterns = new List<PatternPart> { new Constant(PatternTypes.Character, Strings.First()) };
          break;
        case 2:
          var pattern = FindMatchingSubStr(Strings.First(), Strings.Last());

          FoundPatterns = pattern == string.Empty ? new List<PatternPart>() : new List<PatternPart> { new Constant(PatternTypes.Character, FindRoot()), new Variable(PatternTypes.Any) };
          break;
        default:
          FoundPatterns = new List<PatternPart> { new Constant(PatternTypes.Character, FindRoot()), new Variable(PatternTypes.Any) };
          break;
      }


      /*
       * testa
       * testb
       * testc
       * testf
       * 1testc
       * 4testn
       */

      return this;
    }

    public string Result()
    {
      return String.Join(" + ", FoundPatterns.Select(x => x.ToString()));
    }

    private static string FindMatchingSubStr(string lhs, string rhs)
    {
      var length = lhs.Length;
      int i;

      for (i = 0; i < length; i++)
      {
        if (lhs[i] != rhs[i])
          return lhs.Substring(0, i);
      }

      return string.Empty;
    }

    private string FindRoot()
    {
      var root = Strings.First();

      foreach (var item in Strings)
      {
        if (root.Length > item.Length)
          root = root.Substring(0, item.Length);

        for (var i = 0; i < root.Length; i++)
          if (root[i] != item[i])
            return root.Substring(0, i);
      }

      return root;
    }
  }
}
