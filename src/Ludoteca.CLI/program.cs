using System;

namespace Ludoteca
{
  class Program
  {
    public int NextMemberId = 1;
    public int NextGameId = 1;
    public int NextLoanId = 1;

    public const string MenuSeparator = "---------------------";

    private MemberControl MemberControl = null!;
    private GameControl GameLibrary = null!;
    private LoanControl LoanControl = null!;
    private DataService DataService = null!;
    private ConsistencyService ConsistencyService = null!;

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
        DataService = new DataService();
        ConsistencyService = new ConsistencyService();

        var (nextMemberId, nextGameId, nextLoanId) = DataService.Load(MemberControl, GameLibrary, LoanControl);
        NextMemberId = nextMemberId;
        NextGameId = nextGameId;
        NextLoanId = nextLoanId;

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
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            try
            {
              Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
              Console.ReadLine();
            }
          }
          catch (InvalidOperationException ex) // [AV1-5]
          {
            HandleException(ex, "Operação inválida", option);
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            try
            {
              Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
              Console.ReadLine();
            }
          }
          catch (Exception ex)
          {
            HandleException(ex, "Erro inesperado", option);
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            try
            {
              Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
              Console.ReadLine();
            }
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
      Console.WriteLine("2 Listar");
      Console.WriteLine("3 Salvar");
      Console.WriteLine("4 Relatórios");
      Console.WriteLine("0 Sair");
      Console.WriteLine(MenuSeparator);
      Console.Write("Opção: ");
    }

    private void DisplayCadastroMenu()
    {
      Console.Clear();
      Console.WriteLine("=== CADASTRO ===");
      Console.WriteLine("1 Cadastrar membro");
      Console.WriteLine("2 Cadastrar jogo");
      Console.WriteLine("3 Cadastrar empréstimo");
      Console.WriteLine("4 Cadastrar devolução");
      Console.WriteLine("0 Voltar ao menu principal");
      Console.WriteLine(MenuSeparator);
      Console.Write("Opção: ");
    }

    private void DisplayListarMenu()
    {
      Console.Clear();
      Console.WriteLine("=== LISTAR ===");
      Console.WriteLine("1 Listar membros");
      Console.WriteLine("2 Listar jogos");
      Console.WriteLine("3 Listar empréstimos");
      Console.WriteLine("0 Voltar ao menu principal");
      Console.WriteLine(MenuSeparator);
      Console.Write("Opção: ");
    }

    private void DisplayRelatoriosMenu()
    {
      Console.Clear();
      Console.WriteLine("=== RELATÓRIOS ===");
      Console.WriteLine("1 Gerar relatório");
      Console.WriteLine("2 Verificar consistência");
      Console.WriteLine("0 Voltar ao menu principal");
      Console.WriteLine(MenuSeparator);
      Console.Write("Opção: ");
    }

    private bool ProcessMenuOption(string option)
    {
      switch (option)
      {
        case "1":
          ProcessCadastroMenu();
          break;
        case "2":
          ProcessListarMenu();
          break;
        case "3":
          DataService.Save(MemberControl, GameLibrary, LoanControl, NextMemberId, NextGameId, NextLoanId);
          Console.WriteLine("Dados salvos com sucesso.");
          Console.WriteLine("Pressione qualquer tecla para continuar...");
          try
          {
            Console.ReadKey();
          }
          catch (InvalidOperationException)
          {
            Console.ReadLine();
          }
          break;
        case "4":
          ProcessRelatoriosMenu();
          break;
        case "0":
          DataService.Save(MemberControl, GameLibrary, LoanControl, NextMemberId, NextGameId, NextLoanId);
          return false;
        default:
          Console.WriteLine("Opção inválida. Tente novamente.");
          Console.WriteLine("Pressione qualquer tecla para continuar...");
          try
          {
            Console.ReadKey();
          }
          catch (InvalidOperationException)
          {
            Console.ReadLine();
          }
          break;
      }
      return true;
    }

