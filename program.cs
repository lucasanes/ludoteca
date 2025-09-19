using System;
using System.IO;
using System.Text.Json;

namespace Ludoteca
{
  class Program
  {
    private const string JsonDir = "data";
    private const string JsonFile = "database.json";

    public int NextMemberId = 1;
    public int NextGameId = 1;
    public int NextLoanId = 1;
    
    private MemberControl MemberControl = null!;
    private GameControl GameLibrary = null!;
    private LoanControl LoanControl = null!;

    static void Main(string[] args)
    {
      var program = new Program();
      program.Run();
    }
    
    public void Run()
    {
      try
      {
        Logger.LogInfo("Iniciando aplicação Ludoteca");
        
        MemberControl = new MemberControl();
        GameLibrary = new GameControl();
        LoanControl = new LoanControl();

        Load();

      bool running = true;
      while (running)
      {
        Console.Clear();
        Console.WriteLine("=== LUDOTECA .NET ===");
        Console.WriteLine("1 Cadastro");
        Console.WriteLine("  1.1 Cadastrar membro");
        Console.WriteLine("  1.2 Cadastrar jogo");
        Console.WriteLine("  1.3 Cadastrar empréstimo");
        Console.WriteLine("  1.4 Cadastrar devolução");
        Console.WriteLine("2 Listar");
        Console.WriteLine("  2.1 Listar membros");
        Console.WriteLine("  2.2 Listar jogos");
        Console.WriteLine("  2.3 Listar empréstimos");
        Console.WriteLine("3 Salvar");
        Console.WriteLine("4 Relatórios");
        Console.WriteLine("  4.1 Gerar relatório");
        Console.WriteLine("  4.2 Verificar consistência");
        Console.WriteLine("0 Sair");
        Console.Write("Opção: ");

        string option = Console.ReadLine()?.Trim() ?? "";

        try
        {
          switch (option)
          {
            case "1.1":
              MemberControl.AddMember(ref NextMemberId);
              break;
            case "1.2":
              GameLibrary.AddGame(ref NextGameId);
              break;
            case "1.3":
              LoanControl.LendGame(MemberControl, GameLibrary.Games, ref NextLoanId);
              break;
            case "1.4":
              GameLibrary.ReturnGame(LoanControl.Loans);
              break;
            case "2.1":
              MemberControl.ListMembers();
              break;
            case "2.2":
              GameLibrary.ListGames();
              break;
            case "2.3":
              LoanControl.ListLoans(MemberControl, GameLibrary.Games);
              break;
            case "3":
              Save();
              Console.WriteLine("Dados salvos com sucesso.");
              break;
            case "4.1":
              Logger.GenerateReport(MemberControl, GameLibrary, LoanControl);
              Console.WriteLine("Relatório gerado com sucesso em relatorio.txt");
              break;
            case "4.2":
              Logger.AssertConsistency(MemberControl, GameLibrary, LoanControl);
              Console.WriteLine("Verificação de consistência concluída. Verifique debug.log para detalhes.");
              break;
            case "0":
              Save();
              running = false;
              break;
            default:
              Console.WriteLine("Opção inválida. Tente novamente.");
              break;
          }
        }

        catch (ArgumentException ex)
        {
          Console.WriteLine($"Erro de argumento: {ex.Message}");
          Logger.LogError($"ArgumentException na opção {option}: {ex.Message}", ex);
        }
        catch (InvalidOperationException ex)
        {
          Console.WriteLine($"Operação inválida: {ex.Message}");
          Logger.LogError($"InvalidOperationException na opção {option}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Erro inesperado: {ex.Message}");
          Logger.LogError($"Exception inesperada na opção {option}: {ex.Message}", ex);
        }

        if (running)
        {
          Console.WriteLine("Pressione qualquer tecla para continuar...");
          Console.ReadKey();
        }
      }
      
      Logger.LogInfo("Encerrando aplicação Ludoteca");
      }
      catch (Exception ex)
      {
        Logger.LogError("Erro crítico na aplicação", ex);
        Console.WriteLine($"Erro crítico: {ex.Message}");
        Console.WriteLine("Verifique debug.log para mais detalhes.");
      }
    }

    private void Save()
    {
      try
      {
        if (!Directory.Exists(JsonDir))
          Directory.CreateDirectory(JsonDir);

        var dados = new
        {
          members = MemberControl.Members,
          games = GameLibrary.Games,
          loans = LoanControl.Loans,
          nextMemberId = this.NextMemberId,
          nextGameId = this.NextGameId,
          nextLoanId = this.NextLoanId,
        };

        string json = JsonSerializer.Serialize(dados, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(JsonDir, JsonFile), json);
        
        Logger.LogInfo("Dados salvos com sucesso");
      }
      catch (Exception ex)
      {
        Logger.LogError("Erro ao salvar dados", ex);
        throw;
      }
    }

    private void Load()
    {
      try
      {
        string path = Path.Combine(JsonDir, JsonFile);
        if (!File.Exists(path))
        {
          Logger.LogInfo("Arquivo de dados não encontrado. Iniciando com dados vazios.");
          return;
        }

        string json = File.ReadAllText(path);
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        if (root.TryGetProperty("members", out JsonElement membersElement))
        {
          MemberControl.Members = JsonSerializer.Deserialize<List<Member>>(membersElement.GetRawText()) ?? [];
        }
        
        if (root.TryGetProperty("games", out JsonElement gamesElement))
        {
          GameLibrary.Games = JsonSerializer.Deserialize<List<Game>>(gamesElement.GetRawText()) ?? [];
        }
        
        if (root.TryGetProperty("loans", out JsonElement loansElement))
        {
          LoanControl.Loans = JsonSerializer.Deserialize<List<Loan>>(loansElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("nextMemberId", out JsonElement memberIdElement))
        {
          this.NextMemberId = memberIdElement.GetInt32();
        }
        
        if (root.TryGetProperty("nextGameId", out JsonElement gameIdElement))
        {
          this.NextGameId = gameIdElement.GetInt32();
        }
        
        if (root.TryGetProperty("nextLoanId", out JsonElement loanIdElement))
        {
          this.NextLoanId = loanIdElement.GetInt32();
        }
        
        Logger.LogInfo($"Dados carregados com sucesso. Membros: {MemberControl.Members.Count}, Jogos: {GameLibrary.Games.Count}, Empréstimos: {LoanControl.Loans.Count}");
      }
      catch (Exception ex)
      {
        Logger.LogError("Erro ao carregar dados", ex);
        Console.WriteLine($"Erro ao carregar dados: {ex.Message}");
        Console.WriteLine("Continuando com dados vazios...");
      }
    }

  }
}