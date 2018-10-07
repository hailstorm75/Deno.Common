using System.IO;

namespace Common.Data
{
  public static partial class SerializationExtensions
  {
    /// <summary>
    /// Serializes given <paramref name="input"/> to a stream
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <returns>Stream</returns>
    public static MemoryStream SerializeBinary(this object input) => Serialization.SerializeBinary(input);

    /// <summary>
    /// Serializes given <paramref name="input"/> to a <paramref name="file"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <param name="file">File to serialize to</param>
    /// <returns>True if successful</returns>
    public static bool SerializeBinary(this object input, Stream file) => Serialization.SerializeBinary(input, file);
  }
}
