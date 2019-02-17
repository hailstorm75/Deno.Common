using System;
using System.Threading;

namespace Common.Data.RegEx
{
  internal interface IReduceable
  {
    RegularExpression ReduceLeft(string prefix);
    Tuple<RegularExpression, RegularExpression> ReduceMiddle(string root);
    RegularExpression ReduceRight(string suffix);
  }

  interface ICanSimplify
  {
    RegularExpression Simplify(CancellationToken ct);
  }
}
