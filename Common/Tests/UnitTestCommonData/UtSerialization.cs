using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnitTestConstants;

namespace Common.Data.Tests
{
  [TestFixture]
  public class UtSerialization
  {
    private static readonly List<Tuple<string, int, bool>> TestData = new List<Tuple<string, int, bool>>()
    {
      new Tuple<string, int, bool>("A", 50, true),
      new Tuple<string, int, bool>("B", 80, false),
      new Tuple<string, int, bool>("C", int.MaxValue, false)
    };

    private static readonly Dictionary<string, string> FilePaths = new Dictionary<string, string>
    {
      { "Binary", Path.Combine(Path.GetTempPath(), "Binary.test")},
      { "JSON2", Path.Combine(Path.GetTempPath(), "Json2.test")},
    };

    [Test, Category(Constants.EXTENSION)]
    public void BinarySerialize()
    {
      var stream = Serialization.SerializeBinary(TestData);
      var result = Serialization.DeserializeBinary<List<Tuple<string, int, bool>>>(stream);
      Assert.IsTrue(TestData.SequenceEqual(result));
    }

    //[Test, Category(Constants.EXTENSION)]
    //public void BinarySerializeFile()
    //{
    //  using (Stream stream = File.Open(FilePaths["Binary"], FileMode.CreateNew, FileAccess.Write))
    //    Serialization.SerializeBinary(TestData, stream);

    //  List<Tuple<string, int, bool>> result;
    //  using (Stream stream = File.Open(FilePaths["Binary"], FileMode.Open, FileAccess.Read))
    //    result = Serialization.DeserializeBinary<List<Tuple<string, int, bool>>>(stream);

    //  Assert.IsTrue(TestData.SequenceEqual(result));
    //}

    //[TearDown]
    //public static void Cleanup()
    //{
    //  foreach (var path in FilePaths.Select(x => x.Value))
    //    if (File.Exists(path))
    //      File.Delete(path);
    //}
  }
}