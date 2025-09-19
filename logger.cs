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
        
        report.AppendLine("=== RELATÓRIO DO SISTEMA LUDOTECA ===");
        report.AppendLine($"Data de geração: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        report.AppendLine(new string('=', 50));
        report.AppendLine();
        
        report.AppendLine("ESTATÍSTICAS GERAIS:");
        report.AppendLine($"Total de membros cadastrados: {memberControl.Members.Count}");
        report.AppendLine($"Total de jogos cadastrados: {gameLibrary.Games.Count}");
        report.AppendLine($"Total de empréstimos realizados: {loanControl.Loans.Count}");
        report.AppendLine();
        
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
        
        report.AppendLine(new string('=', 50));
        report.AppendLine("Fim do relatório");
        
        File.WriteAllText(ReportFile, report.ToString(), Encoding.UTF8);
        LogInfo($"Relatório gerado com sucesso em {ReportFile}");
      }
      catch (Exception ex)
      {
        LogError("Erro ao gerar relatório", ex);
        throw;
      }
    }
    
    public static void AssertConsistency(MemberControl memberControl, GameControl gameLibrary, LoanControl loanControl)
    {
      try
      {
        LogInfo("Iniciando verificação de consistência dos dados...");
        
        memberControl.ValidateAllMembers();
        gameLibrary.ValidateAllGames();
        loanControl.ValidateAllLoans(gameLibrary.Games, memberControl.Members);
        
        var memberIds = memberControl.Members.Select(m => m.Id).ToList();
        var gameIds = gameLibrary.Games.Select(g => g.Id).ToList();
        var loanIds = loanControl.Loans.Select(l => l.Id).ToList();
        
        if (memberIds.Count != memberIds.Distinct().Count())
        {
          LogError("INCONSISTÊNCIA: IDs duplicados encontrados na lista de membros");
          var duplicates = memberIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
          foreach (var duplicate in duplicates)
          {
            LogError($"  - ID duplicado: {duplicate}");
          }
        }
        
        if (gameIds.Count != gameIds.Distinct().Count())
        {
          LogError("INCONSISTÊNCIA: IDs duplicados encontrados na lista de jogos");
          var duplicates = gameIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
          foreach (var duplicate in duplicates)
          {
            LogError($"  - ID duplicado: {duplicate}");
          }
        }
        
        if (loanIds.Count != loanIds.Distinct().Count())
        {
          LogError("INCONSISTÊNCIA: IDs duplicados encontrados na lista de empréstimos");
          var duplicates = loanIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
          foreach (var duplicate in duplicates)
          {
            LogError($"  - ID duplicado: {duplicate}");
          }
        }
        
        foreach (var loan in loanControl.Loans)
        {
          if (!gameIds.Contains(loan.GameId))
          {
            LogError($"INCONSISTÊNCIA: Empréstimo {loan.Id} referencia jogo inexistente (ID: {loan.GameId})");
          }
          
          if (!memberIds.Contains(loan.MemberId))
          {
            LogError($"INCONSISTÊNCIA: Empréstimo {loan.Id} referencia membro inexistente (ID: {loan.MemberId})");
          }
        }
        
        var activeLoans = loanControl.Loans.Where(l => !l.ReturnDate.HasValue).ToList();
        var loanedGameIds = activeLoans.Select(l => l.GameId).ToHashSet();
        
        foreach (var game in gameLibrary.Games)
        {
          bool shouldBeAvailable = !loanedGameIds.Contains(game.Id);
          
          if (game.Available != shouldBeAvailable)
          {
            LogError($"INCONSISTÊNCIA: Jogo {game.Id} ({game.Name}) tem status incorreto. " +
                    $"Disponível: {game.Available}, Deveria estar disponível: {shouldBeAvailable}");
          }
        }
        
        var duplicateGameLoans = activeLoans.GroupBy(l => l.GameId)
          .Where(g => g.Count() > 1)
          .ToList();
          
        foreach (var duplicateGroup in duplicateGameLoans)
        {
          LogError($"INCONSISTÊNCIA: Jogo {duplicateGroup.Key} possui múltiplos empréstimos ativos simultâneos");
          foreach (var loan in duplicateGroup)
          {
            LogError($"  - Empréstimo ID: {loan.Id}, Data: {loan.LoanDate:dd/MM/yyyy}");
          }
        }
        
        foreach (var loan in loanControl.Loans)
        {
          if (loan.ReturnDate.HasValue && loan.ReturnDate < loan.LoanDate)
          {
            LogError($"INCONSISTÊNCIA: Empréstimo {loan.Id} tem data de devolução anterior à data de empréstimo");
          }
          
          if (loan.LoanDate > DateTime.Now.AddDays(1))
          {
            LogError($"INCONSISTÊNCIA: Empréstimo {loan.Id} tem data de empréstimo futura: {loan.LoanDate:dd/MM/yyyy}");
          }
        }
        
        var severelyOverdueLoans = activeLoans.Where(l => DateTime.Now > l.DueDate.AddDays(30)).ToList();
        foreach (var overdueLoan in severelyOverdueLoans)
        {
          var daysOverdue = (DateTime.Now - overdueLoan.DueDate).Days;
          LogError($"ALERTA: Empréstimo {overdueLoan.Id} está {daysOverdue} dias em atraso (crítico)");
        }
        
        LogInfo("Verificação de consistência concluída");
        
        LogInfo($"RESUMO DA VALIDAÇÃO:");
        LogInfo($"  - Membros validados: {memberControl.Members.Count}");
        LogInfo($"  - Jogos validados: {gameLibrary.Games.Count}");
        LogInfo($"  - Empréstimos validados: {loanControl.Loans.Count}");
        LogInfo($"  - Empréstimos ativos: {activeLoans.Count}");
        LogInfo($"  - Empréstimos em atraso severo: {severelyOverdueLoans.Count}");
      }
      catch (Exception ex)
      {
        LogError("Erro durante verificação de consistência", ex);
        throw;
      }
    }
  }
}