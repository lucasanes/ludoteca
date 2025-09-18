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
    private GameLibrary GameLibrary = null!;
    private LoanControl LoanControl = null!;

    static void Main(string[] args)
    {
      var program = new Program();
      program.Run();
    }
    
    public void Run()
    {
      MemberControl = new MemberControl();
      GameLibrary = new GameLibrary();
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
        }
        catch (InvalidOperationException ex)
        {
          Console.WriteLine($"Operação inválida: {ex.Message}");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Erro inesperado: {ex.Message}");
        }

        if (running)
        {
          Console.WriteLine("Pressione qualquer tecla para continuar...");
          Console.ReadKey();
        }
      }
    }

    private void Save()
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
    }

    private void Load()
    {
      string path = Path.Combine(JsonDir, JsonFile);
      if (!File.Exists(path))
        return;

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
    }

  }
}