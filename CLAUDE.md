Overview plan and requirement

- I want to create C# [ASP.NET](http://ASP.NET) Core API project by .NET8 to demonstrate Unit Test
- It response for Add, Update, remove student and calculate grade from their examination result. It can notify student if data change.
- it connect to sending Email and SMS but we use mock for them.
- it connect PostgresDB as database. Use this connection string for demo project "Server=localhost;Port=65432;Database=demounittest01;User ID=admin;Password=admin;Include Error Detail=true;”
- use AAA pattern of Unit test
- Design project as Clean Architecture. Root\AppCore\Demo.AppCore contain Applications path for logic class, Interfaces path for Interface class and Models path for Model class. Root\Infra\Demo.Database contain Migrations path for migration class and Services path for manipulate data service class. Root\Presentations\Demo.Api contain Controllers path for controller class. Root\Infra\Demo.Notification contains connection to Email and SMS
- Add test by Root\Tests\Demo.AppCore.Tests for unit test and Root\Tests\Demo.Api.IntegrationTests for integration test
- Use xUnit, Moq, Coverlet and ReportGenerator
- Use Github Action to run unit test and make report
- Do not want FluentValidation
- Also use Testcontainers for .NET to do Integration tests

Action Plan

- Create Demo.AppCore and Demo.Database to manage database schema
- Create logic in Demo.AppCore to calculate grade.
- Create Interface in Demo.AppCore and Service in Demo.Database and Service in Demo.Notification
- Create Demo.Api and Controllers.
- Create Unit test and related item
- Create Github Action

Data model (minimal)

- **Student**(Id, StudentNo, FirstName, LastName, Email, Phone)
- **Exam**(Id, Name, MaxScore, Weight)
- **ExamResult**(Id, StudentId → Student, ExamId → Exam, Score)
- **OutboxMessage** for reliable notifications later (nice-to-have)

Architecture plan.

Grade calculation logic

Use a weighted average by Exam.Weight. Normalize weights so they don’t have to sum to 1.

Formula:

- total = Σ(score/maxScore * weight)
- weightSum = Σ(weight) (ignore exams with maxScore=0)
- finalPercent = (weightSum == 0) ? 0 : (total / weightSum) * 100
    
    Edge rules: clamp score to [0,maxScore]; if a student has no results → 0%. Round to 2 decimals for API output.
    

Notification triggers

Send (mocked) notifications on meaningful changes:

- Student created/updated/deleted (profile changes).
- ExamResult created/updated/deleted → recalc grade; notify only if finalPercent changed by ≥0.5% OR letter grade changed.
- Optional: on new Exam added with non-zero weight (since grade potential changes).
    
    Debounce per student (e.g., coalesce changes within 2s) so tests are stable.
    

API authentication

For a demo: keep endpoints open by default. Provide a simple optional API key via header `X-Api-Key` (configurable), disabled in `Development`, enabled in `Production`/CI for integration tests.

Database migration strategy

Yes—EF Core Code-First with PostgreSQL.

- Keep migrations in `Infra/Demo.Database/Migrations`.
- Use `dotnet ef migrations add <name>` and `dotnet ef database update` locally.
- In CI/integration tests, rely on Testcontainers’ Postgres and run `context.Database.Migrate()` at startup of the test host.
- Ship a migration bundle for manual runs: `dotnet ef migrations bundle`.
1. Grade format
    
    Return both numeric and letter to keep UX flexible:
    

```json

{ "finalPercent": 86.75, "letter": "B" }
```

Letter mapping (configurable, default):

- A: ≥90
- B: 80–89.99
- C: 70–79.99
- D: 60–69.99
- F: <60
    
    Put thresholds in config so tests can pin exact behavior.
    

Project structure / bootstrap

Create from scratch (clean slate) to match your layout:

- `AppCore/Demo.AppCore` (Models, Interfaces, Services, Events)
- `Infra/Demo.Database` (DbContext, Repositories, Migrations)
- `Infra/Demo.Notification` (Email/SMS adapters; mocked in unit tests)
- `Presentations/Demo.Api` (Controllers; thin)
- `Tests/Demo.AppCore.Tests` (xUnit+Moq, AAA)
- `Tests/Demo.Api.IntegrationTests` (Testcontainers)
- Coverage gate: fail build if `< 80%`.
- Idempotent notification interface (so repeated events don’t spam).
- Health endpoint `/healthz`.


Note for AI
- When you write enough code, please try dotnet build to check error before continue too.
- Add lib from dotnet package add command instead of add directly into project file
- You are working in Window command prompt. When make new directly, mkdir ".github\workflows" instead of mkdir .github\workflows
- To remove file, please use del "Infra\Demo.Database\Class1.cs" as example
- Please declare string size or data type size of number in Model class instead of writing in Context file
