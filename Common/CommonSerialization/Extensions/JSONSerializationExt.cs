using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Common.Serialization
{
  public static partial class SerializationExtensions
  {
    /// <summary>
    /// Contract resulver for <see cref="SerializeJSON2"/> to access non-public fields are properties
    /// </summary>
    private class CustomContractResolver : DefaultContractResolver
    {
      protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
      {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(p => base.CreateProperty(p, memberSerialization))
                        .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(f => base.CreateProperty(f, memberSerialization)))
                        .ToList();
        props.ForEach(p => { p.Writable = true; p.Readable = true; });
        return props;
      }
    }

    /// <summary>
    /// Serializes given <paramref name="input"/> to a JSON string
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <returns>JSON string</returns>
    public static string SerializeJSON(this object input)
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Serialize(input);
    }

    /// <summary>
    /// Deserializes given JSON string to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Expected type of deserialized object</typeparam>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static T DeserializeJSON<T>(string input)
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Deserialize<T>(input);
    }

    /// <summary>
    /// Serializes given <paramref name="input"/> to a JSON string
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <returns>JSON string</returns>
    public static string SerializeJSON2(this object input)
    {
      var settings = new JsonSerializerSettings()
      {
        ContractResolver = new CustomContractResolver(),
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        NullValueHandling = NullValueHandling.Ignore
      };

      return JsonConvert.SerializeObject(input, Formatting.None, settings);
    }

    /// <summary>
    /// Deserializes given JSON string to an object of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Expected type of deserialized object</typeparam>
    /// <param name="input">JSON string</param>
    /// <returns>Deserialized object</returns>
    public static T DeserializeJSON2<T>(string input)
    {
      var settings = new JsonSerializerSettings() { ContractResolver = new CustomContractResolver() };
      return JsonConvert.DeserializeObject<T>(input, settings);
    }

    /// <summary>
    /// Serializes given <paramref name="input"/> as a JSON string to a <paramref name="file"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <param name="input">Object to serialize</param>
    /// <param name="file">File to serialize to</param>
    /// <returns>True if successful</returns>
    public static bool SerializeJSON2(this object input, StreamWriter file)
    {
      var serializer = new JsonSerializer
      {
        Formatting = Formatting.None,
        ContractResolver = new CustomContractResolver(),
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects
      };

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
    /// Deserializes given JSON string from <paramref name="file"/> to an object of type <typeparamref name="T"/>
    /// <para>Automatically closes <paramref name="file"/></para>
    /// </summary>
    /// <typeparam name="T">Expected type of deserialized object</typeparam>
    /// <param name="file">File to deserialize from</param>
    /// <returns>Deserialized object</returns>
    public static T DeserializeJSON2<T>(StreamReader file)
    {
      var serializer = new JsonSerializer();

      try
      {
        return (T)serializer.Deserialize(file, typeof(T));
      }
      finally
      {
        file?.Close();
      }
    }
  }
}
