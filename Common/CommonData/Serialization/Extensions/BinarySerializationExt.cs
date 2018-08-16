using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common.Data
{
  public static partial class SerializationExtensions
  {
    /// <summary>
    /// Serializes given <paramref name="input"/> to a stream
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <returns>Stream</returns>
    public static MemoryStream SerializeBinary(this object input)
    {
      var formatter = new BinaryFormatter();
      var stream = new MemoryStream();
      formatter.Serialize(stream, input);
      return stream;
    }

    /// <summary>
    /// Serializes given <paramref name="input"/> to a <paramref name="file"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <param name="file">File to serialize to</param>
    /// <returns>True if successful</returns>
    public static bool SerializeBinary(this object input, Stream file)
    {
      var formatter = new BinaryFormatter();
      try
      {
        formatter.Serialize(file, input);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
      finally
      {
        file?.Close();
      }
    }

    /// <summary>
    /// Deserializes given <paramref name="file"/> to an object of type <typeparamref name="T"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <typeparam name="T">Expected type of deserialized object</typeparam>>
    /// <param name="file"></param>
    /// <returns>Deserialized object</returns>
    public static T DeserializeBinary<T>(Stream file)
    {
      file.Seek(0, SeekOrigin.Begin);
      try
      {
        return (T)(new BinaryFormatter()).Deserialize(file);
      }
      finally
      {
        file?.Close();
      }
    }
  }
}
