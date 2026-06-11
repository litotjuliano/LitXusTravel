# LitXusTravel - Architectural Decisions & Implementation Guide

---

## 🔄 UAT Workflow (LitXusTravel ↔ LitXusDevHub)

**Our role:** SENDER — we write UAT lists and receive feedback from DevHub.

**Protocol:**
- Write UAT list → `uat/outgoing/uat-list-v{N}.md`
- Read DevHub feedback → `uat/incoming/test-report-v{N}.md`
- Fix flagged issues → notify DevHub for re-test

**Full details:** See `UAT-WORKFLOW.md` in project root.

**Status markers in feedback files:**
- ✅ Passed — no action needed
- ❌ Failed — fix and notify DevHub
- 🔄 Re-testing — DevHub is retesting your fix
- ⏳ Pending — not yet tested

---

**Project:** LitXusTravel - Travel Package Distribution Platform  
**Start Date:** May 28, 2026  
**Architecture:** Clean Architecture + Multi-Tenant SaaS  
**Tech Stack:** .NET 8, SQL Server, Next.js, shadcn/ui, Framer Motion

---

## 🏗️ Architectural Decisions

### 1. Clean Architecture (CRITICAL)

**Decision:** Organize codebase in 4 layers with unidirectional dependencies.

**Why:** 
- Framework/infrastructure can be replaced without affecting domain logic
- Easy to test business rules in isolation
- Clear separation of concerns
- Scales to microservices later

**Layers:**
```
Domain (no dependencies)
   ↓
Application (depends on Domain only)
   ↓
Infrastructure (depends on Application + Domain)
   ↓
API (depends on all layers)
```

**Implementation:**
- Domain has ZERO external package dependencies (except System)
- Application has MediatR, FluentValidation, AutoMapper
- Infrastructure has EF Core, external services
- API is just HTTP routing and middleware

---

### 2. Multi-Tenancy Strategy

**Decision:** Shared database with automatic tenant filtering.

**Why:**
- Simpler deployment (1 database)
- Automatic data isolation via middleware
- Easy migrations
- Cost-effective for SaaS

**Mechanism:**
- Middleware extracts tenant from subdomain/hostname
- `ICurrentTenant` scoped service holds current context
- All queries automatically filtered by TenantId via DbContext query filters
- NEVER allow queries without tenant context

**Critical Rule:**
```csharp
// Every query MUST include tenant check:
var inquiries = dbContext.Inquiries
    .Where(i => i.TenantId == currentTenant.Id)  // MANDATORY
    .ToListAsync();
```

**Security:**
- Global query filters prevent accidental data leaks
- Tenant authorization filters on API endpoints
- Audit logs track access by tenant

---

### 3. Package Synchronization Engine (Core Differentiator)

**Decision:** Master-Tenant package model with smart override logic.

**Why:**
- Avoids package duplication
- Allows tenant customization without affecting master
- Portal updates propagate to tenants automatically
- Preserves tenant overrides across sync cycles

**Flow:**
```
Master Package
    ↓
Tenant Synchronization
    ↓
Tenant Package (copy of master)
    +
Package Overrides (title, price, images)
    ↓
Final Tenant Package (master + overrides)
```

**Implementation:**
- `Packages` = Master packages (admin-created)
- `TenantPackages` = Synchronization records
- `PackageOverrides` = Tenant customizations
- Sync is idempotent (can run multiple times safely)

**Key Business Logic:**
```csharp
// SyncPackageToTenantCommandHandler.cs
public async Task Handle(SyncPackageCommand request, CancellationToken ct)
{
    var masterPackage = await _packageRepository.GetByIdAsync(request.PackageId);
    
    // Check if already synced
    var existingSync = await _tenantPackageRepository
        .GetAsync(t => t.TenantId == _currentTenant.Id 
               && t.MasterPackageId == masterPackage.Id);
    
    if (existingSync == null)
    {
        // New sync - create mapping
        var tenantPackage = TenantPackage.CreateFromMaster(
            masterPackage, _currentTenant.Id);
        await _tenantPackageRepository.AddAsync(tenantPackage);
    }
    else
    {
        // Update sync status, preserve overrides
        existingSync.UpdateSyncStatus(SyncStatus.Synced);
    }
    
    await _unitOfWork.SaveChangesAsync();
}
```

---

### 4. CQRS (Commands & Queries Separation)

**Decision:** Use MediatR to separate commands (mutations) and queries (reads).

