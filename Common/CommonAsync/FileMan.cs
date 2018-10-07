using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Async
{
  public static class FileMan
  {
    public static async Task<bool> TryDeleteFileAsync(string path)
    {
      try
      {
        using (var stream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.Delete, 4096, true))
        {
          await stream.FlushAsync();
          File.Delete(path);
        }

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public static async Task<bool> TryCopyFileAsync(string source, string destination)
    {
      try
      {
        using (var sourceStream = File.Open(source, FileMode.Open))
        {
          using (var destinationStream = File.Create(destination))
            await sourceStream.CopyToAsync(destinationStream);
        }

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public static async Task DoForFilesAsync(string rootDirectory, Func<string, Task> action, bool recursive = false, string searchPattern = "*")
    {
      var tasks = Directory.EnumerateFiles(rootDirectory, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(action);
      await Task.WhenAll(tasks.ToArray());
    }
  }
}
