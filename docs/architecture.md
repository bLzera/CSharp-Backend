# Arquitetura

## Stack

| Camada | Tecnologia |
|---|---|
| Framework | ASP.NET Core 8 (Web API) |
| ORM | Entity Framework Core 8 |
| Banco de dados | PostgreSQL 17 |
| Auth | JWT Bearer (access token) |
| Hash de senha | BCrypt |
| Documentação | Swagger (Swashbuckle) |
| Container | Docker + Docker Compose |

## Estrutura de pastas

```
src/Notely.Api/
├── Controllers/      # Parsing de request e respostas HTTP
├── Services/         # Lógica de negócio
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
├── Models/           # Entidades do banco
└── DTOs/             # Contratos de entrada e saída da API
    ├── Auth/
    └── Notes/
```

## Fluxo de dados

```
Request → Controller → Service → AppDbContext → PostgreSQL
```

## Modelos

### User
| Campo | Tipo | Observação |
|---|---|---|
| Id | Guid | PK |
| Email | string(256) | unique |
| PasswordHash | string | BCrypt |
| CreatedAt | DateTime | UTC |

### Note
| Campo | Tipo | Observação |
|---|---|---|
| Id | Guid | PK |
| UserId | Guid | FK → User, cascade delete |
| Title | string(255) | |
| Content | string | |
| CreatedAt | DateTime | UTC |
| UpdatedAt | DateTime | UTC |

## Autenticação

- Login e registro retornam um JWT de curta duração (padrão: 24h)
- O token carrega os claims `sub` (userId) e `email`
- Todos os endpoints de `/notes` exigem `Authorization: Bearer <token>`
- Notas são sempre filtradas pelo `UserId` extraído do token — um usuário nunca enxerga dados de outro
