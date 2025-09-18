using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ludoteca
{
  public class GameLibrary
  {
    public List<Game> Games { get; set; } = [];

    public GameLibrary()
    {
    }

    public void AddGame(ref int nextGameId)
    {
      Console.Write("Nome do game: ");
      string name = Console.ReadLine()?.Trim() ?? "";
      var game = new Game(nextGameId++, name);
      Games.Add(game);
      Console.WriteLine("Game cadastrado com sucesso.");
    }

    public void ListGames()
    {
      if (Games.Count == 0)
      {
        Console.WriteLine("Nenhum game cadastrado.");
        return;
      }

      Console.WriteLine();
      Console.WriteLine("┌─────┬─────────────────────────────────────┬───────────────┐");
      Console.WriteLine("│ ID  │               Nome                  │ Disponível    │");
      Console.WriteLine("├─────┼─────────────────────────────────────┼───────────────┤");

      foreach (var game in Games)
      {
        string name = game.Name.Length > 35 ? game.Name.Substring(0, 32) + "..." : game.Name;
        string available = game.Available ? "Sim" : "Não";
        Console.WriteLine($"│ {game.Id,-3} │ {name,-35} │ {available,-13} │");
      }

      Console.WriteLine("└─────┴─────────────────────────────────────┴───────────────┘");
      Console.WriteLine();
    }

    public void ReturnGame(List<Loan> Loans)
    {
      Console.Write("ID do empréstimo: ");
      if (!int.TryParse(Console.ReadLine(), out int loanId))
      {
        throw new ArgumentException("ID inválido.");
      }

      var loan = Loans.FirstOrDefault(e => e.Id == loanId) ?? throw new ArgumentException("Empréstimo não encontrado.");
      loan.RegisterReturn();

      var game = Games.FirstOrDefault(j => j.Id == loan.GameId);
      game?.MarkAsAvailable();

      Console.WriteLine("Game devolvido com sucesso.");
    }
  }
}