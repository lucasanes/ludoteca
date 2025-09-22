# Ludoteca - Sistema de Gerenciamento de Jogos

## Participantes

1. Lucas Abrahão Anes - 06009881
2. Lucas Gomes Coco - 06011471
3. Murilo de Melo Mouteira - 06010561

## Descrição do Projeto
Sistema de gerenciamento para ludotecas desenvolvido em .NET 9.0, permitindo o controle de membros, jogos e empréstimos com persistência de dados em JSON e relatórios detalhados. O projeto utiliza uma arquitetura limpa com separação de responsabilidades entre camadas.

## Estrutura do Projeto

### Arquitetura em Camadas

O projeto está organizado em três projetos principais seguindo os princípios de Clean Architecture:

#### **Ludoteca.Core** - Camada de Domínio
Contém as entidades principais do sistema localizadas em `src/Ludoteca.Core/Entities/`:

##### 1. Member (Membro) - `Member.cs`
**Propósito**: Representa um membro da ludoteca com informações pessoais e validações.

**Construtor**: Linha 11
```csharp
public Member(int Id, string Name, string Registration)
```

**Propriedades com Validação**:
- `Id` (Linha 7): Propriedade com setter privado, validação > 0 (Linha 13)
- `Name` (Linha 8): Propriedade com setter privado, validação não vazio e <= 100 chars (Linhas 15, 19)  
- `Registration` (Linha 9): Propriedade com setter privado, validação não vazio e <= 50 chars (Linhas 17, 21)

**Validações no Construtor**:
- ID deve ser maior que zero (Linha 13)
- Nome não pode ser vazio (Linha 15)  
- Matrícula não pode ser vazia (Linha 17)
- Nome não pode exceder 100 caracteres (Linha 19)
- Matrícula não pode exceder 50 caracteres (Linha 21)

**Método de Validação**: `ValidateConsistency()` para verificação de integridade dos dados

##### 2. Game (Jogo) - `Game.cs`
**Propósito**: Representa um jogo disponível na ludoteca com controle de disponibilidade.

**Construtor**: Linha 11
```csharp
public Game(int id, string name)
```

**Propriedades com Validação**:
- `Id` (Linha 7): Propriedade com setter privado, validação > 0 (Linha 13)
- `Name` (Linha 8): Propriedade com setter privado, validação não vazio e <= 100 chars (Linhas 15, 17)
- `Available` (Linha 9): Propriedade com setter privado, padrão true

**Validações no Construtor**:
- ID deve ser maior que zero (Linha 13)
- Nome não pode ser vazio (Linha 15)
- Nome não pode exceder 100 caracteres (Linha 17)

**Método de Validação**: `ValidateConsistency()` para verificação de integridade dos dados

##### 3. Loan (Empréstimo) - `Loan.cs`
**Propósito**: Representa um empréstimo de jogo com controle de datas e devoluções.

**Construtor**: Linha 14
```csharp
public Loan(int id, int gameId, int memberId, DateTime? returnDate = null)
```

**Propriedades com Validação**:
- `Id` (Linha 7): Propriedade com setter privado, validação > 0 (Linha 16)
- `GameId` (Linha 8): Propriedade com setter privado, validação > 0 (Linha 18)
- `MemberId` (Linha 9): Propriedade com setter privado, validação > 0 (Linha 20)
- `LoanDate` (Linha 10): Propriedade com setter privado, definido automaticamente
- `DueDate` (Linha 11): Propriedade calculada (LoanDate + 7 dias)
- `ReturnDate` (Linha 12): Propriedade com setter privado, validação de data antiga (Linha 22)

**Validações no Construtor**:
- ID deve ser maior que zero (Linha 16)
- GameID deve ser maior que zero (Linha 18)
- MemberID deve ser maior que zero (Linha 20)
- Data de devolução não pode ser muito antiga (Linha 22)

**Métodos**: `MarkAsReturned()` para registrar devolução e `ValidateConsistency()` para verificação

#### **Ludoteca.Infrastructure** - Camada de Infraestrutura
Contém serviços de infraestrutura, logging e utilitários:

- **Logger** (`src/Ludoteca.Infrastructure/Logging/Logger.cs`): Sistema de logging unificado
  - `LogInfo(string message)`: Registra informações
  - `LogError(string message, Exception? ex = null)`: Registra erros com exceções opcionais
  - **Localização**: Logs salvos automaticamente em `data/debug.log` no diretório raiz do projeto

