using System;
using System.Reflection;

namespace Common.Data
{
  /// <summary>
  /// Class that can be derived from to create a singleton. The only issue is that the type must have a non-public parameterless constructor.
  /// </summary>
  /// <typeparam name="T">Any type inherited from Singleton</typeparam>
  public abstract class Singleton<T> where T
		: Singleton<T>
  {
		/// <summary>
		/// Singlegon instance of <typeparamref name="T"/>
		/// </summary>
    public static T Instance => INSTANCE.Value;

    /// <summary>
    /// Lazily created instance variable.
    /// </summary>
    private static readonly Lazy<T> INSTANCE;

    /// <summary>
    /// Static constructor.
    /// </summary>
    static Singleton() => INSTANCE = new Lazy<T>(InstanceFactory);

    /// <summary>
    /// Calls a non-public empty constructor on derived type to create instance. If no such constructor exists a TypeAccessException is thrown.
    /// </summary>
    private static T InstanceFactory()
    {
      var type = typeof(T);
      var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      if (constructors.Length != 1)
        throw new TypeInitializationException(type.FullName,
          new TypeAccessException($"Type must contain a single (non-public) constructor if derived from {nameof(Singleton<T>)}."));
      var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

      // Make sure we found our one and only private or protected constructor.
      if (ctor == null || (!ctor.IsPrivate && !ctor.IsFamily))
        throw new TypeInitializationException(type.FullName,
          new TypeAccessException($"Type must contain a single (non-public) constructor if derived from {nameof(Singleton<T>)}."));

      if (!(ctor.Invoke(new object[] { }) is T instance))
        throw new TypeInitializationException(type.FullName, new NullReferenceException());

      return instance;
    }
  }
}