**Why:**
- Different scaling concerns (reads >> writes typically)
- Clear intent (what are you trying to do?)
- Easy to add cross-cutting concerns (logging, validation, metrics)

**Pattern:**
```
Request → MediatR Pipeline:
  1. Validation Behavior
  2. Logging Behavior
  3. Performance Monitoring Behavior
  4. Handler

Command = mutation (CreateTenant, SyncPackage, etc.)
Query = read-only (GetTenantById, SearchPackages, etc.)
```

**File Structure:**
```
UseCases/Tenants/CreateTenant/
  ├── CreateTenantCommand.cs          (Request)
  ├── CreateTenantCommandHandler.cs   (Logic)
  ├── CreateTenantValidator.cs        (Validation)
  └── CreateTenantCommandTests.cs     (Tests)

UseCases/Packages/GetTenantPackages/
  ├── GetTenantPackagesQuery.cs       (Request)
  └── GetTenantPackagesQueryHandler.cs (Logic)
```

---

### 5. Repository & Unit of Work Pattern

**Decision:** Generic repository + unit of work for data access.

**Why:**
- Consistent data access across layers
- Easy to swap implementations (in-memory for tests, real DB for prod)
- Atomic operations with UnitOfWork.SaveChangesAsync()

**Implementation:**
```csharp
// IRepository<T> - generic interface
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
}

// IUnitOfWork - coordinates multiple repos
public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<Package> Packages { get; }
    IRepository<Inquiry> Inquiries { get; }
    IRepository<Quotation> Quotations { get; }
    // ...
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<int> ExecuteTransactionAsync(Func<Task> action);
}
```

---

### 6. Domain-Driven Design (Strategic)

**Decision:** Model business concepts as domain entities, not just data containers.

**Why:**
- Domain logic lives in domain layer (no anemic models)
- Value objects prevent invalid states
- Domain events capture business events
- Aggregate roots enforce consistency

**Example - Tenant Aggregate:**
```csharp
public class Tenant : AggregateRoot
{
    public TenantId Id { get; private set; }
    public string Name { get; private set; }
    public Email ContactEmail { get; private set; }  // Value object
    public TenantSubscription CurrentSubscription { get; private set; }
    
    // Business rules enforced in domain
    public static Tenant Create(string name, Email email)
    {
        if (string.IsNullOrEmpty(name)) 
            throw new DomainException("Tenant name is required");
        
        var tenant = new Tenant
        {
            Id = TenantId.New(),
            Name = name,
            ContactEmail = email,
        };
        
        tenant.RaiseDomainEvent(new TenantCreatedEvent(tenant.Id));
        return tenant;
    }
    
    public void ActivateSubscription(SubscriptionPlan plan)
    {
        if (CurrentSubscription?.IsActive ?? false)
            throw new DomainException("Tenant already has active subscription");
        
        CurrentSubscription = TenantSubscription.CreateForPlan(plan);
        RaiseDomainEvent(new SubscriptionActivatedEvent(Id, plan));
    }
}
```

---

### 7. Automatic Website Provisioning

**Decision:** Trigger website generation when tenant subscribes.

**Flow:**
```
1. TenantCreatedEvent raised
2. WebsiteProvisionerService subscribed to event
3. Creates template from ProvisioningTemplate
4. Assigns subdomain + SSL
5. Seeds default pages
6. Updates provisioning status
```

**Implementation:**
```csharp
// In Startup (Program.cs)
services.AddDomainEventHandlers();
services.AddTransient<ITenantCreatedEventHandler, TenantWebsiteProvisioningHandler>();

// Handler
public class TenantWebsiteProvisioningHandler 
    : INotificationHandler<TenantCreatedEvent>
{
    public async Task Handle(TenantCreatedEvent @event, CancellationToken ct)
    {
        var tenant = await _tenantRepository.GetByIdAsync(@event.TenantId, ct);
        
        // Provision website from template
        await _websiteProvisioner.ProvisionForTenant(tenant, ct);
        
        // Update tenant status
        tenant.MarkProvisioningComplete();
        await _tenantRepository.UpdateAsync(tenant, ct);
    }
}
```

---

### 8. API Versioning Strategy

**Decision:** URL-based versioning with `/api/v1/`, `/api/v2/` routes.

**Why:**
- Clear breaking change boundaries
- Clients can opt into new APIs
- Old versions stay stable

**Structure:**
```
Controllers/
  v1/
    Admin/TenantsController.cs
    Tenants/MyPackagesController.cs
    Public/PackagesController.cs
  
  v2/  (future)
    Admin/TenantsController.cs  (new endpoints)
```

