using System;
using System.IO;
using System.Text.Json;

namespace Ludoteca
{
  public class DataService
  {
    private const string JsonFile = "database.json";

    public void Save(MemberControl memberControl, GameControl gameControl, LoanControl loanControl, int nextMemberId, int nextGameId, int nextLoanId)
    {
      try
      {
        string dataDir = PathUtils.GetDataDirectory();
        if (!Directory.Exists(dataDir))
          Directory.CreateDirectory(dataDir);

        object dados = new
        {
          members = memberControl.Members,
          games = gameControl.Games,
          loans = loanControl.Loans,
          nextMemberId = nextMemberId,
          nextGameId = nextGameId,
          nextLoanId = nextLoanId,
        };

        string json = JsonSerializer.Serialize(dados, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(dataDir, JsonFile), json);

        Logger.LogInfo("Dados salvos com sucesso");
      }
      catch (Exception ex)
      {
        Logger.LogError("Erro ao salvar dados", ex);
        throw;
      }
    }

    public (int nextMemberId, int nextGameId, int nextLoanId) Load(MemberControl memberControl, GameControl gameControl, LoanControl loanControl)
    {
      try
      {
        string dataDir = PathUtils.GetDataDirectory();
        string path = Path.Combine(dataDir, JsonFile);
        if (!File.Exists(path))
        {
          Logger.LogInfo("Arquivo de dados não encontrado. Iniciando com dados vazios.");
          return (1, 1, 1);
        }

        string json = File.ReadAllText(path);
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        int nextMemberId = 1;
        int nextGameId = 1;
        int nextLoanId = 1;

        if (root.TryGetProperty("members", out JsonElement membersElement))
        {
          memberControl.Members = JsonSerializer.Deserialize<List<Member>>(membersElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("games", out JsonElement gamesElement))
        {
          gameControl.Games = JsonSerializer.Deserialize<List<Game>>(gamesElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("loans", out JsonElement loansElement))
        {
          loanControl.Loans = JsonSerializer.Deserialize<List<Loan>>(loansElement.GetRawText()) ?? [];
        }

        if (root.TryGetProperty("nextMemberId", out JsonElement memberIdElement))
        {
          nextMemberId = memberIdElement.GetInt32();
        }

        if (root.TryGetProperty("nextGameId", out JsonElement gameIdElement))
        {
          nextGameId = gameIdElement.GetInt32();
        }

        if (root.TryGetProperty("nextLoanId", out JsonElement loanIdElement))
        {
          nextLoanId = loanIdElement.GetInt32();
        }

        Logger.LogInfo($"Dados carregados com sucesso. Membros: {memberControl.Members.Count}, Jogos: {gameControl.Games.Count}, Empréstimos: {loanControl.Loans.Count}");

        return (nextMemberId, nextGameId, nextLoanId);
      }
      catch (Exception ex)
      {
        Logger.LogError("Erro ao carregar dados", ex);
        Console.WriteLine($"Erro ao carregar dados: {ex.Message}");
        Console.WriteLine("Continuando com dados vazios...");
        return (1, 1, 1);
      }
    }
  }
}
