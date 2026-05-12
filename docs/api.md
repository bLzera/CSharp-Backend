# API Reference

Base URL: `http://localhost:8080`

Swagger UI disponível em `/swagger` (apenas em ambiente de desenvolvimento).

---

## Auth

### POST /auth/register

Cria um novo usuário e retorna um token JWT.

**Body**
```json
{
  "email": "user@example.com",
  "password": "minimo8chars"
}
```

**Respostas**
| Status | Descrição |
|---|---|
| 200 | `{ "token": "eyJ..." }` |
| 400 | Validação falhou (email inválido, senha curta) |
| 409 | Email já cadastrado |

---

### POST /auth/login

Autentica um usuário existente e retorna um token JWT.

**Body**
```json
{
  "email": "user@example.com",
  "password": "minimo8chars"
}
```

**Respostas**
| Status | Descrição |
|---|---|
| 200 | `{ "token": "eyJ..." }` |
| 400 | Validação falhou |
| 401 | Credenciais inválidas |

---

## Notes

Todos os endpoints abaixo exigem o header:
```
Authorization: Bearer <token>
```

---

### GET /notes

Lista todas as notas do usuário autenticado, ordenadas por `updatedAt` decrescente.

**Resposta 200**
```json
[
  {
    "id": "uuid",
    "title": "Título",
    "content": "Conteúdo",
    "createdAt": "2026-05-11T00:00:00Z",
    "updatedAt": "2026-05-11T00:00:00Z"
  }
]
```

---

### GET /notes/{id}

Retorna uma nota específica do usuário autenticado.

**Respostas**
| Status | Descrição |
|---|---|
| 200 | Objeto da nota |
| 401 | Sem token |
| 404 | Nota não encontrada ou pertence a outro usuário |

---

### POST /notes

Cria uma nova nota.

**Body**
```json
{
  "title": "Título",
  "content": "Conteúdo"
}
```

**Respostas**
| Status | Descrição |
|---|---|
| 201 | Nota criada, com header `Location: /notes/{id}` |
| 400 | Validação falhou |
| 401 | Sem token |

---

### PUT /notes/{id}

Atualiza título e conteúdo de uma nota existente.

**Body**
```json
{
  "title": "Novo título",
  "content": "Novo conteúdo"
}
```

**Respostas**
| Status | Descrição |
|---|---|
| 200 | Nota atualizada |
| 400 | Validação falhou |
| 401 | Sem token |
| 404 | Nota não encontrada ou pertence a outro usuário |

---

### DELETE /notes/{id}

Remove uma nota.

**Respostas**
| Status | Descrição |
|---|---|
| 204 | Removida com sucesso |
| 401 | Sem token |
| 404 | Nota não encontrada ou pertence a outro usuário |
