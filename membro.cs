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
      if (string.IsNullOrWhiteSpace(Name))
        throw new ArgumentException("Nome do membro não pode ser vazio.");
      if (string.IsNullOrWhiteSpace(Registration))
        throw new ArgumentException("Matrícula não pode ser vazia.");

      this.Id = Id;
      this.Name = Name;
      this.Registration = Registration;
    }
  }
}