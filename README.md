# Demo Unit Test Project

A comprehensive C# ASP.NET Core API project demonstrating unit testing best practices with Clean Architecture.

## Features

- **Clean Architecture** with separation of concerns
- **Grade calculation** using weighted averages
- **Student management** with CRUD operations
- **Notification system** with Email/SMS mocking
- **Comprehensive testing** with 80% coverage requirement
- **PostgreSQL database** with Entity Framework Core
- **API authentication** (optional API key)
- **Docker support** with Testcontainers for integration tests

## Project Structure

```
├── AppCore/Demo.AppCore/          # Application Core (Models, Interfaces, Services)
├── Infra/Demo.Database/           # Database Infrastructure (EF Core, Repositories)
├── Infra/Demo.Notification/       # Notification Services (Email, SMS mocks)
├── Presentations/Demo.Api/        # Web API Controllers
├── Tests/Demo.AppCore.Tests/      # Unit Tests (xUnit + Moq)
└── Tests/Demo.Api.IntegrationTests/ # Integration Tests (Testcontainers)
```

## Technologies Used

- **.NET 8.0**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL**
- **xUnit** for testing
- **Moq** for mocking
- **Testcontainers** for integration testing
- **Coverlet** for code coverage
- **GitHub Actions** for CI/CD

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL (or Docker for development)
- Git

### Database Setup

The application uses PostgreSQL with the following default connection:
```
Server=localhost;Port=65432;Database=demounittest01;User ID=admin;Password=admin;Include Error Detail=true;
```

### Running the Application

1. Clone the repository
2. Restore dependencies:
   ```bash
   dotnet restore Demo.sln
   ```
3. Build the solution:
   ```bash
   dotnet build Demo.sln
   ```
4. Run the API:
   ```bash
   dotnet run --project Presentations/Demo.Api/Demo.Api.csproj
   ```

### Running Tests

**Unit Tests:**
```bash
dotnet test Tests/Demo.AppCore.Tests/Demo.AppCore.Tests.csproj
```

**Integration Tests:**
```bash
dotnet test Tests/Demo.Api.IntegrationTests/Demo.Api.IntegrationTests.csproj
```

**All Tests with Coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## API Endpoints

- `GET /healthz` - Health check
- `GET /api/students` - Get all students
- `GET /api/students/{id}` - Get student by ID
- `GET /api/students/{id}/grade` - Get student's calculated grade
- `POST /api/students` - Create new student
- `PUT /api/students/{id}` - Update student
- `DELETE /api/students/{id}` - Delete student

## Grade Calculation

Grades are calculated using weighted averages:

- **Formula**: `total = Σ(score/maxScore * weight)`
- **Final Grade**: `(total / weightSum) * 100`
- **Letter Grades**: A (≥90%), B (80-89%), C (70-79%), D (60-69%), F (<60%)

## Testing Strategy

The project follows the **AAA pattern** (Arrange, Act, Assert):

- **Unit Tests**: Test individual components in isolation using mocks
- **Integration Tests**: Test complete workflows with real database via Testcontainers
- **Coverage Gate**: Minimum 80% code coverage required for builds

## CI/CD Pipeline

GitHub Actions workflow includes:

- Automated testing on push/PR
- Code coverage reporting
- Coverage threshold enforcement (80%)
- Automated builds and deployments

## Authentication

Optional API key authentication can be enabled in production:

- Set `ApiKeyRequired: true` in configuration
- Include `X-Api-Key` header with requests
- Default key: `demo-api-key-12345`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Write tests for new functionality
4. Ensure all tests pass and coverage meets requirements
5. Submit a pull request