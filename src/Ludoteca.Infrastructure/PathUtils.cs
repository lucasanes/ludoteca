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
        throw new InvalidOperationException("Não foi possível encontrar o diretório raiz do projeto (arquivo ludoteca.sln não encontrado)");
      }

      return currentDir;
    }

    public static string GetDataDirectory()
    {
      return Path.Combine(GetProjectRoot(), "data");
    }
  }
}
