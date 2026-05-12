# Marcio

Sou o assistente de desenvolvimento do projeto Notely. Este documento registra o que implementei e as decisões que tomei.

---

## O que eu fiz

### Feature: Infraestrutura de testes

Implementei o projeto de testes `Notely.Tests` do zero, cobrindo todos os serviços e controllers existentes. O objetivo era atingir ~70% de cobertura com banco de dados isolado da produção e suporte a factories para geração de dados.

---

## Estrutura criada

```
tests/
└── Notely.Tests/
    ├── Notely.Tests.csproj
    ├── GlobalUsings.cs
    ├── appsettings.Testing.json
    ├── Common/
    │   ├── Factories/
    │   │   ├── UserFactory.cs
    │   │   ├── NoteFactory.cs
    │   │   └── NoteGroupFactory.cs
    │   └── Fixtures/
    │       ├── IntegrationWebAppFactory.cs
    │       └── AuthHelper.cs
    ├── Services/
    │   ├── AuthServiceTests.cs
    │   ├── NoteServiceTests.cs
    │   └── NoteGroupServiceTests.cs
    └── Controllers/
        ├── AuthControllerTests.cs
        ├── NotesControllerTests.cs
        └── NoteGroupsControllerTests.cs
```

---

## Decisões técnicas

### Dois tipos de banco para testes

Usei estratégias diferentes dependendo do tipo de teste:

- **EF Core InMemory** nos testes de serviço — rápidos, sem dependência de Docker, isolados por teste (cada um cria um banco com nome `Guid.NewGuid()`). Suficiente para cobrir a lógica LINQ dos serviços.
- **Testcontainers.PostgreSql** nos testes de controller — sobe um PostgreSQL 17 Alpine real, aplica as migrations e testa o fluxo HTTP completo incluindo autenticação JWT, serialização JSON e validações.

### WebApplicationFactory com override de DbContext

O `IntegrationWebAppFactory` herda de `WebApplicationFactory<Program>` e implementa `IAsyncLifetime`. No `ConfigureWebHost`, remove o `DbContextOptions<AppDbContext>` registrado pelo `Program.cs` e o substitui pela connection string do container de teste. As migrations são aplicadas no `InitializeAsync`, antes de qualquer teste.

O `Program.cs` precisou de uma linha adicional no final para expor a classe gerada por top-level statements ao assembly de testes:

```csharp
public partial class Program { }
```

### Factories com Bogus

As três factories (`UserFactory`, `NoteFactory`, `NoteGroupFactory`) usam a biblioteca Bogus com locale `pt_BR`. Isso garante dados únicos por chamada, evitando colisões de email e outros campos únicos quando testes rodam em paralelo.

### AuthHelper

Utilitário estático que registra um usuário via HTTP e retorna o JWT, além de um método de extensão `WithJwt(token)` no `HttpClient`. Elimina a repetição de setup de autenticação nos testes de controller.

---

## Cobertura planejada

| Componente | Casos de teste | Banco |
|---|---|---|
| `AuthService` | 6 | InMemory |
| `NoteService` | 9 | InMemory |
| `NoteGroupService` | 7 | InMemory |
| `AuthController` | 4 | Testcontainers |
| `NotesController` | 9 | Testcontainers |
| `NoteGroupsController` | 7 | Testcontainers |

Total: **42 casos de teste** cobrindo os caminhos principais e de erro, incluindo isolamento entre usuários em todos os recursos.

---

## Como rodar

```bash
# Apenas os testes (requer Docker para os de controller)
dotnet test tests/Notely.Tests

# Com relatório de cobertura
dotnet test tests/Notely.Tests \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:Exclude="[Notely.Tests]*"
```

Os testes de serviço rodam sem Docker. Os de controller sobem e derrubam o container automaticamente via `IAsyncLifetime`.

---

## Arquivos modificados no projeto principal

- `src/Notely.Api/Program.cs` — adicionado `public partial class Program { }` no final, necessário para o `WebApplicationFactory<Program>` enxergar a classe no assembly de testes.
