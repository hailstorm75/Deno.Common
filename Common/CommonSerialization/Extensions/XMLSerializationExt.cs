using System;
using System.IO;
using System.Xml.Serialization;

namespace Common.Serialization
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
    public static bool SerializeXml(this object input, StreamWriter file)
    {
      var serializer = new XmlSerializer(input.GetType());
      try
      {
        serializer.Serialize(file, input);
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
    /// Deserializes given XML string from <paramref name="file"/> to an object of type <typeparamref name="T"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <typeparam name="T">Expected type of deserialized object</typeparam>
    /// <param name="file">File to deserialize from</param>
    /// <returns>Deserialized object</returns>
    public static T DeserializeXml<T>(StreamReader file)
    {
      var serializer = new XmlSerializer(typeof(T));
      try
      {
        return (T)serializer.Deserialize(file);
      }
      finally
      {
        file?.Close();
      }
    }
  }
}
