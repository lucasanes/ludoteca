using System;

namespace Ludoteca
{
  public class ConsistencyService
  {
    public void VerifyDataConsistency(MemberControl memberControl, GameControl gameControl, LoanControl loanControl)
    {
      Logger.LogInfo("Iniciando verificação de consistência dos dados...");
      Console.WriteLine("Verificando consistência dos dados...");

      memberControl.ValidateAllMembers();
      gameControl.ValidateAllGames();
      loanControl.ValidateAllLoans(gameControl.Games, memberControl.Members);

      Console.WriteLine("Verificação de consistência concluída. Verifique debug.log para detalhes.");
      Logger.LogInfo("Verificação de consistência concluída.");
    }
  }
}
