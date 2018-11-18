using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Data.Tests
{
  [TestClass]
  public class UtSerialization
  {
    private static readonly List<Tuple<string, int, bool>> testData = new List<Tuple<string, int, bool>>()
    {
      new Tuple<string, int, bool>("A", 50, true),
      new Tuple<string, int, bool>("B", 80, false),
      new Tuple<string, int, bool>("C", int.MaxValue, false)
    };

    private static readonly Dictionary<string, string> filePaths = new Dictionary<string, string>
    {
      { "Binary", Path.Combine(Path.GetTempPath(), "Binary.test")},
      { "JSON2", Path.Combine(Path.GetTempPath(), "Json2.test")},
    };

    [TestMethod]
    public void BinarySerialize()
    {
      var stream = Serialization.SerializeBinary(testData);
      var result = Serialization.DeserializeBinary<List<Tuple<string, int, bool>>>(stream);
      Assert.IsTrue(testData.SequenceEqual(result));
    }

    [TestMethod]
    public void BinarySerializeFile()
    {
      using (Stream stream = File.Open(filePaths["Binary"], FileMode.CreateNew, FileAccess.Write))
        Serialization.SerializeBinary(testData, stream);

      List<Tuple<string, int, bool>> result;
      using (Stream stream = File.Open(filePaths["Binary"], FileMode.Open, FileAccess.Read))
        result = Serialization.DeserializeBinary<List<Tuple<string, int, bool>>>(stream);

      Assert.IsTrue(testData.SequenceEqual(result));
    }

    [ClassCleanup]
    public static void Cleanup()
    {
      foreach (var path in filePaths.Select(x => x.Value))
        if (File.Exists(path))
          File.Delete(path);
    }
  }
}
