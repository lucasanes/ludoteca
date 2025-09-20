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

      PrintLoansTableHeader();
      PrintLoansTableContent(MemberControl, Games);
      PrintLoansTableFooter();
    }

    private void PrintLoansTableHeader()
    {
      Console.WriteLine();
      Console.WriteLine("┌─────┬──────────────────────┬──────────────────────┬─────────────────┬────────────────┐");
      Console.WriteLine("│ ID  │      Nome do Jogo    │    Nome do Membro    │ Data Empréstimo │ Data Devolução │");
      Console.WriteLine("├─────┼──────────────────────┼──────────────────────┼─────────────────┼────────────────┤");
    }

    private void PrintLoansTableContent(MemberControl MemberControl, List<Game> Games)
    {
      foreach (Loan loan in Loans)
      {
        string? gameName = Games.FirstOrDefault(g => g.Id == loan.GameId)?.Name ?? "N/A";
        string? memberName = MemberControl.Members.FirstOrDefault(m => m.Id == loan.MemberId)?.Name ?? "N/A";

        gameName = gameName.Length > 20 ? gameName.Substring(0, 17) + "..." : gameName;
        memberName = memberName.Length > 20 ? memberName.Substring(0, 17) + "..." : memberName;

        string loanDate = loan.LoanDate.ToString("dd/MM/yyyy");
        string returnDate = loan.ReturnDate.HasValue ? loan.ReturnDate.Value.ToString("dd/MM/yyyy") : "Não devolvido";

        Console.WriteLine($"│ {loan.Id,-3} │ {gameName,-20} │ {memberName,-20} │ {loanDate,-15} │ {returnDate,-14} │");
      }
    }

    private void PrintLoansTableFooter()
    {
      Console.WriteLine("└─────┴──────────────────────┴──────────────────────┴─────────────────┴────────────────┘");
      Console.WriteLine();
    }

    public void LendGame(MemberControl memberControl, List<Game> Games, ref int nextLoanId)
    {
      try // [AV1-5]
      {
        Console.Write("ID do game: ");
        if (!int.TryParse(Console.ReadLine(), out int gameId))
          throw new ArgumentException("ID inválido.");

        Console.Write("ID do membro: ");
        if (!int.TryParse(Console.ReadLine(), out int membroId))
          throw new ArgumentException("ID inválido.");

        Game game = Games.FirstOrDefault(j => j.Id == gameId) ?? throw new ArgumentException("Jogo não encontrado.");
        Member membro = memberControl.Members.FirstOrDefault(m => m.Id == membroId) ?? throw new ArgumentException("Membro não encontrado.");

        game.MarkAsLoan();

        Loan loan = new Loan(nextLoanId++, gameId, membroId);
        Loans.Add(loan);

        Logger.LogInfo($"Empréstimo realizado: ID={loan.Id}, JogoID={gameId}, MembroID={membroId}, Data={loan.LoanDate:dd/MM/yyyy}");
        Console.WriteLine("Jogo emprestado com sucesso. Devolução em até 7 dias.");
      }
      catch (Exception ex) // [AV1-5]
      {
        Logger.LogError("Erro ao realizar empréstimo", ex);
        throw;
      }
    }

    public void ValidateAllLoans(List<Game> games, List<Member> members)
    {
      Logger.LogInfo("Iniciando validação de consistência dos empréstimos...");
      
      foreach (Loan loan in Loans)
      {
        loan.ValidateConsistency();
        
        if (!games.Any(g => g.Id == loan.GameId))
        {
          Logger.LogError($"INCONSISTÊNCIA: Empréstimo {loan.Id} referencia jogo inexistente (ID: {loan.GameId})");
        }
        
        if (!members.Any(m => m.Id == loan.MemberId))
        {
          Logger.LogError($"INCONSISTÊNCIA: Empréstimo {loan.Id} referencia membro inexistente (ID: {loan.MemberId})");
        }
      }
      
      Logger.LogInfo($"Validação de empréstimos concluída. Total de empréstimos: {Loans.Count}");
    }

  }
}