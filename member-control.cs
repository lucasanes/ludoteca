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
      try
      {
        Console.Write("Nome do membro: ");
        string name = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Matrícula: ");
        string registration = Console.ReadLine()?.Trim() ?? "";

        if (Members.Any(m => m.Registration.Equals(registration, StringComparison.OrdinalIgnoreCase)))
        {
          throw new ArgumentException("Já existe um membro com esta matrícula.");
        }

        var member = new Member(nextMemberId++, name, registration);
        Members.Add(member);

        Logger.LogInfo($"Membro cadastrado: ID={member.Id}, Nome={member.Name}, Matrícula={member.Registration}");
        Console.WriteLine("Membro cadastrado com sucesso.");
      }
      catch (Exception ex)
      {
        Logger.LogError("Erro ao cadastrar membro", ex);
        throw;
      }
    }
    
    public void ValidateAllMembers()
    {
      Logger.LogInfo("Iniciando validação de consistência dos membros...");
      
      foreach (var member in Members)
      {
        member.ValidateConsistency();
      }
      
      var duplicateRegistrations = Members
        .GroupBy(m => m.Registration.ToLower())
        .Where(g => g.Count() > 1)
        .ToList();
        
      foreach (var duplicate in duplicateRegistrations)
      {
        Logger.LogError($"INCONSISTÊNCIA: Matrícula duplicada encontrada: {duplicate.Key}");
        foreach (var member in duplicate)
        {
          Logger.LogError($"  - Membro ID {member.Id}: {member.Name}");
        }
      }
      
      Logger.LogInfo($"Validação de membros concluída. Total de membros: {Members.Count}");
    }
  }
}