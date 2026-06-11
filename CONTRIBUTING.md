# Contributing to LitXusTravel

Thank you for your interest in contributing to LitXusTravel! This guide will help you get started.

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- SQL Server Express or LocalDB
- Git

### Setup Development Environment

```bash
# Clone repository
git clone https://github.com/nexus-systems/nexus-travel.git
cd nexus-travel

# Restore .NET dependencies
dotnet restore

# Install frontend dependencies
cd web/admin-portal && npm install
cd ../public-website && npm install
cd ../..

# Create local database
dotnet ef database update --project src/LitXusTravel.Infrastructure
```

## 📋 Development Workflow

### 1. Create Feature Branch

```bash
git checkout -b feature/your-feature-name
# or
git checkout -b bugfix/your-bug-fix
```

**Branch naming conventions:**
- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `refactor/description` - Code improvements
- `docs/description` - Documentation
- `test/description` - Tests

### 2. Make Your Changes

**Code Style:**
- Follow Clean Architecture (Domain → Application → Infrastructure → API)
- Use CQRS pattern with MediatR
- PascalCase for C# classes/methods
- camelCase for JavaScript/TypeScript

**Commit Messages:**
```
feat: add new feature description
fix: fix bug description
docs: update documentation
refactor: improve code quality
test: add tests for feature
```

### 3. Test Your Changes

**Backend Tests:**
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/LitXusTravel.Tests.Integration/LitXusTravel.Tests.Integration.csproj

# Run with coverage
dotnet test /p:CollectCoverageReport=true
```

**Frontend Tests:**
```bash
# Lint code
cd web/admin-portal && npm run lint

# Build
npm run build

# Run tests
npm test
```

### 4. Build Verification

```bash
# .NET Build
dotnet build --configuration Release

# Frontend Build
cd web/admin-portal && npm run build
cd ../public-website && npm run build
```

### 5. Create Pull Request

- Push to your fork
- Create PR against `main` branch
- Fill out PR template completely
- Link related issues
- Wait for CI/CD pipeline to pass
- Request code review

## 🏗️ Architecture Guidelines

### Clean Architecture Layers

```
Domain (LitXusTravel.Domain)
├─ Entities
├─ ValueObjects
├─ Aggregates
└─ Exceptions

Application (LitXusTravel.Application)
├─ UseCases/[Feature]/[Operation]/
│  ├─ CommandOrQuery.cs
│  ├─ CommandOrQueryHandler.cs
│  ├─ CommandOrQueryValidator.cs
│  └─ CommandOrQueryTests.cs
├─ Interfaces
├─ DTOs
└─ Validators

Infrastructure (LitXusTravel.Infrastructure)
├─ Persistence (EF Core, Repositories)
├─ Services (External APIs, Email, etc.)
├─ Identity (JWT, Auth)
└─ Data (Migrations, Seeding)

API (LitXusTravel.API)
├─ Controllers
├─ Middleware
├─ Program.cs
└─ appsettings.json
```

### CQRS Pattern

**Commands (Mutations):**
```csharp
// Command definition
public record CreatePackageCommand(
    string Title,
    string Destination,
    decimal BasePrice
) : IRequest<Result<PackageResponse>>;

// Handler
public class CreatePackageCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreatePackageCommand, Result<PackageResponse>>
{
    public async Task<Result<PackageResponse>> Handle(
        CreatePackageCommand request, CancellationToken ct)
    {
        // Validation, business logic, persistence
        var package = Package.Create(request.Title, request.Destination, request.BasePrice);
        await uow.Packages.AddAsync(package, ct);
        await uow.SaveChangesAsync(ct);
        
        return Result<PackageResponse>.Success(MapToResponse(package));
    }
}
```

**Queries (Reads):**
```csharp
public record GetPackageByIdQuery(Guid Id) : IRequest<Result<PackageResponse>>;

public class GetPackageByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPackageByIdQuery, Result<PackageResponse>>
{
    public async Task<Result<PackageResponse>> Handle(
        GetPackageByIdQuery request, CancellationToken ct)
    {
        var package = await uow.Packages.GetByIdAsync(request.Id, ct);
        if (package is null)
            return Result<PackageResponse>.Failure("Package not found.");
        
        return Result<PackageResponse>.Success(MapToResponse(package));
    }
}
```

### Multi-Tenancy Pattern

Every query MUST include tenant filtering:

```csharp
// ❌ WRONG - No tenant filtering
var packages = await context.Packages.ToListAsync();

// ✅ CORRECT - Tenant context is automatic
// Global query filters in DbContext handle it
var packages = await context.Packages.ToListAsync();
```

## 📚 Common Tasks

### Adding a New Feature

```
1. Create domain entity in Domain layer
2. Create command/query and handler in Application
3. Add validator if needed
4. Create repository in Infrastructure (if needed)
5. Add API controller endpoint
6. Add frontend page/component
7. Add tests
8. Update documentation
```

### Adding a Database Migration

```bash
dotnet ef migrations add DescribeYourChanges \
  --project src/LitXusTravel.Infrastructure \
  --startup-project src/LitXusTravel.API

# Review generated migration file

dotnet ef database update
```

### Adding a New API Endpoint

```csharp
[ApiController]
[Route("api/v1/packages")]
[Authorize(Roles = "Admin")]
public class PackagesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPackageByIdQuery(id), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }
}
```

## 🧹 Code Review Checklist

**Reviewers should verify:**

- [ ] Code follows architecture guidelines
- [ ] CQRS pattern correctly applied
- [ ] No direct database access outside repositories
- [ ] Proper error handling with Result<T> pattern
- [ ] Validation implemented
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] No security vulnerabilities
- [ ] Performance considered
- [ ] Database migrations working correctly

## 📊 Commit Message Convention

```
<type>: <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style
- `refactor`: Code refactoring
- `perf`: Performance improvement
- `test`: Test changes
- `ci`: CI/CD changes
- `chore`: Build/dependency changes

**Example:**
```
feat: implement package synchronization engine

- Added SyncPackagesToTenantCommand handler
- Implemented NULL field override semantics
- Added comprehensive integration tests

Closes #42
```

## 🔐 Security Considerations

- Never commit secrets (use environment variables)
- Validate all user input
- Implement proper authorization checks
- Use parameterized SQL queries (EF Core does this)
- Hash passwords securely
- Implement rate limiting on API endpoints
- Use HTTPS in production

## 📖 Documentation

Update documentation when:
- Adding new features
- Changing API endpoints
- Modifying database schema
- Adding deployment steps
- Updating configuration

## 🤝 Community

- Be respectful and professional
- Follow code of conduct
- Help other contributors
- Share knowledge and best practices

## ❓ Questions?

- Check existing issues/PRs
- Read documentation in `/docs`
- Ask in GitHub Discussions
- Contact maintainers

---

**Thank you for contributing to LitXusTravel!** 🎉
