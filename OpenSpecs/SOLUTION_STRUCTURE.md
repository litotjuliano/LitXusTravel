# LitXusTravel .NET Solution Structure
## Clean Architecture Implementation Guide

---

## Project Hierarchy & Dependencies

```
LitXusTravel.sln
│
├── 📦 src/
│
├─── LitXusTravel.Domain/                    ✅ NO DEPENDENCIES
│     └── Pure C# business rules, entities, value objects
│
├─── LitXusTravel.Application/               ⬇️ depends on Domain only
│     └── Use cases, DTOs, interfaces, services
│
├─── LitXusTravel.Infrastructure/            ⬇️ depends on Application + Domain
│     └── Database, external services, implementations
│
├─── LitXusTravel.API/                       ⬇️ depends on all layers
│     └── Controllers, middleware, HTTP endpoints
│
└─── LitXusTravel.Tests/                     📊 tests for all layers
      └── Unit, Integration, E2E tests

```

---

## Project-by-Project Breakdown

### 1. LitXusTravel.Domain

**Purpose:** Enterprise business rules (never changes for technical reasons)

```csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <!-- NO EXTERNAL DEPENDENCIES (except Microsoft packages) -->
</Project>
```

**Key Files:**

```
Entities/
  ├── Tenant.cs                    (Multi-tenancy aggregate root)
  ├── Package.cs                   (Master package aggregate)
  ├── PackageOverride.cs           (Tenant-specific overrides)
  ├── Inquiry.cs
  ├── Quotation.cs
  └── Subscription.cs

ValueObjects/
  ├── TenantId.cs                  (Strongly-typed ID)
  ├── PackageId.cs
  ├── Money.cs
  ├── Email.cs
  └── PackageSyncStatus.cs

DomainEvents/
  ├── TenantCreatedEvent.cs
  ├── PackageSynchronizedEvent.cs
  ├── InquiryReceivedEvent.cs
  └── SubscriptionExpiredEvent.cs

Exceptions/
  ├── DomainException.cs           (Base exception)
  └── InvalidPackageOverrideException.cs
```

### 2. LitXusTravel.Application

**Purpose:** Orchestrates domain logic, translates to DTOs

**NuGet Packages:**
```xml
<PackageReference Include="MediatR" Version="12.0.1" />
<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.0.0" />
<PackageReference Include="FluentValidation" Version="11.9.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
<PackageReference Include="AutoMapper" Version="13.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
```

**Key Files:**

```
UseCases/
  Tenants/
    ├── CreateTenant/
    │   ├── CreateTenantCommand.cs
    │   ├── CreateTenantCommandHandler.cs
    │   ├── CreateTenantValidator.cs
    │   └── CreateTenantCommandTests.cs
    └── GetTenantById/
        ├── GetTenantByIdQuery.cs
        └── GetTenantByIdQueryHandler.cs

  Packages/
    ├── CreatePackage/
    ├── SyncPackageToTenant/        (CORE FEATURE)
    │   ├── SyncPackageCommand.cs
    │   ├── SyncPackageCommandHandler.cs
    │   └── SyncPackageValidator.cs
    └── ApplyPackageOverride/
        ├── ApplyPackageOverrideCommand.cs
        └── ApplyPackageOverrideCommandHandler.cs

  Inquiries/
    ├── CreateInquiry/
    ├── GetInquiries/
    └── RouteInquiryToWhatsApp/

Interfaces/
  Persistence/
    ├── IRepository.cs              (Generic base)
    ├── ITenantRepository.cs
    ├── IPackageRepository.cs
    ├── IInquiryRepository.cs
    └── IUnitOfWork.cs

  Services/
    ├── ITenantResolver.cs          (Multi-tenancy)
    ├── IPackageSyncEngine.cs       (Core business logic)
    ├── IWebsiteProvisioner.cs
    ├── INotificationService.cs
    ├── IWhatsAppService.cs
    └── ICurrentTenant.cs

DTOs/
  Request/
    ├── CreateTenantRequest.cs
    ├── SyncPackageRequest.cs
    ├── CreateInquiryRequest.cs
    └── GenerateQuotationRequest.cs

  Response/
    ├── TenantResponse.cs
    ├── PackageResponse.cs
    ├── InquiryResponse.cs
    └── QuotationResponse.cs

Common/
  ├── Behaviours/
  │   ├── ValidationBehaviour.cs    (Pipeline behavior)
  │   ├── LoggingBehaviour.cs
  │   └── PerformanceBehaviour.cs
  ├── Models/
  │   ├── Result.cs                 (Operation result pattern)
  │   ├── PagedList.cs
  │   └── ApiResponse.cs
  └── Mappings/
      └── MappingProfile.cs         (AutoMapper config)
```

