using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestConstants;

namespace Common.Math.Tests
{
  [TestClass]
  public class UtUniversalNumericOperation
  {
    [TestMethod, TestCategory(Constants.METHOD)]
    public void InvalidNumber()
    {
      Assert.IsFalse(new sbyte().IsNumber(), nameof(SByte));
      Assert.IsFalse(new byte().IsNumber(), nameof(Byte));
      Assert.IsFalse(new bool().IsNumber(), nameof(Boolean));
      Assert.IsFalse(new DateTime().IsNumber(), nameof(DateTime));
      Assert.IsFalse(' '.IsNumber(), nameof(Char));
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    public void InvalidInteger()
    {
      Assert.IsFalse(new sbyte().IsInteger(), nameof(SByte));
      Assert.IsFalse(new byte().IsInteger(), nameof(Byte));
      Assert.IsFalse(new bool().IsInteger(), nameof(Boolean));
      Assert.IsFalse(new DateTime().IsInteger(), nameof(DateTime));
      Assert.IsFalse(' '.IsInteger(), nameof(Char));

      Assert.IsFalse(new float().IsInteger(), nameof(Single));
      Assert.IsFalse(new double().IsInteger(), nameof(Double));
      Assert.IsFalse(new decimal().IsInteger(), nameof(Decimal));
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    public void InvalidSignedInteger()
    {
      Assert.IsFalse(new sbyte().IsSignedInteger(), nameof(SByte));
      Assert.IsFalse(new byte().IsSignedInteger(), nameof(Byte));
      Assert.IsFalse(new bool().IsSignedInteger(), nameof(Boolean));
      Assert.IsFalse(new DateTime().IsSignedInteger(), nameof(DateTime));
      Assert.IsFalse(' '.IsSignedInteger(), nameof(Char));

      Assert.IsFalse(new ushort().IsSignedInteger(), nameof(UInt16));
      Assert.IsFalse(new uint().IsSignedInteger(), nameof(UInt32));
      Assert.IsFalse(new ulong().IsSignedInteger(), nameof(UInt64));

      Assert.IsFalse(new float().IsSignedInteger(), nameof(Single));
      Assert.IsFalse(new double().IsSignedInteger(), nameof(Double));
      Assert.IsFalse(new decimal().IsSignedInteger(), nameof(Decimal));
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    public void InvalidUnsignedInteger()
    {
      Assert.IsFalse(new sbyte().IsUnsignedInteger(), nameof(SByte));
      Assert.IsFalse(new byte().IsUnsignedInteger(), nameof(Byte));
      Assert.IsFalse(new bool().IsUnsignedInteger(), nameof(Boolean));
      Assert.IsFalse(new DateTime().IsUnsignedInteger(), nameof(DateTime));
      Assert.IsFalse(' '.IsUnsignedInteger(), nameof(Char));

      Assert.IsFalse(new short().IsUnsignedInteger(), nameof(Int16));
      Assert.IsFalse(new int().IsUnsignedInteger(), nameof(Int32));
      Assert.IsFalse(new long().IsUnsignedInteger(), nameof(Int64));

      Assert.IsFalse(new float().IsUnsignedInteger(), nameof(Single));
      Assert.IsFalse(new double().IsUnsignedInteger(), nameof(Double));
      Assert.IsFalse(new decimal().IsUnsignedInteger(), nameof(Decimal));
    }

    [TestMethod, TestCategory(Constants.METHOD)]
    public void InvalidDecimal()
    {
      Assert.IsFalse(new sbyte().IsDecimal(), nameof(SByte));
      Assert.IsFalse(new byte().IsDecimal(), nameof(Byte));
      Assert.IsFalse(new bool().IsDecimal(), nameof(Boolean));
      Assert.IsFalse(new DateTime().IsDecimal(), nameof(DateTime));
      Assert.IsFalse(' '.IsDecimal(), nameof(Char));

      Assert.IsFalse(new short().IsDecimal(), nameof(Int16));
      Assert.IsFalse(new int().IsDecimal(), nameof(Int32));
      Assert.IsFalse(new long().IsDecimal(), nameof(Int64));

      Assert.IsFalse(new ushort().IsDecimal(), nameof(UInt16));
      Assert.IsFalse(new uint().IsDecimal(), nameof(UInt32));
      Assert.IsFalse(new ulong().IsDecimal(), nameof(UInt64));
    }
  }
}
