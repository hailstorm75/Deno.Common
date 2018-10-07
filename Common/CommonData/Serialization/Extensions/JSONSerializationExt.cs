using System.IO;

namespace Common.Data
{
  public static partial class SerializationExtensions
  {
    /// <summary>
    /// Serializes given <paramref name="input"/> to a JSON string
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <returns>JSON string</returns>
    public static string SerializeJson(this object input) => Serialization.SerializeJson(input);

    /// <summary>
    /// Serializes given <paramref name="input"/> to a JSON string
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <returns>JSON string</returns>
    public static string SerializeJson2(this object input) => Serialization.SerializeJson2(input);

    /// <summary>
    /// Serializes given <paramref name="input"/> as a JSON string to a <paramref name="file"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <param name="file">File to serialize to</param>
    /// <returns>True if successful</returns>
    public static bool SerializeJson2(this object input, StreamWriter file) => Serialization.SerializeJson2(input, file);
  }
}
