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
      Program program = new();
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
          DisplayMenu();
          string option = Console.ReadLine()?.Trim() ?? "";

          try
          {
            running = ProcessMenuOption(option);
          }
          catch (ArgumentException ex) // [AV1-5]
          {
            HandleException(ex, "Erro de argumento", option);
          }
          catch (InvalidOperationException ex) // [AV1-5]
          {
            HandleException(ex, "Operação inválida", option);
          }
          catch (Exception ex)
          {
            HandleException(ex, "Erro inesperado", option);
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

    private void DisplayMenu()
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
    }

    private bool ProcessMenuOption(string option)
    {
      switch (option)
      {
        case "1.1": // [AV1-4-CadastrarMembro]
          MemberControl.AddMember(ref NextMemberId);
          break;
        case "1.2": // [AV1-4-CadastrarJogo]
          GameLibrary.AddGame(ref NextGameId);
          break;
        case "1.3": // [AV1-4-CadastrarEmprestimo]
          LoanControl.LendGame(MemberControl, GameLibrary.Games, ref NextLoanId);
          break;
        case "1.4": // [AV1-4-CadastrarDevolucao]
          GameLibrary.ReturnGame(LoanControl.Loans);
          break;
        case "2.1": // [AV1-4-ListarMembros]
          MemberControl.ListMembers();
          break;
        case "2.2": // [AV1-4-ListarJogos]
          GameLibrary.ListGames();
          break;
        case "2.3": // [AV1-4-ListarEmprestimos]
          LoanControl.ListLoans(MemberControl, GameLibrary.Games);
          break;
        case "3": // [AV1-4-Salvar]
          Save();
          Console.WriteLine("Dados salvos com sucesso.");
          break;
        case "4.1": // [AV1-4-GerarRelatorio]
          Logger.GenerateReport(MemberControl, GameLibrary, LoanControl);
          Console.WriteLine("Relatório gerado com sucesso em relatorio.txt");
          break;
        case "4.2": // [AV1-4-VerificarConsistencia]
          VerifyDataConsistency();
          break;
        case "0": // [AV1-4-Sair]
          Save();
          return false;
        default:
          Console.WriteLine("Opção inválida. Tente novamente.");
          break;
      }
      return true;
    }

    private void HandleException(Exception ex, string errorType, string option)
    {
      Console.WriteLine($"{errorType}: {ex.Message}");
      Logger.LogError($"{ex.GetType().Name} na opção {option}: {ex.Message}", ex);
    }

    private void VerifyDataConsistency()
    {
      Logger.LogInfo("Iniciando verificação de consistência dos dados...");
      Console.WriteLine("Verificando consistência dos dados...");
      
      MemberControl.ValidateAllMembers();
      GameLibrary.ValidateAllGames();
      LoanControl.ValidateAllLoans(GameLibrary.Games, MemberControl.Members);
      
      Console.WriteLine("Verificação de consistência concluída. Verifique debug.log para detalhes.");
      Logger.LogInfo("Verificação de consistência concluída.");
    }

    private void Save()
    {
      try
      {
        if (!Directory.Exists(JsonDir))
          Directory.CreateDirectory(JsonDir);

        object dados = new
        {
          members = MemberControl.Members,
          games = GameLibrary.Games,
          loans = LoanControl.Loans,
          nextMemberId = this.NextMemberId,
          nextGameId = this.NextGameId,
          nextLoanId = this.NextLoanId,
        };

        string json = JsonSerializer.Serialize(dados, new JsonSerializerOptions { WriteIndented = true }); // [AV1-3]
        File.WriteAllText(Path.Combine(JsonDir, JsonFile), json); // [AV1-3]
        
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

        string json = File.ReadAllText(path); // [AV1-3]
        using JsonDocument document = JsonDocument.Parse(json); // [AV1-3]
        JsonElement root = document.RootElement; // [AV1-3]

        if (root.TryGetProperty("members", out JsonElement membersElement)) // [AV1-3]
        {
          MemberControl.Members = JsonSerializer.Deserialize<List<Member>>(membersElement.GetRawText()) ?? []; // [AV1-3]
        }
        
        if (root.TryGetProperty("games", out JsonElement gamesElement)) // [AV1-3]
        {
          GameLibrary.Games = JsonSerializer.Deserialize<List<Game>>(gamesElement.GetRawText()) ?? []; // [AV1-3]
        }
        
        if (root.TryGetProperty("loans", out JsonElement loansElement)) // [AV1-3]
        {
          LoanControl.Loans = JsonSerializer.Deserialize<List<Loan>>(loansElement.GetRawText()) ?? []; // [AV1-3]
        }

        if (root.TryGetProperty("nextMemberId", out JsonElement memberIdElement)) // [AV1-3]
        {
          this.NextMemberId = memberIdElement.GetInt32(); // [AV1-3]
        }
        
        if (root.TryGetProperty("nextGameId", out JsonElement gameIdElement)) // [AV1-3]
        {
          this.NextGameId = gameIdElement.GetInt32(); // [AV1-3]
        }
        
        if (root.TryGetProperty("nextLoanId", out JsonElement loanIdElement)) // [AV1-3]
        {
          this.NextLoanId = loanIdElement.GetInt32(); // [AV1-3]
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