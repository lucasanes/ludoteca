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
      if (gameId <= 0)
        throw new ArgumentException("ID do jogo inválido.");
      if (memberId <= 0)
        throw new ArgumentException("ID do membro inválido.");

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
    }
  }
}