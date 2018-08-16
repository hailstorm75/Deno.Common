using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Data.Tests
{
  [TestClass]
  public class UtExtensions
  {
    private List<Tuple<string, int, bool>> TestData { get; } = new List<Tuple<string, int, bool>>()
    {
      new Tuple<string, int, bool>("A", 50, true),
      new Tuple<string, int, bool>("B", 80, false),
      new Tuple<string, int, bool>("C", int.MaxValue, false)
    };

    [TestMethod]
    public void BinarySerialize()
    {
      var stream = TestData.SerializeBinary();
      var result = Serialization.DeserializeBinary<List<Tuple<string, int, bool>>>(stream);
      Assert.IsTrue(TestData.SequenceEqual(result));
    }

    [TestMethod]
    public void JsonSerialize()
    {
      var json = TestData.SerializeJson();
      var result = Serialization.DeserializeJson<List<Tuple<string, int, bool>>>(json);
      Assert.IsTrue(TestData.SequenceEqual(result));
    }

    [TestMethod]
    public void JsonSerialize2()
    {
      var json = TestData.SerializeJson2();
      var result = Serialization.DeserializeJson2<List<Tuple<string, int, bool>>>(json);
      Assert.IsTrue(TestData.SequenceEqual(result));
    }

    //[TestMethod]
    //public void XMLSerialize()
    //{

    //}
  }
}
