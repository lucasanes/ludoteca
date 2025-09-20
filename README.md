# Ludoteca - Sistema de Gerenciamento de Jogos

## Descrição do Projeto
Sistema de gerenciamento para ludotecas desenvolvido em .NET 9.0, permitindo o controle de membros, jogos e empréstimos com persistência de dados em JSON e relatórios detalhados.

## Estrutura do Projeto

### Classes Principais

#### 1. Member (Membro) - `member.cs`
**Propósito**: Representa um membro da ludoteca com informações pessoais e validações.

**Construtor**: Linha 11
```csharp
public Member(int Id, string Name, string Registration)
```

**Propriedades com Validação**:
- `Id` (Linha 7): Propriedade privada setter, validação > 0 (Linha 13)
- `Name` (Linha 8): Propriedade privada setter, validação não vazio e <= 100 chars (Linhas 15, 19)  
- `Registration` (Linha 9): Propriedade privada setter, validação não vazio e <= 50 chars (Linhas 17, 21)

**Validações no Construtor**:
- ID deve ser maior que zero (Linha 13)
- Nome não pode ser vazio (Linha 15)  
- Matrícula não pode ser vazia (Linha 17)
- Nome não pode exceder 100 caracteres (Linha 19)
- Matrícula não pode exceder 50 caracteres (Linha 21)

#### 2. Game (Jogo) - `game.cs`
**Propósito**: Representa um jogo disponível na ludoteca com controle de disponibilidade.

**Construtor**: Linha 11
```csharp
public Game(int id, string name)
```

**Propriedades com Validação**:
- `Id` (Linha 7): Propriedade privada setter, validação > 0 (Linha 13)
- `Name` (Linha 8): Propriedade privada setter, validação não vazio e <= 100 chars (Linhas 15, 17)
- `Available` (Linha 9): Propriedade privada setter, padrão true

**Validações no Construtor**:
- ID deve ser maior que zero (Linha 13)
- Nome não pode ser vazio (Linha 15)
- Nome não pode exceder 100 caracteres (Linha 17)

#### 3. Loan (Empréstimo) - `loan.cs`
**Propósito**: Representa um empréstimo de jogo com controle de datas e devoluções.

**Construtor**: Linha 14
```csharp
public Loan(int id, int gameId, int memberId, DateTime? returnDate = null)
```

**Propriedades com Validação**:
- `Id` (Linha 7): Propriedade privada setter, validação > 0 (Linha 16)
- `GameId` (Linha 8): Propriedade privada setter, validação > 0 (Linha 18)
- `MemberId` (Linha 9): Propriedade privada setter, validação > 0 (Linha 20)
- `LoanDate` (Linha 10): Propriedade privada setter, definido automaticamente
- `DueDate` (Linha 11): Propriedade calculada (LoanDate + 7 dias)
- `ReturnDate` (Linha 12): Propriedade privada setter, validação de data antiga (Linha 22)

**Validações no Construtor**:
- ID deve ser maior que zero (Linha 16)
- GameID deve ser maior que zero (Linha 18)
- MemberID deve ser maior que zero (Linha 20)
- Data de devolução não pode ser muito antiga (Linha 22)

#### 4. Classes de Controle

##### MemberControl (Controle de Membros) - `member-control.cs`
**Propósito**: Gerencia operações CRUD e validações para membros.
- **Propriedades**: `Members` (Lista de membros) - Linha 11
- **Métodos principais**: AddMember, ListMembers, ValidateAllMembers

##### GameControl (Controle de Jogos) - `game-control.cs` 
**Propósito**: Gerencia operações CRUD e validações para jogos.
- **Propriedades**: `Games` (Lista de jogos) - Linha 10
- **Métodos principais**: AddGame, ListGames, ReturnGame, ValidateAllGames

##### LoanControl (Controle de Empréstimos) - `loan-control.cs`
**Propósito**: Gerencia operações CRUD e validações para empréstimos.
- **Propriedades**: `Loans` (Lista de empréstimos) - Linha 11  
- **Métodos principais**: LendGame, ListLoans, ValidateAllLoans

## Funcionalidades

### Serialização JSON (System.Text.Json)
- **Método Salvar()**: Linhas 135-154 em `program.cs` - Marcadas com `// [AV1-3]`
- **Método Carregar()**: Linhas 172-217 em `program.cs` - Marcadas com `// [AV1-3]`

### Menu do Console
Menu interativo marcado com comentários `// [AV1-4-*]` (Linhas 63-95 em `program.cs`):
- **1.1 Cadastrar Membro** - `// [AV1-4-CadastrarMembro]`
- **1.2 Cadastrar Jogo** - `// [AV1-4-CadastrarJogo]`  
- **1.3 Cadastrar Empréstimo** - `// [AV1-4-CadastrarEmprestimo]`
- **1.4 Cadastrar Devolução** - `// [AV1-4-CadastrarDevolucao]`
- **2.1 Listar Membros** - `// [AV1-4-ListarMembros]`
- **2.2 Listar Jogos** - `// [AV1-4-ListarJogos]`
- **2.3 Listar Empréstimos** - `// [AV1-4-ListarEmprestimos]`
- **3 Salvar** - `// [AV1-4-Salvar]`
- **4.1 Gerar Relatório** - `// [AV1-4-GerarRelatorio]`
- **4.2 Verificar Consistência** - `// [AV1-4-VerificarConsistencia]`
- **0 Sair** - `// [AV1-4-Sair]`

### Tratamento de Exceções
Blocos try/catch marcados com `// [AV1-5]`:
- **ArgumentException** e **InvalidOperationException** tratados em:
  - `program.cs` linhas 108-118
  - `member-control.cs` linhas 39-60  
  - `game-control.cs` linhas 18-32 e 88-110
  - `loan-control.cs` linhas 48-72

## Como Executar

1. **Compilar o projeto**:
   ```bash
   dotnet build
   ```

2. **Executar a aplicação**:
   ```bash
   dotnet run
   ```

3. **Dados persistidos**: Os dados são automaticamente salvos em `data/database.json`

4. **Logs e relatórios**: 
   - Logs: `data/debug.log`
   - Relatórios: `data/relatorio.txt`

## Arquivos de Dados
- **database.json**: Persistência dos dados do sistema
- **debug.log**: Log detalhado das operações
- **relatorio.txt**: Relatórios gerenciais gerados

## Tecnologias Utilizadas
- **.NET 9.0**
- **System.Text.Json** para serialização
- **LINQ** para consultas
- **Console Application** para interface do usuário

## Validações Implementadas
- **Encapsulamento**: Todas as propriedades possuem setters privados
- **Validações de entrada**: Verificação de valores nulos, vazios e limites de tamanho
- **Regras de negócio**: Controle de disponibilidade de jogos e datas de empréstimo
- **Consistência de dados**: Verificação de integridade referencial entre entidades