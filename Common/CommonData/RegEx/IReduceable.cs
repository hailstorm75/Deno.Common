namespace Common.Data.RegEx
{
  internal interface IReduceable
  {
    RegularExpression ReduceLeft(string prefix);
    RegularExpression ReduceRight(string suffix);
  }
}
