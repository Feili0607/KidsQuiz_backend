# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

KidsQuiz is a .NET 9.0 web API backend for an educational quiz platform designed for children. The application follows a clean architecture pattern with three main layers:

- **KidsQuiz.API**: ASP.NET Core Web API layer with controllers, middleware, and configuration
- **KidsQuiz.Services**: Business logic layer with services, DTOs, and domain interfaces
- **KidsQuiz.Data**: Data access layer with Entity Framework Core, models, and migrations

## Development Commands

### Build and Run
```bash
# Restore dependencies
dotnet restore KidsQuiz.sln

# Build the solution
dotnet build KidsQuiz.sln --configuration Release

# Run the API locally
dotnet run --project KidsQuiz.API/KidsQuiz.API.csproj
# API will be available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger

# Using Docker
docker-compose up
```

### Database Operations
```bash
# Run migrations
dotnet ef database update --project KidsQuiz.Data --startup-project KidsQuiz.API

# Create a new migration
dotnet ef migrations add <MigrationName> --project KidsQuiz.Data --startup-project KidsQuiz.API
```

### Code Quality
```bash
# Format code
dotnet format KidsQuiz.sln

# Check for formatting issues
dotnet format --verify-no-changes KidsQuiz.sln

# Check for vulnerable packages
dotnet list package --vulnerable

# Check for outdated dependencies
dotnet outdated --fail-on-updates
```

### Testing
Note: No test projects currently exist. When adding tests:
```bash
# Run all tests
dotnet test KidsQuiz.sln --verbosity normal

# Run specific test project
dotnet test <TestProject>.csproj
```

## Architecture and Key Patterns

### Layered Architecture
1. **API Layer** (`KidsQuiz.API`):
   - Controllers handle HTTP requests/responses
   - Custom middleware for exception handling and request logging
   - Swagger/OpenAPI documentation
   - CORS configuration for frontend communication
   - Dependency injection configuration

2. **Service Layer** (`KidsQuiz.Services`):
   - Business logic implementation
   - DTOs for data transfer between layers
   - Custom exceptions for domain-specific errors
   - Interfaces defining service contracts
   - Caching support via `ICacheService`
   - LLM integration for AI-generated quizzes

3. **Data Layer** (`KidsQuiz.Data`):
   - Entity Framework Core with SQL Server
   - Domain models (Kid, Quiz, Question, QuizSolvingRecord, QuestionBank)
   - Value Objects (AgeGroup, DifficultyLevel, InterestCategory)
   - Entity configurations using Fluent API
   - Database migrations with seed data

### Key Services
- **IKidService**: Manages child user profiles
- **IQuizService**: Handles quiz operations and retrieval
- **IQuizSolvingRecordService**: Tracks quiz completion and scoring
- **ILLMQuizService**: Generates quizzes using AI/LLM integration
- **IQuestionBankService**: Manages question repository

### Middleware Pipeline
1. Exception handling middleware catches and formats errors
2. Request logging middleware tracks API usage
3. CORS middleware enables frontend communication
4. Authorization middleware (configured but not currently enforced)

### Database Context
- Uses Entity Framework Core with SQL Server
- Connection string in appsettings.json: `DefaultConnection`
- Migrations track schema changes
- Seed data provides initial quiz content

### Logging
- Serilog for structured logging
- Application Insights integration for telemetry
- Console and file sinks configured
- Request/response logging via middleware

### API Endpoints Pattern
Controllers follow RESTful conventions:
- `GET /api/{resource}` - List all
- `GET /api/{resource}/{id}` - Get single
- `POST /api/{resource}` - Create new
- `PUT /api/{resource}/{id}` - Update existing
- `DELETE /api/{resource}/{id}` - Delete

### Configuration
- Environment-specific settings via appsettings.{Environment}.json
- Docker support with docker-compose.yml
- Launch profiles in Properties/launchSettings.json
- CI/CD pipeline configured in .github/workflows/ci.yml

## Important Notes
- Currently on branch `feat/sam` with uncommitted changes
- No test projects exist yet - consider adding when implementing new features
- API redirects root path to Swagger UI for easy exploration
- Frontend expected to run on localhost:3000
- Backend exposes port 5000 (80 in Docker)