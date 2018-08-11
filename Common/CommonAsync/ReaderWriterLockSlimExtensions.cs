using System;
using System.Threading;

namespace Common.Async
{
  /// <summary>
  /// Helper extensions for the <see cref="ReaderWriterLockSlim"/> class
  /// </summary>
  public static class ReaderWriterLockSlimExtensions
  {
    /// <summary>
    /// Locks thread for read method execution
    /// </summary>
    /// <param name="lockSlim">Locker. Can be null</param>
    /// <param name="method">Method to execute</param>
    public static void ReadLock(this ReaderWriterLockSlim lockSlim, Action method)
    {
      if (lockSlim == null)
      {
        method();
        return;
      }

      lockSlim.EnterReadLock();
      try
      {
        method();
      }
      finally
      {
        lockSlim.ExitReadLock();
      }
    }

    /// <summary>
    /// Locks thread for read method execution
    /// </summary>
    /// <typeparam name="TResult">Type of value to return</typeparam>
    /// <param name="lockSlim">Locker. Can be null</param>
    /// <param name="method">Method to execute</param>
    /// <returns><paramref name="method"/> return</returns>
    public static TResult ReadLock<TResult>(this ReaderWriterLockSlim lockSlim, Func<TResult> method)
    {
      if (lockSlim == null) return method();

      lockSlim.EnterReadLock();
      try
      {
        return method();
      }
      finally
      {
        lockSlim.ExitReadLock();
      }
    }

    /// <summary>
    /// Locks thread for write method execution
    /// </summary>
    /// <param name="lockSlim">Locker. Can be null</param>
    /// <param name="method">Method to execute</param>
    public static void WriteLock(this ReaderWriterLockSlim lockSlim, Action method)
    {
      if (lockSlim == null)
      {
        method();
        return;
      }

      lockSlim.EnterWriteLock();
      try
      {
        method();
      }
      finally
      {
        lockSlim.ExitWriteLock();
      }
    }

    /// <summary>
    /// Locks thread for write method execution
    /// </summary>
    /// <typeparam name="TResult">Type of value to return</typeparam>
    /// <param name="lockSlim">Locker. Can be null</param>
    /// <param name="method">Method to execute</param>
    /// <returns><paramref name="method"/> return</returns>
    public static TResult WriteLock<TResult>(this ReaderWriterLockSlim lockSlim, Func<TResult> method)
    {
      if (lockSlim == null) return method();

      lockSlim.EnterWriteLock();
      try
      {
        return method();
      }
      finally
      {
        lockSlim.ExitWriteLock();
      }
    }

    /// <summary>
    /// Locks thread for upgradable method execution
    /// </summary>
    /// <param name="lockSlim">Locker. Can be null</param>
    /// <param name="method">Method to execute</param>
    public static void UpgradableLock(this ReaderWriterLockSlim lockSlim, Action method)
    {
      if (lockSlim == null)
      {
        method();
        return;
      }

      lockSlim.EnterUpgradeableReadLock();
      try
      {
        method();
      }
      finally
      {
        lockSlim.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Locks thread for upgradable method execution
    /// </summary>
    /// <typeparam name="TResult">Type of value to return</typeparam>
    /// <param name="lockSlim">Locker. Can be null</param>
    /// <param name="method">Method to execute</param>
    /// <returns><paramref name="method"/> return</returns>
    public static TResult UpgradableLock<TResult>(this ReaderWriterLockSlim lockSlim, Func<TResult> method)
    {
      if (lockSlim == null) return method();

      lockSlim.EnterUpgradeableReadLock();
      try
      {
        return method();
      }
      finally
      {
        lockSlim.ExitUpgradeableReadLock();
      }
    }
  }
}