### 3. LitXusTravel.Infrastructure

**Purpose:** Data access, external services, implementations

**NuGet Packages:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
```

**Key Files:**

```
Data/
  Contexts/
    ├── LitXusTravelDbContext.cs     (DbContext)
    └── TenantDbContextFactory.cs   (Multi-tenancy)

  Configurations/                   (EF Core Fluent API)
    ├── TenantConfiguration.cs
    ├── PackageConfiguration.cs
    ├── PackageOverrideConfiguration.cs
    └── InquiryConfiguration.cs

  Migrations/
    ├── 001_InitialCreate.cs
    ├── 002_AddPackageSynchronization.cs
    └── ...

Repositories/
  ├── Repository.cs                 (Generic base)
  ├── TenantRepository.cs
  ├── PackageRepository.cs
  ├── InquiryRepository.cs
  └── UnitOfWork.cs                 (IUnitOfWork implementation)

Services/
  ├── TenantResolver.cs             (Multi-tenancy implementation)
  ├── CurrentTenant.cs
  ├── PackageSyncEngine.cs          (Core business logic)
  ├── WebsiteProvisioner.cs         (Auto provisioning)
  ├── NotificationService.cs
  ├── WhatsAppService.cs
  ├── QuotationGenerator.cs
  └── SubscriptionService.cs

External/
  WhatsApp/
    ├── WhatsAppProvider.cs         (3rd party integration)
    └── WhatsAppHttpClient.cs

  Storage/
    ├── AzureStorageService.cs      (Cloud storage)
    └── LocalStorageService.cs

  Email/
    └── SendGridEmailService.cs

DependencyInjection.cs              (Service registration)
```

### 4. LitXusTravel.API

**Purpose:** HTTP entry point, controllers, middleware

**NuGet Packages:**
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.0.0" />
<PackageReference Include="Serilog" Version="3.0.1" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.0" />
```

**Key Files:**

```
Controllers/
  v1/
    Admin/
      ├── TenantsController.cs      (Tenant CRUD)
      ├── PackagesController.cs     (Package Management)
      ├── SubscriptionsController.cs
      └── AnalyticsController.cs

    Tenants/                         (Tenant-specific endpoints)
      ├── DashboardController.cs
      ├── MyPackagesController.cs   (Synced packages)
      ├── InquiriesController.cs
      ├── QuotationsController.cs
      └── SettingsController.cs

    Public/                          (Public website API)
      ├── PackagesController.cs
      └── InquiriesController.cs

Middleware/
  ├── TenantResolutionMiddleware.cs (Multi-tenancy routing)
  ├── ExceptionHandlingMiddleware.cs
  ├── RequestLoggingMiddleware.cs
  └── CorrelationIdMiddleware.cs

Filters/
  ├── ValidateModelFilter.cs        (Model validation)
  ├── TenantAuthorizationFilter.cs  (Tenant authorization)
  └── ExceptionFilter.cs

Extensions/
  ├── ServiceCollectionExtensions.cs
  └── ApplicationBuilderExtensions.cs

Program.cs                          (Startup configuration)
appsettings.json
appsettings.Development.json
appsettings.Production.json
```

### 5. LitXusTravel.Tests

**NuGet Packages:**
```xml
<PackageReference Include="xunit" Version="2.6.1" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.1" />
<PackageReference Include="Moq" Version="4.20.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
<PackageReference Include="Testcontainers" Version="3.7.0" />
```

**Key Files:**

```
Unit/
  Domain/
    ├── Tenant/
    │   ├── TenantAggregateTests.cs
    │   └── TenantIdTests.cs
    └── Package/
        ├── PackageTests.cs
        └── PackageOverrideTests.cs

  Application/
    Tenants/
      ├── CreateTenantCommandTests.cs
      └── CreateTenantValidatorTests.cs

    Packages/
      ├── SyncPackageCommandTests.cs
      └── ApplyPackageOverrideTests.cs

Integration/
  API/
    ├── Tenants/
    │   └── CreateTenantIntegrationTests.cs
    ├── Packages/
    │   └── SyncPackageIntegrationTests.cs
    └── Inquiries/
        └── InquiryIntegrationTests.cs

  Database/
    ├── RepositoryTests.cs
    └── MultiTenancyTests.cs

  ExternalServices/
    ├── WhatsAppServiceTests.cs
    └── NotificationServiceTests.cs

Common/
  ├── Fixtures/
  │   ├── DatabaseFixture.cs        (Test database setup)
  │   ├── WebApplicationFactory.cs  (API testing)
  │   └── TestDataFixture.cs
  ├── Builders/
  │   ├── TenantBuilder.cs
  │   ├── PackageBuilder.cs
  │   └── InquiryBuilder.cs
  └── TestData.cs
```