---

### 9. Error Handling & Result Pattern

**Decision:** Use `Result<T>` pattern instead of exceptions for business errors.

**Why:**
- Explicit about success/failure
- No exception overhead for validation failures
- Clear error messages to clients

**Implementation:**
```csharp
public record Result
{
    public bool IsSuccess { get; init; }
    public string[] Errors { get; init; } = [];
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(params string[] errors) => 
        new() { IsSuccess = false, Errors = errors };
}

// Usage
public async Task<Result> CreateTenant(CreateTenantRequest request)
{
    var validation = await _validator.ValidateAsync(request);
    if (!validation.IsValid)
        return Result.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
    
    var tenant = Tenant.Create(request.Name, new Email(request.Email));
    // ...
    return Result.Success();
}
```

---

### 10. Role Hierarchy & Authorization

**Decision:** Three-tier role hierarchy: SuperAdmin → Admin → Agents

**Why:**
- Delegation of responsibilities (SuperAdmin creates Admins)
- Scalability (Admins manage Tenants)
- Clear permission boundaries
- Audit trail for all actions

**Roles:**

```
SuperAdmin (Full platform control)
  ├─ Create other Admins
  ├─ Create global subscription packages
  ├─ Oversee all dashboards (read-only)
  ├─ Audit all Admin activities
  └─ Dispute resolution authority

Admin (Two variants)
  ├─ Platform Admin
  │  ├─ Manage operational aspects
  │  ├─ Assign tenants
  │  └─ Cannot create other Admins
  └─ Tenant Admin (assigned to 1+ tenants)
     ├─ Create tenants
     ├─ Manage packages & pricing
     ├─ Create staff agents
     └─ Configure commission rules

StaffAgent (Internal employees)
  ├─ Scoped to single tenant
  ├─ Create/manage tours
  ├─ Book customers on tours
  ├─ Earn commissions on sales
  └─ Access: tenant1.LitXusTravel.com (white-label)

IndependentAgent (Freelancers)
  ├─ Work with multiple tenants
  ├─ Resell tenant tours
  ├─ Own subscription & white-label site
  ├─ Earn commission on bookings
  └─ Access: agent1.LitXusTravel.com (white-label)
```

**Token Claims:**
```json
// SuperAdmin
{
  "role": "SuperAdmin",
  "sub": "user-id"
}

// Platform Admin
{
  "role": "Admin",
  "scope": "Platform",
  "sub": "user-id"
}

// Tenant Admin
{
  "role": "Admin",
  "scope": "Tenant",
  "tenantId": "guid",
  "sub": "user-id"
}

// Staff Agent
{
  "role": "StaffAgent",
  "tenantId": "guid",
  "agentId": "guid",
  "sub": "user-id"
}

// Independent Agent
{
  "role": "IndependentAgent",
  "agentId": "guid",
  "sub": "user-id"
}
```

**Full Details:** See `ROLE_HIERARCHY_AND_COMMISSION_SYSTEM.md`

---

### 11. Commission System & Safeguards

**Decision:** Flexible commission model for both staff and freelancers with 10 critical safeguards

**Why:**
- Incentivizes both internal staff and external resellers
- Prevents fraud through comprehensive safeguards
- Transparent commission tracking
- Dispute resolution instead of SuperAdmin override

**Commission Models:**

**Staff Agent Commission:**
```
Triggered on: Tour booked/completed by staff
Configured by: Tenant Admin (per staff member or rule-based)
Options:
  ├─ Fixed: $50 per booking
  ├─ Percentage: 10% of booking price
  ├─ Tiered: Volume-based rates
  └─ Hybrid: Base salary + commission
```

**Independent Agent Commission (Reselling):**
```
Triggered on: Booking made on agent's white-label site
Configured by: Tenant (per agent or category)
Options:
  ├─ Fixed: $50 per booking
  ├─ Percentage: 20% of booking price
  ├─ Tiered: 15%/20%/25% by volume
  ├─ Markup: Agent can add $X or Y% to tour price
  └─ All commissions based on COMPLETED tours
```

**Critical Safeguards:**

1. **Refund/Cancellation Reversal**
   - Commission finalizes only on tour completion
   - Cancelled bookings auto-reverse commissions
   - Refunds deducted from next payout if already paid

2. **Self-Booking Prevention**
   - Block staff from using own referral code
   - System validation: `StaffId != CodeOwnerId`
   - Suspicious pattern detection