    private void ProcessCadastroMenu()
    {
      bool inSubmenu = true;
      while (inSubmenu)
      {
        DisplayCadastroMenu();
        string option = Console.ReadLine()?.Trim() ?? "";

        try
        {
          switch (option)
          {
            case "1":
              MemberControl.AddMember(ref NextMemberId);
              break;
            case "2":
              GameLibrary.AddGame(ref NextGameId);
              break;
            case "3":
              LoanControl.LendGame(MemberControl, GameLibrary.Games, ref NextLoanId);
              break;
            case "4":
              GameLibrary.ReturnGame(LoanControl.Loans);
              break;
            case "0":
              inSubmenu = false;
              break;
            default:
              Console.WriteLine("Opção inválida. Tente novamente.");
              break;
          }
        }
        catch (ArgumentException ex)
        {
          HandleException(ex, "Erro de argumento", option);
        }
        catch (InvalidOperationException ex)
        {
          HandleException(ex, "Operação inválida", option);
        }
        catch (Exception ex)
        {
          HandleException(ex, "Erro inesperado", option);
        }

        if (inSubmenu && option != "0")
        {
          Console.WriteLine("Pressione qualquer tecla para continuar...");
          try
          {
            Console.ReadKey();
          }
          catch (InvalidOperationException)
          {
            Console.ReadLine();
          }
        }
      }
    }

    private void ProcessListarMenu()
    {
      bool inSubmenu = true;
      while (inSubmenu)
      {
        DisplayListarMenu();
        string option = Console.ReadLine()?.Trim() ?? "";

        try
        {
          switch (option)
          {
            case "1":
              MemberControl.ListMembers();
              break;
            case "2":
              GameLibrary.ListGames();
              break;
            case "3":
              LoanControl.ListLoans(MemberControl, GameLibrary.Games);
              break;
            case "0":
              inSubmenu = false;
              break;
            default:
              Console.WriteLine("Opção inválida. Tente novamente.");
              break;
          }
        }
        catch (ArgumentException ex)
        {
          HandleException(ex, "Erro de argumento", option);
        }
        catch (InvalidOperationException ex)
        {
          HandleException(ex, "Operação inválida", option);
        }
        catch (Exception ex)
        {
          HandleException(ex, "Erro inesperado", option);
        }

        if (inSubmenu && option != "0")
        {
          Console.WriteLine("Pressione qualquer tecla para continuar...");
          try
          {
            Console.ReadKey();
          }
          catch (InvalidOperationException)
          {
            Console.ReadLine();
          }
        }
      }
    }

    private void ProcessRelatoriosMenu()
    {
      bool inSubmenu = true;
      while (inSubmenu)
      {
        DisplayRelatoriosMenu();
        string option = Console.ReadLine()?.Trim() ?? "";

        try
        {
          switch (option)
          {
            case "1":
              ReportService.GenerateReport(MemberControl, GameLibrary, LoanControl);
              break;
            case "2":
              ConsistencyService.VerifyDataConsistency(MemberControl, GameLibrary, LoanControl);
              break;
            case "0":
              inSubmenu = false;
              break;
            default:
              Console.WriteLine("Opção inválida. Tente novamente.");
              break;
          }
        }
        catch (ArgumentException ex)
        {
          HandleException(ex, "Erro de argumento", option);
        }
        catch (InvalidOperationException ex)
        {
          HandleException(ex, "Operação inválida", option);
        }
        catch (Exception ex)
        {
          HandleException(ex, "Erro inesperado", option);
        }

        if (inSubmenu && option != "0")
        {
          Console.WriteLine("Pressione qualquer tecla para continuar...");
          try
          {
            Console.ReadKey();
          }
          catch (InvalidOperationException)
          {
            Console.ReadLine();
          }
        }
      }
    }

    private void HandleException(Exception ex, string errorType, string option)
    {
      Console.WriteLine($"{errorType}: {ex.Message}");
      Logger.LogError($"{ex.GetType().Name} na opção {option}: {ex.Message}", ex);
    }

  }
}