---

## .csproj Template

### Domain Project

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>

  <!-- NO dependencies - pure C# -->

</Project>
```

### Application Project

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.0.1" />
    <PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.0.0" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.0" />
    <PackageReference Include="AutoMapper" Version="13.0.1" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\LitXusTravel.Domain\LitXusTravel.Domain.csproj" />
  </ItemGroup>

</Project>
```

### Infrastructure Project

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\LitXusTravel.Domain\LitXusTravel.Domain.csproj" />
    <ProjectReference Include="..\LitXusTravel.Application\LitXusTravel.Application.csproj" />
  </ItemGroup>

</Project>
```

### API Project

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.0.0" />
    <PackageReference Include="Serilog" Version="3.0.1" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\LitXusTravel.Domain\LitXusTravel.Domain.csproj" />
    <ProjectReference Include="..\LitXusTravel.Application\LitXusTravel.Application.csproj" />
    <ProjectReference Include="..\LitXusTravel.Infrastructure\LitXusTravel.Infrastructure.csproj" />
  </ItemGroup>

</Project>
```

---

## Folder Structure to Create

```bash
# Navigate to workspace
cd C:\Nexus Systems\LitXusTravel\LitXusTravel

# Create solution structure
mkdir src
mkdir tests
mkdir web
mkdir docs

# Domain layer
mkdir src\LitXusTravel.Domain\Common
mkdir src\LitXusTravel.Domain\Entities
mkdir src\LitXusTravel.Domain\ValueObjects
mkdir src\LitXusTravel.Domain\DomainEvents
mkdir src\LitXusTravel.Domain\Exceptions

# Application layer
mkdir src\LitXusTravel.Application\UseCases\Tenants\CreateTenant
mkdir src\LitXusTravel.Application\UseCases\Packages\SyncPackageToTenant
mkdir src\LitXusTravel.Application\UseCases\Inquiries
mkdir src\LitXusTravel.Application\DTOs\Request
mkdir src\LitXusTravel.Application\DTOs\Response
mkdir src\LitXusTravel.Application\Interfaces\Persistence
mkdir src\LitXusTravel.Application\Interfaces\Services
mkdir src\LitXusTravel.Application\Mappings
mkdir src\LitXusTravel.Application\Validators

# Infrastructure layer
mkdir src\LitXusTravel.Infrastructure\Data\Contexts
mkdir src\LitXusTravel.Infrastructure\Data\Configurations
mkdir src\LitXusTravel.Infrastructure\Data\Migrations
mkdir src\LitXusTravel.Infrastructure\Repositories
mkdir src\LitXusTravel.Infrastructure\Services
mkdir src\LitXusTravel.Infrastructure\External\WhatsApp
mkdir src\LitXusTravel.Infrastructure\External\Storage
mkdir src\LitXusTravel.Infrastructure\External\Email

# API layer
mkdir src\LitXusTravel.API\Controllers\v1\Admin
mkdir src\LitXusTravel.API\Controllers\v1\Tenants
mkdir src\LitXusTravel.API\Controllers\v1\Public
mkdir src\LitXusTravel.API\Middleware
mkdir src\LitXusTravel.API\Filters
mkdir src\LitXusTravel.API\Extensions

# Tests
mkdir tests\LitXusTravel.Tests\Unit\Domain
mkdir tests\LitXusTravel.Tests\Unit\Application
mkdir tests\LitXusTravel.Tests\Integration\API
mkdir tests\LitXusTravel.Tests\Integration\Database
mkdir tests\LitXusTravel.Tests\Common

# Frontend
mkdir web\public-website
mkdir web\admin-portal
mkdir web\tenant-dashboard
mkdir web\shared-ui

# Documentation
mkdir docs\openapi
```

---

## Next Steps

1. **Create `.sln` file:** `LitXusTravel.sln`
2. **Create `.csproj` files** for each project
3. **Setup dependency injection** in `Program.cs`
4. **Configure EF Core** with multi-tenancy
5. **Define domain entities** and value objects
6. **Implement repositories** and unit of work
7. **Create first use case** (CreateTenant)
8. **Setup API controllers** with Swagger/OpenAPI
9. **Configure middleware** for tenant resolution
10. **Write domain and application tests**

Claude Code will handle:
- Next.js frontend implementation
- Component development with shadcn/ui
- API integration
- Deployment configuration

---

**Solution Structure Ready for Implementation** ✅
