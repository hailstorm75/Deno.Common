using System.IO;

namespace Common.Data
{
  public static partial class SerializationExtensions
  {
    /// <summary>
    /// Serializes given <paramref name="input"/> as an XML string to a <paramref name="file"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <param name="file">File to serialize to</param>
    /// <returns>True if successful</returns>
    public static bool SerializeXml(this object input, StreamWriter file) => Serialization.SerializeXml(input, file);
  }
}