3. **Code Sharing Prevention**
   - Staff codes unique (STAFF-JOHN-001 only for John)
   - Monthly code rotation
   - IP/location anomaly detection

4. **Tiered Commission Gaming**
   - Only COMPLETED bookings count toward tier
   - Minimum booking value threshold
   - Monthly tier reset

5. **Markup Price Validation**
   - Max markup: lower of ($X or Y%)
   - Validation: Reject markups > original + 10%
   - Tenant approval for high markups

6. **Duplicate Booking Detection**
   - Prevent same customer + tour + date
   - Customer warning message
   - Validation at booking time

7. **Departing Staff Policy**
   - Commission locked when staff leaves
   - Configurable: Auto-pay / Hold 30-day escrow / Forfeit
   - Clear audit trail

8. **Refund/Reversal Tracking**
   - Reverse commission on refund
   - If already paid → Deduct next payout
   - Full audit history

9. **Tiered Commission Caps**
   - Max tier commission capped (e.g., 30%)
   - SuperAdmin/Admin approval for changes
   - Quarterly abuse review

10. **Dispute Resolution (Not Direct Override)**
    - SuperAdmin creates dispute ticket
    - Tenant Admin reviews & approves
    - System recalculates automatically
    - Full audit trail (original → disputed → resolved)
    - Emergency force override requires dual-control + audit

**Implementation:**
- Commission finalizes on tour completion (not booking)
- Payout includes only finalized commissions
- Audit log tracks every commission action
- All safeguards enforced in domain layer

**Full Details:** See `ROLE_HIERARCHY_AND_COMMISSION_SYSTEM.md`

---

## 📋 Implementation Conventions

### Naming Conventions

```csharp
// Commands (mutation = verb in past tense)
CreateTenantCommand
UpdatePackageCommand
SyncPackageToTenantCommand
DeleteInquiryCommand
CreateStaffAgentCommand
ConfigureCommissionRuleCommand
CreateBookingCommand
ProcessCommissionPayoutCommand
CreateDisputeTicketCommand

// Queries (read = get/search)
GetTenantByIdQuery
SearchPackagesQuery
GetTenantInquiriesQuery
GetAgentCommissionBalanceQuery
GetTenantCommissionReportsQuery
GetSuperAdminAuditLogsQuery

// Events (when something happened = past tense)
TenantCreatedEvent
PackageSynchronizedEvent
InquiryReceivedEvent
BookingCreatedEvent
BookingCompletedEvent
CommissionAccruedEvent
CommissionPayoutProcessedEvent

// Services (action-oriented)
ITenantResolver
IPackageSyncEngine
IWebsiteProvisioner
INotificationService
ICommissionCalculationEngine
ICommissionPayoutService
IAuditLoggingService
IDisputeResolutionService

// Repositories
ITenantRepository
IPackageRepository
IInquiryRepository
IStaffAgentRepository
IIndependentAgentRepository
ICommissionRuleRepository
ICommissionAccrualRepository
ICommissionPayoutRepository
IDisputeResolutionRepository
IAuditLogRepository
```

### File Organization

```
Feature/UseCase/
  ├── CommandOrQuery.cs           (Request object)
  ├── CommandOrQueryHandler.cs    (Logic)
  ├── CommandOrQueryValidator.cs  (Validation)
  └── CommandOrQueryTests.cs      (Tests)
```

### Database Naming

```sql
-- Tables: PascalCase, no underscore
Tenants
TenantPackages
PackageOverrides
AdminUsers
StaffAgents
IndependentAgents
CommissionRules
CommissionAccruals
CommissionPayouts
CodeUsageAudits
DisputeResolutionTickets
AuditLogs

-- Columns: PascalCase
Id
TenantId
CreatedAt
UpdatedAt
IsActive
AdminRole
AdminScope
AgentType
CommissionAmount
CommissionPercentage
CommissionStatus

-- Stored Procedures: sp_VerbObject
sp_SyncPackageToTenant
sp_GetInactiveSubscriptions
sp_CalculateStaffCommissions
sp_ProcessCommissionPayouts
sp_DetectCommissionAnomalies

-- Indexes: IX_Table_FieldList
IX_Inquiries_TenantId_Status
IX_Packages_Category_Destination
IX_StaffAgents_TenantId_IsActive
IX_CommissionAccruals_AgentId_Status
IX_CommissionAccruals_BookingId
IX_CodeUsageAudits_Code_UsedAt
IX_AuditLogs_SuperAdminId_Timestamp
IX_DisputeResolutionTickets_Status
```

