using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Ludoteca
{
  public class LoanControl
  {
    public List<Loan> Loans { get; set; } = [];

    public void ListLoans(MemberControl MemberControl, List<Game> Games)
    {
      if (Loans.Count == 0)
      {
        Console.WriteLine("Nenhum empréstimo cadastrado.");
        return;
      }

      Console.WriteLine();
      Console.WriteLine("┌─────┬──────────────────────┬──────────────────────┬─────────────────┬────────────────┐");
      Console.WriteLine("│ ID  │      Nome do Jogo    │    Nome do Membro    │ Data Empréstimo │ Data Devolução │");
      Console.WriteLine("├─────┼──────────────────────┼──────────────────────┼─────────────────┼────────────────┤");

      foreach (var loan in Loans)
      {
        string? gameName = Games.FirstOrDefault(g => g.Id == loan.GameId)?.Name ?? "N/A";
        string? memberName = MemberControl.Members.FirstOrDefault(m => m.Id == loan.MemberId)?.Name ?? "N/A";

        gameName = gameName.Length > 20 ? gameName.Substring(0, 17) + "..." : gameName;
        memberName = memberName.Length > 20 ? memberName.Substring(0, 17) + "..." : memberName;

        string loanDate = loan.LoanDate.ToString("dd/MM/yyyy");
        string returnDate = loan.ReturnDate.HasValue ? loan.ReturnDate.Value.ToString("dd/MM/yyyy") : "Não devolvido";

        Console.WriteLine($"│ {loan.Id,-3} │ {gameName,-20} │ {memberName,-20} │ {loanDate,-15} │ {returnDate,-14} │");
      }

      Console.WriteLine("└─────┴──────────────────────┴──────────────────────┴─────────────────┴────────────────┘");
      Console.WriteLine();
    }

    public void LendGame(MemberControl memberControl, List<Game> Games, ref int nextLoanId)
    {
      Console.Write("ID do game: ");
      if (!int.TryParse(Console.ReadLine(), out int gameId))
        throw new ArgumentException("ID inválido.");

      Console.Write("ID do membro: ");
      if (!int.TryParse(Console.ReadLine(), out int membroId))
        throw new ArgumentException("ID inválido.");

      var game = Games.FirstOrDefault(j => j.Id == gameId) ?? throw new ArgumentException("Game não encontrado.");
      var membro = memberControl.Members.FirstOrDefault(m => m.Id == membroId) ?? throw new ArgumentException("Membro não encontrado.");

      game.MarkAsLoan();

      var loan = new Loan(nextLoanId++, gameId, membroId);
      Loans.Add(loan);

      Console.WriteLine("Game emprestado com sucesso.");
    }

  }
}