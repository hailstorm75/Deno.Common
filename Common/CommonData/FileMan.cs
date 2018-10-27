﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Data
{
  public class FileMan
  {
    public enum EntityType
    {
      Unknown,
      File,
      Directory
    }

    /// <summary>
    /// Searches for an entity of given <see cref="name"/> and <see cref="type"/> in a <see cref="rootDirectory"/>
    /// </summary>
    /// <param name="name">Entity name</param>
    /// <param name="rootDirectory">Directory to search in</param>
    /// <param name="type">Type of searched entity</param>
    /// <param name="hint">Possible directory name containing given entity</param>
    public static void SearchInDirectory(string name, string rootDirectory, CancellationToken ct, EntityType type = EntityType.Unknown, string hint = "")
    {
      var reg = new Regex(name);

      if (type != EntityType.Directory)
      {
        var files = Directory.EnumerateFiles(rootDirectory, string.Empty, SearchOption.TopDirectoryOnly)
                             .Where(x => reg.IsMatch(name));
      }
    }
  }
}