- **PathUtils** (`src/Ludoteca.Infrastructure/PathUtils.cs`): Utilitário para resolução de caminhos
  - `GetProjectRoot()`: Localiza dinamicamente o diretório raiz do projeto (busca por ludoteca.sln)
  - `GetDataDirectory()`: Retorna caminho absoluto para o diretório de dados do projeto
  - **Propósito**: Garante que dados sejam salvos no local correto independente do diretório de execução

#### **Ludoteca.CLI** - Camada de Apresentação
Contém a aplicação console e controladores:

##### Classes de Controle (`src/Ludoteca.CLI/Controller/`)

**MemberControl** - `MemberControl.cs`
**Propósito**: Gerencia operações CRUD e validações para membros.
- **Propriedades**: `Members` (Lista de membros)
- **Métodos principais**: `AddMember`, `ListMembers`, `ValidateAllMembers`

**GameControl** - `GameControl.cs` 
**Propósito**: Gerencia operações CRUD e validações para jogos.
- **Propriedades**: `Games` (Lista de jogos)
- **Métodos principais**: `AddGame`, `ListGames`, `ReturnGame`, `ValidateAllGames`

**LoanControl** - `LoanControl.cs`
**Propósito**: Gerencia operações CRUD e validações para empréstimos.
- **Propriedades**: `Loans` (Lista de empréstimos)
- **Métodos principais**: `LendGame`, `ListLoans`, `ValidateAllLoans`

##### Classes de Serviço (`src/Ludoteca.CLI/Services/`)

**DataService** - `DataService.cs`
**Propósito**: Responsável pela persistência de dados em JSON com localização automática.
- **Métodos**:
  - `Save()`: Salva todos os dados no arquivo JSON
  - `Load()`: Carrega dados do arquivo JSON e retorna próximos IDs
- **Características**:
  - **Localização dinâmica**: Dados salvos automaticamente no diretório `data/` do projeto raiz
  - **Detecção automática de diretório**: Utiliza PathUtils para encontrar a localização correta

**ConsistencyService** - `ConsistencyService.cs`
**Propósito**: Responsável pela verificação de consistência dos dados.
- **Métodos**:
  - `VerifyDataConsistency()`: Executa validações em todas as entidades

**ReportService** - `ReportService.cs`
**Propósito**: Geração de relatórios gerenciais com versionamento automático.
- **Métodos**:
  - `GenerateReport()`: Gera relatório completo do sistema com timestamp único
- **Características**:
  - **Versionamento automático**: Cada relatório é salvo com timestamp (formato: `relatorio_YYYY-MM-DD_HH-mm-ss.txt`)
  - **Preservação histórica**: Relatórios anteriores não são sobrescritos
  - **Localização dinâmica**: Salvos automaticamente no diretório `data/` do projeto

## Funcionalidades

### Sistema de Menu Hierárquico
O sistema possui um menu principal com submenus organizados:

#### **Menu Principal**:
- **1 Cadastro** - Abre submenu de cadastros
- **2 Listar** - Abre submenu de listagens  
- **3 Salvar** - Salva dados imediatamente
- **4 Relatórios** - Abre submenu de relatórios
- **0 Sair** - Encerra aplicação

### Funcionalidades Avançadas de Gerenciamento de Dados

#### **Versionamento Automático de Relatórios**
- **Timestamping**: Cada relatório é salvo com timestamp único (formato: `relatorio_2025-09-21_14-30-25.txt`)
- **Preservação histórica**: Relatórios anteriores nunca são sobrescritos
- **Rastreabilidade**: Fácil identificação de quando cada relatório foi gerado

#### **Localização Dinâmica de Dados**
- **Detecção automática**: Sistema encontra automaticamente o diretório raiz do projeto
- **Independência de execução**: Dados sempre salvos no local correto, independente de onde a aplicação é executada
- **Estrutura consistente**: Todos os dados centralizados no diretório `data/` do projeto raiz

#### **Submenu Cadastro**:
- **1 Cadastrar membro**
- **2 Cadastrar jogo**
- **3 Cadastrar empréstimo**
- **4 Cadastrar devolução**
- **0 Voltar ao menu principal**

#### **Submenu Listar**:
- **1 Listar membros**
- **2 Listar jogos**
- **3 Listar empréstimos**
- **0 Voltar ao menu principal**

#### **Submenu Relatórios**:
- **1 Gerar relatório**
- **2 Verificar consistência**
- **0 Voltar ao menu principal**

### Navegação Otimizada
- **Navegação instantânea**: Voltar aos menus superiores não requer confirmação
- **Feedback claro**: Cada submenu possui título identificativo
- **Consistência**: Opção "0" sempre retorna ao nível anterior

### Persistência de Dados (System.Text.Json)
**DataService** gerencia toda a serialização:
- **Salvamento automático**: Dados são salvos ao sair da aplicação
- **Carregamento na inicialização**: Dados são restaurados automaticamente
- **Tratamento de erros**: Falhas na persistência são logadas apropriadamente

