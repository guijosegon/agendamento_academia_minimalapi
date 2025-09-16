# AgendamentoAcademia – Desafio Técnico

API em **.NET 8 (Minimal API)** para **agendamento de aulas coletivas** em academia, com:
- cadastro de **alunos** (nome e plano),
- cadastro de **aulas** (tipo, **dataHora**, capacidade),
- **agendamento** respeitando capacidade e limite mensal por plano,
- **relatório mensal por aluno** (`/alunos/{id}/relatorio`).

![Tela Inicial](https://raw.githubusercontent.com/guijosegon/project-assets/master/AgendamentoAcademia/swagger.png)

Swagger habilitado e enums serializados como **string**.

---

## 🧰 Stack
- .NET 8
- ASP.NET Core Minimal API
- Entity Framework Core 8 + **SQLite**
- Swashbuckle (Swagger)
- xUnit (projeto de testes)

---

## 📁 Estrutura (resumo)

```
AgendamentoAcademia.sln
AgendamentoAcademia.API/
  ├─ Api/
  │  ├─ Endpoints/
  │  │  ├─ AlunoEndpoints.cs
  │  │  ├─ AulaEndpoints.cs
  │  │  └─ AgendamentoEndpoints.cs
  │  └─ EnumSchemaFilter.cs
  ├─ Application/
  │  ├─ DTOs/
  │  │  ├─ AlunoDTOs.cs
  │  │  ├─ AulaDTOs.cs
  │  │  └─ AgendamentoDTOs.cs
  │  └─ Services/AgendamentoService.cs
  ├─ Domain/
  │  ├─ Entities/ (Aluno, Aula, Agendamento)
  │  └─ Extensions/Enums.cs (TipoPlano, TipoAula)
  ├─ Infra/
  │  ├─ AcademiaDbContext.cs
  │  ├─ Seeders/ (AlunoSeeder, AulaSeeder)
  │  └─ Data/ (SQLite *.db, *.db-wal, *.db-shm)  ← ignorado no Git
  ├─ appsettings.json
  └─ Program.cs
AgendamentoAcademia.Tests/
  └─ AgendamentoTests.cs
```

---

## ⚙️ Configuração

### Requisitos
- .NET **SDK 8.0**
- (opcional) Visual Studio 2022 / Rider / VS Code

### Connection string (SQLite)
`AgendamentoAcademia.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Data Source=Infra/Data/academia.db"
  }
}
```

---

## ▶️ Como executar

### Visual Studio 2022
1. Abra a solução `AgendamentoAcademia.sln`.
2. Defina **AgendamentoAcademia.API** como *Startup Project*.
3. **F5** (ou `Ctrl+F5`). O Swagger abre em `/swagger`.

### CLI
```bash
dotnet build
dotnet run --project AgendamentoAcademia.API
# ou definir a URL:
# DOTNET_URLS="https://localhost:7112;http://localhost:5112" dotnet run --project AgendamentoAcademia.API
```

> Se o navegador acusar certificado inválido, rode:
> ```
> dotnet dev-certs https --clean
> dotnet dev-certs https --trust
> ```

---

## 🌐 Swagger

- UI: `https://localhost:7112/swagger`
- `EnumSchemaFilter` documenta enums como **string**.
- No runtime os enums também são **string** (`JsonStringEnumConverter`).

---

## 🔒 Regras de Negócio (no `AgendamentoService`)
- **Capacidade**: não permite agendar acima da capacidade da aula.
- **Limite mensal por plano** (mês/ano da **AULA**):
  - Mensal = 12, Trimestral = 20, Anual = 30.
- **Duplicidade**: impede mesmo aluno na mesma aula (índice único + validação).
- **Relatório**: total do mês e ranking dos tipos de aula mais frequentes.

---

## 🔗 Endpoints (principais)

### `POST /alunos`
Cria aluno.
```json
{
  "nome": "Alice",
  "plano": "Mensal" // enum como string ou int
}
```

### `GET /alunos?skip=0&take=10`
Lista paginada de alunos.

### `GET /alunos/{id}/relatorio?ano=YYYY&mes=MM`
Retorna o relatório mensal do aluno (usa `AgendamentoService.ReportAsync`).  
**Exemplo de resposta:**
```json
{
  "alunoId": 1,
  "alunoNome": "Alice",
  "ano": 2025,
  "mes": 9,
  "totalAgendamentos": 3,
  "aulasFrequentes": [
    { "tipo": "Pilates", "quantidade": 2, "porcentagem": 66.67 },
    { "tipo": "Cross", "quantidade": 1, "porcentagem": 33.33 }
  ]
}
```

---

### `POST /aulas`
Cria aula.
```json
{
  "tipo": "Pilates", // enum como string ou int
  "dataHora": "2025-09-20T18:00:00Z",
  "capacidade": 10
}
```

### `GET /aulas/{id}`
Detalhe da aula.
```json
{
  "id": 3,
  "tipo": "Pilates",
  "dataHora": "2025-09-20T18:00:00Z",
  "capacidade": 10
}
```

---

### `POST /agendamentos`
Agenda um aluno em uma aula.
```json
{
  "alunoId": 1,
  "aulaId": 3
}
```
**Respostas possíveis**
- `201 Created` → `AgendamentoResponse` com `criadoEm` (DateTimeOffset).
- `400 BadRequest` → regra de negócio (capacidade, limite do plano, aluno/aula inexistentes).

**Exemplo de resposta 201:**
```json
{
  "id": 10,
  "alunoId": 1,
  "aulaId": 3,
  "criadoEm": "2025-09-10T12:34:56.789Z"
}
```

---

## 🧪 Testes
Projeto `AgendamentoAcademia.Tests` (xUnit) com testes de regras principais.
```bash
dotnet test
```

---

## 🛠️ Notas de implementação
- **Datas**: usar `DateTimeOffset` em DTOs/entidades (campo `dataHora` em aulas e `criadoEm` no agendamento).
- **Enums**: serializados como string (ex.: `Mensal`, `Trimestral`, `Anual`).  
- **Modelagem**: índice único `(AulaId, AlunoId)` em `Agendamento` evita duplicidade.  
- **Seed**: `AlunoSeedAsync()` e `AulaSeedAsync()` rodam no startup para dados de exemplo.

---
