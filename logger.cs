using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Ludoteca
{
  public static class Logger
  {
    private const string Dir = "data";
    private const string DebugLogFile = Dir + "/debug.log";
    private const string ReportFile = Dir + "/relatorio.txt";

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

        File.AppendAllText(DebugLogFile, logEntry, Encoding.UTF8);
      }
      catch (Exception logException)
      {
        Console.WriteLine($"Erro ao escrever no log: {logException.Message}");
      }
    }

    public static void GenerateReport(MemberControl memberControl, GameControl gameLibrary, LoanControl loanControl)
    {
      try
      {
        var report = new StringBuilder();

        AddReportHeader(report);
        GenerateBasicStats(report, memberControl, gameLibrary, loanControl);
        GenerateGameStats(report, gameLibrary);
        GenerateLoanStats(report, loanControl);
        GenerateOverdueReport(report, loanControl, gameLibrary, memberControl);
        GenerateTopReports(report, loanControl, gameLibrary, memberControl);
        AddReportFooter(report);

        File.WriteAllText(ReportFile, report.ToString(), Encoding.UTF8);
        LogInfo($"Relatório gerado com sucesso em {ReportFile}");
      }
      catch (Exception ex)
      {
        LogError("Erro ao gerar relatório", ex);
        throw;
      }
    }

    private static void AddReportHeader(StringBuilder report)
    {
      report.AppendLine("=== RELATÓRIO DO SISTEMA LUDOTECA ===");
      report.AppendLine($"Data de geração: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
      report.AppendLine(new string('=', 50));
      report.AppendLine();
    }

    private static void GenerateBasicStats(StringBuilder report, MemberControl memberControl, GameControl gameLibrary, LoanControl loanControl)
    {
      report.AppendLine("ESTATÍSTICAS GERAIS:");
      report.AppendLine($"Total de membros cadastrados: {memberControl.Members.Count}");
      report.AppendLine($"Total de jogos cadastrados: {gameLibrary.Games.Count}");
      report.AppendLine($"Total de empréstimos realizados: {loanControl.Loans.Count}");
      report.AppendLine();
    }

    private static void GenerateGameStats(StringBuilder report, GameControl gameLibrary)
    {
      var availableGames = gameLibrary.Games.Count(g => g.Available);
      var loanedGames = gameLibrary.Games.Count(g => !g.Available);

      report.AppendLine("ESTATÍSTICAS DE JOGOS:");
      report.AppendLine($"Jogos disponíveis: {availableGames}");
      report.AppendLine($"Jogos emprestados: {loanedGames}");

      if (gameLibrary.Games.Count > 0)
      {
        double availabilityPercentage = (double)availableGames / gameLibrary.Games.Count * 100;
        report.AppendLine($"Taxa de disponibilidade: {availabilityPercentage:F2}%");
      }
      report.AppendLine();
    }

    private static void GenerateLoanStats(StringBuilder report, LoanControl loanControl)
    {
      var activeLoans = loanControl.Loans.Where(l => !l.ReturnDate.HasValue).ToList();
      var returnedLoans = loanControl.Loans.Where(l => l.ReturnDate.HasValue).ToList();
      var overdueLoans = activeLoans.Where(l => DateTime.Now > l.DueDate).ToList();

      report.AppendLine("ESTATÍSTICAS DE EMPRÉSTIMOS:");
      report.AppendLine($"Empréstimos ativos: {activeLoans.Count}");
      report.AppendLine($"Empréstimos devolvidos: {returnedLoans.Count}");
      report.AppendLine($"Empréstimos em atraso: {overdueLoans.Count}");

      if (loanControl.Loans.Count > 0)
      {
        double returnRate = (double)returnedLoans.Count / loanControl.Loans.Count * 100;
        report.AppendLine($"Taxa de devolução: {returnRate:F2}%");
      }
      report.AppendLine();
    }

    private static void GenerateOverdueReport(StringBuilder report, LoanControl loanControl, GameControl gameLibrary, MemberControl memberControl)
    {
      var activeLoans = loanControl.Loans.Where(l => !l.ReturnDate.HasValue).ToList();
      var overdueLoans = activeLoans.Where(l => DateTime.Now > l.DueDate).ToList();

      if (overdueLoans.Count > 0)
      {
        report.AppendLine("EMPRÉSTIMOS EM ATRASO:");
        foreach (var loan in overdueLoans)
        {
          var game = gameLibrary.Games.FirstOrDefault(g => g.Id == loan.GameId);
          var member = memberControl.Members.FirstOrDefault(m => m.Id == loan.MemberId);
          var daysOverdue = (DateTime.Now - loan.DueDate).Days;

          report.AppendLine($"- Empréstimo #{loan.Id}:");
          report.AppendLine($"  Jogo: {game?.Name ?? "N/A"}");
          report.AppendLine($"  Membro: {member?.Name ?? "N/A"}");
          report.AppendLine($"  Data do empréstimo: {loan.LoanDate:dd/MM/yyyy}");
          report.AppendLine($"  Data de vencimento: {loan.DueDate:dd/MM/yyyy}");
          report.AppendLine($"  Dias em atraso: {daysOverdue}");
          report.AppendLine();
        }
      }
    }

    private static void GenerateTopReports(StringBuilder report, LoanControl loanControl, GameControl gameLibrary, MemberControl memberControl)
    {
      GenerateTopGamesReport(report, loanControl, gameLibrary);
      GenerateTopMembersReport(report, loanControl, memberControl);
    }

    private static void GenerateTopGamesReport(StringBuilder report, LoanControl loanControl, GameControl gameLibrary)
    {
      var gameLoans = loanControl.Loans
        .GroupBy(l => l.GameId)
        .Select(g => new { GameId = g.Key, Count = g.Count() })
        .OrderByDescending(x => x.Count)
        .Take(5)
        .ToList();

      if (gameLoans.Any())
      {
        report.AppendLine("TOP 5 JOGOS MAIS EMPRESTADOS:");
        foreach (var gameLoan in gameLoans)
        {
          var game = gameLibrary.Games.FirstOrDefault(g => g.Id == gameLoan.GameId);
          report.AppendLine($"- {game?.Name ?? "N/A"}: {gameLoan.Count} empréstimos");
        }
        report.AppendLine();
      }
    }

    private static void GenerateTopMembersReport(StringBuilder report, LoanControl loanControl, MemberControl memberControl)
    {
      var memberLoans = loanControl.Loans
        .GroupBy(l => l.MemberId)
        .Select(g => new { MemberId = g.Key, Count = g.Count() })
        .OrderByDescending(x => x.Count)
        .Take(5)
        .ToList();

      if (memberLoans.Any())
      {
        report.AppendLine("TOP 5 MEMBROS MAIS ATIVOS:");
        foreach (var memberLoan in memberLoans)
        {
          var member = memberControl.Members.FirstOrDefault(m => m.Id == memberLoan.MemberId);
          report.AppendLine($"- {member?.Name ?? "N/A"}: {memberLoan.Count} empréstimos");
        }
        report.AppendLine();
      }
    }

    private static void AddReportFooter(StringBuilder report)
    {
      report.AppendLine(new string('=', 50));
      report.AppendLine("Fim do relatório");
    }
  }
}