### Sistema de Logging Unificado
**Logger centralizado** com dois níveis:
- **LogInfo**: Informações operacionais e de fluxo
- **LogError**: Erros e exceções com stack trace completo

### Tratamento de Exceções Robusto
Sistema de tratamento de exceções em múltiplas camadas:
- **Exceções específicas**: `ArgumentException` e `InvalidOperationException`
- **Tratamento global**: Captura de exceções não previstas no loop principal
- **Logging detalhado**: Todas as exceções são registradas com contexto completo
- **Experiência do usuário**: Mensagens claras sem interromper o fluxo da aplicação

## Melhorias Implementadas

### Arquitetura Limpa
- **Separação de responsabilidades**: Cada classe tem uma responsabilidade única
- **Desacoplamento**: Serviços independentes e reutilizáveis
- **Manutenibilidade**: Código mais fácil de testar e modificar

### Organização do Código
- **Redução de linhas**: `program.cs` reduzido de ~469 para 375 linhas
- **Modularização**: Lógica de negócio separada em serviços específicos
- **Estrutura clara**: Hierarquia de pastas seguindo padrões de projeto

### Experiência do Usuário
- **Navegação intuitiva**: Menus hierárquicos com retorno instantâneo
- **Feedback visual**: Separadores e títulos claros em cada tela
- **Operação fluida**: Sem confirmações desnecessárias na navegação

## Como Executar

1. **Pré-requisitos**:
   - .NET 9.0 SDK instalado
   - Terminal/Console compatível

2. **Compilar e executar o projeto**:
  ```bash
  # Navegue até o diretório do projeto CLI
  cd src/Ludoteca.CLI
  
  # Compile e execute o projeto
  dotnet build && dotnet run
  ```
  
  Alternativamente, você pode executar diretamente da raiz do projeto:

3. **Executar a aplicação**:
   ```bash
   dotnet run --project src/Ludoteca.CLI
   ```

4. **Navegação**:
   - Use números para selecionar opções
   - Use "0" para voltar ao menu anterior ou sair
   - Dados são salvos automaticamente ao encerrar

## Estrutura de Arquivos

### Dados e Logs
- **`data/database.json`**: Persistência de todos os dados do sistema
- **`data/debug.log`**: Log detalhado de operações e erros
- **`data/relatorio_YYYY-MM-DD_HH-mm-ss.txt`**: Relatórios gerenciais versionados com timestamp
- **Localização automática**: Todos os arquivos de dados são criados automaticamente no diretório `data/` do projeto raiz, independente de onde a aplicação é executada

### Projetos
- **`src/Ludoteca.Core/`**: Entidades de domínio (Member, Game, Loan)
- **`src/Ludoteca.Infrastructure/`**: Serviços de infraestrutura (Logger, PathUtils)
- **`src/Ludoteca.CLI/`**: Interface de usuário e orquestração
  - **`Controller/`**: Controladores de negócio
  - **`Services/`**: Serviços de aplicação

## Tecnologias e Padrões Utilizados

### Tecnologias
- **.NET 9.0**: Framework base
- **System.Text.Json**: Serialização de dados
- **Console Application**: Interface de usuário

### Padrões de Design
- **Clean Architecture**: Separação em camadas independentes
- **Single Responsibility Principle**: Cada classe tem uma responsabilidade
- **Dependency Injection**: Injeção manual de dependências
- **Service Pattern**: Serviços especializados para diferentes operações

### Práticas de Desenvolvimento
- **Encapsulamento**: Propriedades com setters privados
- **Validação de entrada**: Verificação rigorosa em construtores
- **Tratamento de exceções**: Captura e logging adequados
- **Logging centralizado**: Rastro completo de operações
- **Consistência de dados**: Validações de integridade referencial
- **Versionamento de relatórios**: Preservação histórica com timestamps únicos
- **Gerenciamento de caminhos**: Localização automática e dinâmica de diretórios do projeto

## Validações e Regras de Negócio

### Validações de Entidade
- **Member**: ID > 0, Nome e Matrícula não vazios, limites de caracteres
- **Game**: ID > 0, Nome não vazio, limite de 100 caracteres
- **Loan**: IDs > 0, controle de datas de empréstimo e devolução

### Regras de Negócio
- **Disponibilidade**: Jogos emprestados ficam indisponíveis
- **Prazo de empréstimo**: 7 dias padrão para devolução
- **Integridade referencial**: Empréstimos devem referenciar membros e jogos existentes
- **Auditoria**: Todas as operações são logadas para rastreabilidade