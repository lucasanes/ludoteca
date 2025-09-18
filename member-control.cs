using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Ludoteca
{
  public class MemberControl
  {
    public List<Member> Members { get; set; } = [];

    public void ListMembers()
    {
      if (Members.Count == 0)
      {
        Console.WriteLine("Nenhum membro cadastrado.");
        return;
      }

      Console.WriteLine();
      Console.WriteLine("┌─────┬─────────────────────────────────────┬─────────────────────┐");
      Console.WriteLine("│ ID  │               Nome                  │      Matrícula      │");
      Console.WriteLine("├─────┼─────────────────────────────────────┼─────────────────────┤");

      foreach (var member in Members)
      {
        string name = member.Name.Length > 35 ? member.Name.Substring(0, 32) + "..." : member.Name;
        string registration = member.Registration.Length > 19 ? member.Registration.Substring(0, 16) + "..." : member.Registration;
        Console.WriteLine($"│ {member.Id,-3} │ {name,-35} │ {registration,-19} │");
      }

      Console.WriteLine("└─────┴─────────────────────────────────────┴─────────────────────┘");
      Console.WriteLine();
    }

    public void AddMember(ref int nextMemberId)
    {
      Console.Write("Nome do membro: ");
      string name = Console.ReadLine()?.Trim() ?? "";
      Console.Write("Matrícula: ");
      string registration = Console.ReadLine()?.Trim() ?? "";

      var member = new Member(nextMemberId++, name, registration);
      Members.Add(member);

      Console.WriteLine("Membro cadastrado com sucesso.");
    }
  }
}