---

## 🎯 Testing Strategy

### Unit Tests (Domain + Application)

```csharp
// Domain entities tested for invariants
public class TenantTests
{
    [Fact]
    public void Create_WithValidData_RaisesTenantCreatedEvent()
    {
        var tenant = Tenant.Create("Travel Pro", new Email("contact@travelpro.com"));
        
        Assert.NotNull(tenant.Id);
        Assert.Single(tenant.DomainEvents);
        Assert.IsType<TenantCreatedEvent>(tenant.DomainEvents.First());
    }
    
    [Fact]
    public void Create_WithEmptyName_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => 
            Tenant.Create("", new Email("test@example.com")));
    }
}

// Commission Safeguard Tests
public class CommissionSafeguardTests
{
    [Fact]
    public async Task CreateBooking_WithStaffOwnCode_ThrowsDomainException()
    {
        // Safeguard 2: Self-booking prevention
        var handler = new CreateBookingCommandHandler(
            _bookingRepo, _staffAgentRepo, _codeRepo);
        
        var command = new CreateBookingCommand
        {
            TourId = _tourId,
            CustomerId = _customerId,
            ReferralCode = "STAFF-JOHN-001" // John's own code
        };
        
        // Should throw when current user is John
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Cannot use own referral code", exception.Message);
    }
    
    [Fact]
    public async Task ProcessRefund_AutomaticallyReversesCommission()
    {
        // Safeguard 1: Refund/cancellation reversal
        var booking = CreateTestBooking(BookingStatus.Completed);
        var commission = CreateTestCommission(booking.Id, CommissionStatus.Accrued);
        
        var handler = new ProcessRefundCommandHandler(_bookingRepo, _commissionRepo);
        var command = new ProcessRefundCommand { BookingId = booking.Id };
        
        await handler.Handle(command, CancellationToken.None);
        
        // Commission should be reversed
        var reversedCommission = await _commissionRepo.GetAsync(
            c => c.SourceId == booking.Id);
        Assert.Equal(CommissionStatus.Reversed, reversedCommission.Status);
    }
    
    [Fact]
    public async Task CreateBooking_WithDuplicateCustomerTourDate_Rejected()
    {
        // Safeguard 6: Duplicate booking detection
        var existingBooking = new Booking 
        { 
            CustomerId = _customerId, 
            TourId = _tourId, 
            TourDate = new DateTime(2026, 07, 15),
            Status = BookingStatus.Confirmed
        };
        await _bookingRepo.AddAsync(existingBooking);
        
        var handler = new CreateBookingCommandHandler(_bookingRepo, _tourRepo);
        var command = new CreateBookingCommand
        {
            CustomerId = _customerId,
            TourId = _tourId,
            TourDate = new DateTime(2026, 07, 15) // Same date
        };
        
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("already has booking", exception.Message);
    }
    
    [Fact]
    public async Task ValidateMarkup_ExceedsMaxAmount_Rejected()
    {
        // Safeguard 5: Markup validation
        var rule = new CommissionRule 
        { 
            MaxMarkupDollars = 50, 
            MaxMarkupPercent = 10,
            ApprovalThreshold = 30
        };
        
        var handler = new ValidateMarkupCommandHandler(_ruleRepo);
        var command = new ValidateMarkupCommand
        {
            ProposedPrice = 600, // $100 markup on $500 tour
            TourBasePrice = 500
        };
        
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("exceeds max", exception.Message);
    }
}

// Application commands tested with mocked repos
public class CreateTenantCommandTests
{
    [Fact]
    public async Task Handle_WithValidRequest_CreatesTenantAndReturnsId()
    {
        var command = new CreateTenantCommand("Travel Pro", "contact@travelpro.com");
        var handler = new CreateTenantCommandHandler(_mockRepository, _mockUnitOfWork);
        
        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tenant>()));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync());
    }
}
```

### Integration Tests (API + Database)

```csharp
public class CreateTenantIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly LitXusTravelDbContext _dbContext;
    
    [Fact]
    public async Task Post_CreateTenant_ReturnsCreatedAndInDatabase()
    {
        var request = new CreateTenantRequest 
        { 
            Name = "Travel Pro", 
            Email = "contact@travelpro.com" 
        };
        
        var response = await _client.PostAsync("/api/v1/admin/tenants", 
            new StringContent(JsonConvert.SerializeObject(request), 
                Encoding.UTF8, "application/json"));
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        // Verify in database
        var tenant = await _dbContext.Tenants
            .FirstOrDefaultAsync(t => t.Name == "Travel Pro");
        Assert.NotNull(tenant);
    }
}
```

