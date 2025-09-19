using System;

namespace Ludoteca
{
  public class Game
  {
    public int Id { get; private set; }
    public string Name { get; private set; }
    public bool Available { get; private set; } = true;

    public Game(int id, string name)
    {
      if (id <= 0)
        throw new ArgumentException("ID do jogo deve ser maior que zero.");
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Nome do jogo não pode ser vazio.");
      if (name.Length > 100)
        throw new ArgumentException("Nome do jogo não pode exceder 100 caracteres.");

      Id = id;
      Name = name.Trim();
      
    }

    public void MarkAsLoan()
    {
      if (!Available)
        throw new InvalidOperationException("Jogo já está emprestado.");
      Available = false;
      Logger.LogInfo($"Jogo {Id} marcado como emprestado");
    }

    public void MarkAsAvailable()
    {
      Available = true;
      Logger.LogInfo($"Jogo {Id} marcado como disponível");
    }
    
    // Método para validar consistência interna
    public void ValidateConsistency()
    {
      if (Id <= 0)
        Logger.LogError($"INCONSISTÊNCIA: Jogo com ID inválido: {Id}");
      if (string.IsNullOrWhiteSpace(Name))
        Logger.LogError($"INCONSISTÊNCIA: Jogo {Id} com nome vazio ou nulo");
    }
  }
}