using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Common.Async
{
  public static class SafeInvokeExtensions
  {
    #region Fields

    public static int MainThreadId => locker.ReadLock(() => mainThreadId ?? -1);
    private static int? mainThreadId;
    private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    #endregion

    public static void InitializeWithCurrentThread() => locker.WriteLock(() => mainThreadId = Thread.CurrentThread.ManagedThreadId);

    #region Safe Invokes To Main Thread

    public static Task SafeInvokeToMainThreadAsync<T>(this T target, Action method)
      where T : ISynchronizeInvoke
    {
      return target.SafeInvokeAsync(() =>
      {
        if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
          return;

        method();
      });
    }

    public static Task<TOut> SafeInvokeToMainThreadAsync<T, TOut>(this T target, Func<TOut> method)
      where T : ISynchronizeInvoke
    {
      return target.SafeInvokeAsync(() =>
      {
        if (Thread.CurrentThread.ManagedThreadId != MainThreadId)
          return default(TOut);

        return method();
      });
    }

    public static Task SafeInvokeToMainThreadAsync<T>(this T target, Func<Task> taskFactory)
      where T : ISynchronizeInvoke
    {
      return target.SafeInvokeAsync(async () =>
      {
        if (Thread.CurrentThread.ManagedThreadId != MainThreadId) return;
        await taskFactory();
      });
    }

    public static Task<TOut> SafeInvokeToMainThreadAsync<T, TOut>(this T target, Func<Task<TOut>> taskFactory)
      where T : ISynchronizeInvoke
    {
      return target.SafeInvokeAsync(async () =>
      {
        return Thread.CurrentThread.ManagedThreadId != MainThreadId ? default(TOut) : await taskFactory();
      });
    }

    #endregion

    #region Safe Invokes

    public static async Task SafeInvokeAsync<T>(this T target, Action method)
      where T : ISynchronizeInvoke
    {
      if (!target.InvokeRequired) method();
      else
      {
        try
        {
          await Task.Run(() => target.Invoke(method, null));
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
      }
    }

    public static async Task SafeInvokeAsync<T, TParam>(this T target, Action<TParam> method, TParam param)
      where T : ISynchronizeInvoke
    {
      if (!target.InvokeRequired) method(param);
      else
      {
        try
        {
          await Task.Run(() => target.Invoke(method, new object[] { param }));
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
      }
    }

    public static async Task SafeInvokeAsync<T>(this T target, Func<Task> taskFactory)
      where T : ISynchronizeInvoke
    {
      if (!target.InvokeRequired) await taskFactory();
      else
      {
        try
        {
          await Task.Run(async () => await (Task)target.Invoke(new Func<Task>(() => taskFactory()), null));
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
      }
    }

    public static async Task<TOut> SafeInvokeAsync<T, TOut>(this T target, Func<TOut> method)
      where T : ISynchronizeInvoke
    {
      if (!target.InvokeRequired) return method();
      else
      {
        try
        {
          return await Task.Run(() => (TOut)target.Invoke(method, null));
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
      }

      return default(TOut);
    }

    public static async Task<TOut> SafeInvokeAsync<T, TOut>(this T target, Func<Task<TOut>> taskFactory)
      where T : ISynchronizeInvoke
    {
      if (!target.InvokeRequired) return await taskFactory();
      else
      {
        try
        {
          return await Task.Run(async () => await (Task<TOut>)target.Invoke(new Func<Task<TOut>>(() => taskFactory()), null));
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
      }

      return default(TOut);
    }

    #endregion
  }
}