---

## 🚀 Deployment Strategy

### Local Development
```
1. Create LitXusTravel_Dev SQL Server database
2. Run: dotnet ef database update
3. Run: dotnet run (API on http://localhost:5000)
4. Next.js: npm run dev (http://localhost:3000)
```

### Staging/Production
```
1. Docker containers for .NET API
2. Azure SQL Database for SQL Server
3. Next.js on Vercel or Azure Static Web Apps
4. GitHub Actions CI/CD pipeline
5. Environment variables via Azure Key Vault
```

---

## 🔒 Security Considerations

### Authentication
- JWT tokens for API
- Tenant ID embedded in token claims
- Token validation on every request

### Authorization
- Tenant context middleware validates requests
- Prevent cross-tenant data access
- Admin-only endpoints require special claims

### Data Protection
- All sensitive data encrypted at rest (SQL Server TDE)
- HTTPS everywhere
- CORS configured per tenant subdomain

---

## 📊 Monitoring & Observability

### Logging
- Serilog for structured logging
- Tenant ID in every log entry
- Error tracking via Application Insights

### Metrics
- Request latency
- Error rates per endpoint
- Package sync performance
- Subscription renewal health

---

## 🔄 Migration Path

**This setup allows evolution to:**

1. **Microservices:** Extract services into separate APIs (no code duplication)
2. **Event Sourcing:** Capture all domain events for audit trail
3. **CQRS Extreme:** Separate read/write databases
4. **Multi-Database:** Separate tenant databases if needed
5. **GraphQL:** Easy to add alongside REST API

---

## 📝 Quick Reference

### Adding a New Feature

```
1. Define domain entity/aggregate in Domain layer
2. Create UseCase command/query in Application
3. Create validator in Application
4. Implement repository in Infrastructure
5. Create API controller in API layer
6. Write unit tests for domain
7. Write integration tests for API
8. Update database schema if needed
```

### Adding a Service

```
1. Define interface in Application/Interfaces
2. Implement in Infrastructure/Services
3. Register in DependencyInjection.cs
4. Inject in handlers where needed
5. Mock in unit tests
```

### Adding Commission Logic

```
1. Define CommissionRule aggregate (if new rule type)
2. Create command handler (e.g., ConfigureCommissionRuleCommand)
3. Add safeguard validation in domain
4. Implement repository methods
5. Add commission calculation logic to ICommissionCalculationEngine
6. Create integration test for full flow
7. Add API endpoint for configuration
8. Include in SuperAdmin audit trail
9. Test fraud prevention scenarios
```

### Adding Role-Based Access

```
1. Define role + scope in AdminUser aggregate
2. Create authorization attribute/policy
3. Add claim validation in middleware
4. Add role checks to handlers
5. Document required claims in API docs
6. Test unauthorized access scenarios
7. Add action to AuditLog
```

---

## 🔐 Commission Fraud Prevention Checklist

When implementing commission features:

- [ ] Commission finalizes on tour COMPLETION, not booking
- [ ] Refunds reverse accrued commissions automatically
- [ ] Self-booking blocked (can't use own code)
- [ ] Code sharing detected (IP/location anomalies)
- [ ] Tiered commissions based on COMPLETED bookings
- [ ] Markup capped (lower of $ or %)
- [ ] Duplicate bookings rejected
- [ ] Departing staff policy enforced
- [ ] Refunds tracked & reversed if already paid
- [ ] Dispute resolution logged with full audit
- [ ] SuperAdmin overrides require dual-control + audit
- [ ] Monthly commission anomaly report generated

---

**This architecture prioritizes:**
- ✅ Testability
- ✅ Maintainability
- ✅ Scalability
- ✅ Domain clarity
- ✅ Independent deployability
- ✅ Framework flexibility
- ✅ Fraud prevention
- ✅ Audit transparency

---

## 📚 Supporting Documents

- **ROLE_HIERARCHY_AND_COMMISSION_SYSTEM.md** — Complete role definitions, agent models, commission rules, and safeguards
- **UAT-WORKFLOW.md** — Testing protocol between LitXusTravel and LitXusDevHub

---

**CLAUDE.MD - Architecture Ready** ✅  
**Role Hierarchy & Commission System** ✅  
**Commission Safeguards Implemented** ✅
