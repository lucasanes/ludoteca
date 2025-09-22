using System;
using System.IO;
using System.Reflection;

namespace Ludoteca
{
  public static class PathUtils
  {
    public static string GetProjectRoot()
    {
      var assemblyLocation = Assembly.GetExecutingAssembly().Location;
      var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

      var currentDir = assemblyDirectory;

      while (currentDir != null && !File.Exists(Path.Combine(currentDir, "ludoteca.sln")))
      {
        currentDir = Directory.GetParent(currentDir)?.FullName;
      }

      if (currentDir == null)
      {
        throw new InvalidOperationException("Could not find project root directory (ludoteca.sln not found)");
      }

      return currentDir;
    }

    public static string GetDataDirectory()
    {
      return Path.Combine(GetProjectRoot(), "data");
    }
  }
}
