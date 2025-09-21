using System;
using System.IO;
using System.Text;

namespace Ludoteca
{
  public static class Logger
  {
    private const string Dir = "data";
    private const string DebugLogFile = Dir + "/debug.log";

    public static void LogError(string message, Exception? ex = null)
    {
      try
      {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"[{timestamp}] ERROR: {message}";

        if (ex != null)
        {
          logEntry += $"\nException: {ex.GetType().Name}: {ex.Message}";
          if (!string.IsNullOrEmpty(ex.StackTrace))
          {
            logEntry += $"\nStackTrace: {ex.StackTrace}";
          }
        }

        logEntry += "\n" + new string('-', 50) + "\n";

        EnsureDirectoryExists();
        File.AppendAllText(DebugLogFile, logEntry, Encoding.UTF8);
      }
      catch (Exception logException)
      {
        Console.WriteLine($"Erro ao escrever no log: {logException.Message}");
      }
    }

    public static void LogInfo(string message)
    {
      try
      {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"[{timestamp}] INFO: {message}\n";

        EnsureDirectoryExists();
        File.AppendAllText(DebugLogFile, logEntry, Encoding.UTF8);
      }
      catch (Exception logException)
      {
        Console.WriteLine($"Erro ao escrever no log: {logException.Message}");
      }
    }

    private static void EnsureDirectoryExists()
    {
      if (!Directory.Exists(Dir))
      {
        Directory.CreateDirectory(Dir);
      }
    }
  }
}
