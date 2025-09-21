using System;

namespace Ludoteca
{
  public class Loan
  {
    public int Id { get; private set; }
    public int GameId { get; private set; }
    public int MemberId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate => LoanDate.AddDays(7);
    public DateTime? ReturnDate { get; private set; }

    public Loan(int id, int gameId, int memberId, DateTime? returnDate = null)
    {
      if (id <= 0)
        throw new ArgumentException("ID do empréstimo deve ser maior que zero.");
      if (gameId <= 0)
        throw new ArgumentException("ID do jogo inválido.");
      if (memberId <= 0)
        throw new ArgumentException("ID do membro inválido.");
      if (returnDate.HasValue && returnDate.Value < DateTime.Now.AddDays(-365))
        throw new ArgumentException("Data de devolução não pode ser muito antiga.");

      Id = id;
      GameId = gameId;
      MemberId = memberId;
      LoanDate = DateTime.Now;
      ReturnDate = returnDate;

    }

    public void RegisterReturn()
    {
      if (ReturnDate.HasValue)
        throw new InvalidOperationException("Empréstimo já devolvido.");
      ReturnDate = DateTime.Now;
      Logger.LogInfo($"Devolução registrada para empréstimo {Id} em {ReturnDate:dd/MM/yyyy HH:mm:ss}");
    }

    public void ValidateConsistency()
    {
      if (Id <= 0)
        Logger.LogError($"INCONSISTÊNCIA: Empréstimo com ID inválido: {Id}");
      if (GameId <= 0)
        Logger.LogError($"INCONSISTÊNCIA: Empréstimo {Id} com GameID inválido: {GameId}");
      if (MemberId <= 0)
        Logger.LogError($"INCONSISTÊNCIA: Empréstimo {Id} com MemberID inválido: {MemberId}");
      if (ReturnDate.HasValue && ReturnDate < LoanDate)
        Logger.LogError($"INCONSISTÊNCIA: Empréstimo {Id} tem data de devolução ({ReturnDate:dd/MM/yyyy}) anterior à data de empréstimo ({LoanDate:dd/MM/yyyy})");
      if (LoanDate > DateTime.Now.AddDays(1))
        Logger.LogError($"INCONSISTÊNCIA: Empréstimo {Id} tem data de empréstimo futura: {LoanDate:dd/MM/yyyy}");
    }
  }
}