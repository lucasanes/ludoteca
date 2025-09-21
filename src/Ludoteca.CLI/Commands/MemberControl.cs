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

      PrintMembersTableHeader();
      PrintMembersTableContent();
      PrintMembersTableFooter();
    }

    private void PrintMembersTableHeader()
    {
      Console.WriteLine();
      Console.WriteLine("┌─────┬─────────────────────────────────────┬─────────────────────┐");
      Console.WriteLine("│ ID  │               Nome                  │      Matrícula      │");
      Console.WriteLine("├─────┼─────────────────────────────────────┼─────────────────────┤");
    }

    private void PrintMembersTableContent()
    {
      foreach (Member member in Members)
      {
        string name = member.Name.Length > 35 ? member.Name.Substring(0, 32) + "..." : member.Name;
        string registration = member.Registration.Length > 19 ? member.Registration.Substring(0, 16) + "..." : member.Registration;
        Console.WriteLine($"│ {member.Id,-3} │ {name,-35} │ {registration,-19} │");
      }
    }

    private void PrintMembersTableFooter()
    {
      Console.WriteLine("└─────┴─────────────────────────────────────┴─────────────────────┘");
      Console.WriteLine();
    }

    public void AddMember(ref int nextMemberId)
    {
      try // [AV1-5]
      {
        Console.Write("Nome do membro: ");
        string name = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Matrícula: ");
        string registration = Console.ReadLine()?.Trim() ?? "";

        if (Members.Any(m => m.Registration.Equals(registration, StringComparison.OrdinalIgnoreCase)))
        {
          throw new ArgumentException("Já existe um membro com esta matrícula.");
        }

        Member member = new Member(nextMemberId++, name, registration);
        Members.Add(member);

        Logger.LogInfo($"Membro cadastrado: ID={member.Id}, Nome={member.Name}, Matrícula={member.Registration}");
        Console.WriteLine("Membro cadastrado com sucesso.");
      }
      catch (Exception ex) // [AV1-5]
      {
        Logger.LogError("Erro ao cadastrar membro", ex);
        throw;
      }
    }
    
    public void ValidateAllMembers()
    {
      Logger.LogInfo("Iniciando validação de consistência dos membros...");
      
      foreach (Member member in Members)
      {
        member.ValidateConsistency();
      }
      
      List<IGrouping<string, Member>> duplicateRegistrations = [.. Members
        .GroupBy(m => m.Registration.ToLower())
        .Where(g => g.Count() > 1)];
        
      foreach (IGrouping<string, Member> duplicate in duplicateRegistrations)
      {
        Logger.LogError($"INCONSISTÊNCIA: Matrícula duplicada encontrada: {duplicate.Key}");
        foreach (Member member in duplicate)
        {
          Logger.LogError($"  - Membro ID {member.Id}: {member.Name}");
        }
      }
      
      Logger.LogInfo($"Validação de membros concluída. Total de membros: {Members.Count}");
    }
  }
}