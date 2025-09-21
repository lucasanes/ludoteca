using System;

namespace Ludoteca
{
  public class Member
  {
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Registration { get; private set; }

    public Member(int Id, string Name, string Registration)
    {
      if (Id <= 0)
        throw new ArgumentException("ID do membro deve ser maior que zero.");
      if (string.IsNullOrWhiteSpace(Name))
        throw new ArgumentException("Nome do membro não pode ser vazio.");
      if (string.IsNullOrWhiteSpace(Registration))
        throw new ArgumentException("Matrícula não pode ser vazia.");
      if (Name.Length > 100)
        throw new ArgumentException("Nome do membro não pode exceder 100 caracteres.");
      if (Registration.Length > 50)
        throw new ArgumentException("Matrícula não pode exceder 50 caracteres.");

      this.Id = Id;
      this.Name = Name.Trim();
      this.Registration = Registration.Trim();
      
    }
    
    public void ValidateConsistency()
    {
      if (Id <= 0)
        Logger.LogError($"INCONSISTÊNCIA: Membro com ID inválido: {Id}");
      if (string.IsNullOrWhiteSpace(Name))
        Logger.LogError($"INCONSISTÊNCIA: Membro {Id} com nome vazio ou nulo");
      if (string.IsNullOrWhiteSpace(Registration))
        Logger.LogError($"INCONSISTÊNCIA: Membro {Id} com matrícula vazia ou nula");
    }
  }
}