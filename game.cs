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
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Nome do jogo não pode ser vazio.");

      Id = id;
      Name = name;
    }

    public void MarkAsLoan()
    {
      if (!Available)
        throw new InvalidOperationException("Jogo já está emprestado.");
      Available = false;
    }

    public void MarkAsAvailable()
    {
      Available = true;
    }
  }
}