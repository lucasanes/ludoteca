using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ludoteca
{
  public class GameControl
  {
    public List<Game> Games { get; set; } = new List<Game>();

    public GameControl()
    {
    }

    public void AddGame(ref int nextGameId)
    {
      try // [AV1-5]
      {
        Console.Write("Nome do game: ");
        string name = Console.ReadLine()?.Trim() ?? "";
        var game = new Game(nextGameId++, name);
        Games.Add(game);

        Logger.LogInfo($"Jogo cadastrado: ID={game.Id}, Nome={game.Name}");
        Console.WriteLine("Game cadastrado com sucesso.");
      }
      catch (Exception ex) // [AV1-5]
      {
        Logger.LogError("Erro ao cadastrar jogo", ex);
        throw;
      }
    }

    public void ValidateAllGames()
    {
      Logger.LogInfo("Iniciando validação de consistência dos jogos...");

      foreach (var game in Games)
      {
        game.ValidateConsistency();
      }

      // Verificar duplicatas de nome
      var duplicateNames = Games
        .GroupBy(g => g.Name.ToLower())
        .Where(g => g.Count() > 1)
        .ToList();

      foreach (var duplicate in duplicateNames)
      {
        Logger.LogError($"INCONSISTÊNCIA: Nome de jogo duplicado encontrado: {duplicate.Key}");
        foreach (var game in duplicate)
        {
          Logger.LogError($"  - Jogo ID {game.Id}: {game.Name}");
        }
      }

      Logger.LogInfo($"Validação de jogos concluída. Total de jogos: {Games.Count}");
    }

    public void ListGames()
    {
      if (Games.Count == 0)
      {
        Console.WriteLine("Nenhum game cadastrado.");
        return;
      }

      PrintGamesTableHeader();
      PrintGamesTableContent();
      PrintGamesTableFooter();
    }

    private void PrintGamesTableHeader()
    {
      Console.WriteLine();
      Console.WriteLine("┌─────┬─────────────────────────────────────┬───────────────┐");
      Console.WriteLine("│ ID  │               Nome                  │ Disponível    │");
      Console.WriteLine("├─────┼─────────────────────────────────────┼───────────────┤");
    }

    private void PrintGamesTableContent()
    {
      foreach (Game game in Games)
      {
        string name = game.Name.Length > 35 ? game.Name.Substring(0, 32) + "..." : game.Name;
        string available = game.Available ? "Sim" : "Não";
        Console.WriteLine($"│ {game.Id,-3} │ {name,-35} │ {available,-13} │");
      }
    }

    private void PrintGamesTableFooter()
    {
      Console.WriteLine("└─────┴─────────────────────────────────────┴───────────────┘");
      Console.WriteLine();
    }

    public void ReturnGame(List<Loan> Loans)
    {
      try // [AV1-5]
      {
        Console.Write("ID do empréstimo: ");
        if (!int.TryParse(Console.ReadLine(), out int loanId))
        {
          throw new ArgumentException("ID inválido.");
        }

        Loan loan = Loans.FirstOrDefault(e => e.Id == loanId) ?? throw new ArgumentException("Empréstimo não encontrado.");
        loan.RegisterReturn();

        Game? game = Games.FirstOrDefault(j => j.Id == loan.GameId);
        game?.MarkAsAvailable();

        Logger.LogInfo($"Jogo devolvido: EmprestimoID={loanId}, JogoID={loan.GameId}");
        Console.WriteLine("Game devolvido com sucesso.");
      }
      catch (Exception ex) // [AV1-5]
      {
        Logger.LogError("Erro ao devolver jogo", ex);
        throw;
      }
    }
  }
}