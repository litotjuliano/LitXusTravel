# LitXusTravel Implementation Checklist

**Date:** 2026-06-11  
**Status:** Ready for Development  
**Owner:** Development Team

---

## 📦 Deliverables Summary

### Documentation (Completed ✓)

**Architecture & Design:**
- [x] CLAUDE.md (Sections 10-11: Role Hierarchy & Commission System)
- [x] ROLE_HIERARCHY_AND_COMMISSION_SYSTEM.md (comprehensive reference)
- [x] 8 Word documents with diagrams in `Documentation/` folder

**UAT & Testing:**
- [x] Seed-Data.sql (comprehensive test data)
- [x] UAT-1-Role-Hierarchy.md (8 test cases)
- [x] UAT-2-Agent-Models.md (10 test cases)
- [x] UAT-3-Commission-Safeguards.md (30 test cases covering 10 safeguards)
- [x] UAT-4-Commission-Workflows.md (10 test cases)
- [x] UAT-README.md (testing guide)

**Total:** 15+ documents ready

---

## 🔧 Development Phases

### PHASE 1: Core Entities & Repositories (Week 1)

**Database Schema:**
```sql
CREATE TABLE AdminUsers (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  Name NVARCHAR(255) NOT NULL,
  Email NVARCHAR(255) NOT NULL UNIQUE,
  Role NVARCHAR(50) NOT NULL, -- SuperAdmin, Admin
  Scope NVARCHAR(50) NOT NULL, -- Platform, Tenant
  AssignedTenantId UNIQUEIDENTIFIER,
  ManagedTenantIds NVARCHAR(MAX), -- JSON array
  IsActive BIT NOT NULL,
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2,
  CONSTRAINT FK_AdminUsers_Tenants 
    FOREIGN KEY(AssignedTenantId) REFERENCES Tenants(Id)
);

CREATE TABLE StaffAgents (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  TenantId UNIQUEIDENTIFIER NOT NULL,
  Name NVARCHAR(255) NOT NULL,
  Email NVARCHAR(255) NOT NULL,
  UniqueCode NVARCHAR(50) NOT NULL UNIQUE,
  CodeIssuedAt DATETIME2 NOT NULL,
  CodeExpiresAt DATETIME2,
  IsActive BIT NOT NULL,
  JoinedAt DATETIME2 NOT NULL,
  DepartedAt DATETIME2,
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2,
  CONSTRAINT FK_StaffAgents_Tenants 
    FOREIGN KEY(TenantId) REFERENCES Tenants(Id),
  CONSTRAINT IX_StaffAgents_TenantId_IsActive 
    UNIQUE(TenantId, IsActive)
);

CREATE TABLE IndependentAgents (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  Name NVARCHAR(255) NOT NULL,
  Email NVARCHAR(255) NOT NULL UNIQUE,
  SubscriptionTier NVARCHAR(50) NOT NULL,
  WhiteLabelDomain NVARCHAR(255) NOT NULL UNIQUE,
  IsActive BIT NOT NULL,
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2
);

CREATE TABLE CommissionRules (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  TenantId UNIQUEIDENTIFIER NOT NULL,
  AgentId UNIQUEIDENTIFIER,
  Trigger NVARCHAR(50) NOT NULL, -- TourBooked, TourCompleted
  Amount DECIMAL(10,2) NOT NULL,
  IsPercentage BIT NOT NULL,
  PayoutFrequency NVARCHAR(50) NOT NULL, -- Monthly
  AutoApprove BIT NOT NULL,
  MinimumThreshold DECIMAL(10,2) NOT NULL,
  EffectiveFrom DATETIME2 NOT NULL,
  EffectiveTo DATETIME2,
  IsActive BIT NOT NULL,
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2,
  CONSTRAINT FK_CommissionRules_Tenants 
    FOREIGN KEY(TenantId) REFERENCES Tenants(Id)
);

CREATE TABLE CommissionAccruals (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  AgentId UNIQUEIDENTIFIER NOT NULL,
  TenantId UNIQUEIDENTIFIER NOT NULL,
  CommissionRuleId UNIQUEIDENTIFIER NOT NULL,
  TriggerType NVARCHAR(50) NOT NULL,
  SourceId UNIQUEIDENTIFIER NOT NULL, -- BookingId
  CommissionAmount DECIMAL(10,2) NOT NULL,
  CommissionPercentage DECIMAL(5,2),
  BaseAmount DECIMAL(10,2) NOT NULL,
  Status NVARCHAR(50) NOT NULL, -- Accrued, Finalized, Paid, Reversed, Cancelled
  AccruedAt DATETIME2 NOT NULL,
  PaidAt DATETIME2,
  PayoutId UNIQUEIDENTIFIER,
  DisputeTicketId UNIQUEIDENTIFIER,
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2,
  CONSTRAINT FK_CommissionAccruals_Tenants 
    FOREIGN KEY(TenantId) REFERENCES Tenants(Id),
  CONSTRAINT IX_CommissionAccruals_AgentId_Status 
    UNIQUE(AgentId, Status)
);

CREATE TABLE CommissionPayouts (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  AgentId UNIQUEIDENTIFIER,
  TenantId UNIQUEIDENTIFIER NOT NULL,
  PayoutPeriodStart DATETIME2 NOT NULL,
  PayoutPeriodEnd DATETIME2 NOT NULL,
  CommissionAccrualIds NVARCHAR(MAX), -- JSON array of IDs
  TotalAmount DECIMAL(10,2) NOT NULL,
  Status NVARCHAR(50) NOT NULL, -- Pending, Approved, Processed, Failed
  ProcessedAt DATETIME2,
  TransactionId NVARCHAR(255),
  CreatedAt DATETIME2 NOT NULL,
  UpdatedAt DATETIME2
);

CREATE TABLE CodeUsageAudits (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  Code NVARCHAR(50) NOT NULL,
  UsedAt DATETIME2 NOT NULL,
  CustomerIp NVARCHAR(50),
  CustomerLocation NVARCHAR(255),
  BookingId UNIQUEIDENTIFIER NOT NULL,
  StaffAgentId UNIQUEIDENTIFIER,
  TenantId UNIQUEIDENTIFIER NOT NULL,
  CreatedAt DATETIME2 NOT NULL,
  CONSTRAINT IX_CodeUsageAudits_Code_UsedAt 
    UNIQUE(Code, UsedAt)
);

CREATE TABLE DisputeResolutionTickets (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  SuperAdminId UNIQUEIDENTIFIER NOT NULL,
  CommissionAccrualId UNIQUEIDENTIFIER NOT NULL,
  Description NVARCHAR(MAX) NOT NULL,
  ProposedFix NVARCHAR(MAX) NOT NULL,
  ReasonCode NVARCHAR(50) NOT NULL,
  Status NVARCHAR(50) NOT NULL, -- Open, Pending, Approved, Rejected
  ReviewedByTenantAdminId UNIQUEIDENTIFIER,
  CreatedAt DATETIME2 NOT NULL,
  ResolvedAt DATETIME2,
  OriginalAmount DECIMAL(10,2),
  AdjustedAmount DECIMAL(10,2),
  UpdatedAt DATETIME2
);

CREATE TABLE AuditLogs (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  SuperAdminId UNIQUEIDENTIFIER NOT NULL,
  Action NVARCHAR(255) NOT NULL,
  Timestamp DATETIME2 NOT NULL,
  AffectedEntityType NVARCHAR(50),
  AffectedEntityId UNIQUEIDENTIFIER,
  AffectedTenantId UNIQUEIDENTIFIER,
  AffectedAgentId UNIQUEIDENTIFIER,
  Reason NVARCHAR(MAX),
  BeforeState NVARCHAR(MAX), -- JSON
  AfterState NVARCHAR(MAX), -- JSON
  ClientIp NVARCHAR(50),
  CreatedAt DATETIME2 NOT NULL,
  CONSTRAINT IX_AuditLogs_SuperAdminId_Timestamp 
    UNIQUE(SuperAdminId, Timestamp)
);
```

**Domain Entities to Create:**
- [ ] `AdminUser` aggregate root
- [ ] `StaffAgent` aggregate root
- [ ] `IndependentAgent` aggregate root
- [ ] `CommissionRule` value object
- [ ] `CommissionAccrual` entity
- [ ] `CommissionPayout` entity
- [ ] `DisputeResolutionTicket` entity
- [ ] `AuditLog` entity

**Repositories:**
- [ ] `IAdminUserRepository`
- [ ] `IStaffAgentRepository`
- [ ] `IIndependentAgentRepository`
- [ ] `ICommissionRuleRepository`
- [ ] `ICommissionAccrualRepository`
- [ ] `ICommissionPayoutRepository`
- [ ] `IDisputeResolutionRepository`
- [ ] `IAuditLogRepository`

**Tests:**
- [ ] Domain entity tests (invariants)
- [ ] Repository integration tests
- [ ] Unit tests for all entities

**Estimated Time:** 3-4 days

---

### PHASE 2: Commission Safeguards (Week 2)

**Implement 10 Safeguards:**

- [ ] **Safeguard 1:** Refund/Cancellation Reversal
  - Commission finalizes only on completion
  - Cancelled bookings reverse commission
  - Refunds after payout deduct from next payout
  
- [ ] **Safeguard 2:** Self-Booking Prevention
  - Block staff from using own code
  - Validate: StaffId != CodeOwnerId
  - Audit suspicious attempts

- [ ] **Safeguard 3:** Code Sharing Prevention
  - Track code usage (IP, location, time)
  - Alert on geographic impossibilities
  - Detect unusual volume patterns

- [ ] **Safeguard 4:** Tiered Commission Gaming
  - Only COMPLETED bookings count toward tier
  - Minimum booking value threshold
  - Monthly tier reset

- [ ] **Safeguard 5:** Markup Price Validation
  - Enforce max markup cap ($ and %)
  - Customer sees original + markup breakdown
  - Require approval for high markups

- [ ] **Safeguard 6:** Duplicate Booking Detection
  - Prevent same customer + tour + date
  - Allow different dates for same tour
  - User-friendly error messaging

- [ ] **Safeguard 7:** Departing Staff Policy
  - Commission locked when staff leaves
  - Configurable policy: Auto-pay/Escrow/Forfeit
  - Clear audit trail

- [ ] **Safeguard 8:** Refund Reversal Tracking
  - Reverse commission on refund
  - Deduct from next payout if already paid
  - Full audit history

- [ ] **Safeguard 9:** Tiered Commission Caps
  - Max commission rate enforced (e.g., 30%)
  - Approval required for tier changes
  - Quarterly abuse review

- [ ] **Safeguard 10:** Dispute Resolution
  - SuperAdmin creates dispute ticket
  - Tenant Admin reviews & approves
  - System recalculates automatically
  - Emergency force override (dual-control)

**Validation Layer:**
- [ ] Commission calculation validation
- [ ] Code usage validation
- [ ] Markup validation
- [ ] Tier calculation validation

**Tests:**
- [ ] Unit tests for each safeguard
- [ ] Integration tests for fraud scenarios
- [ ] Edge case testing

**Estimated Time:** 4-5 days

---

### PHASE 3: Use Cases (Commands & Queries) (Week 3)

**Commands:**

- [ ] `CreateAdminCommand` + Handler + Validator
- [ ] `AssignTenantToAdminCommand` + Handler
- [ ] `CreateStaffAgentCommand` + Handler + Validator
- [ ] `RotateStaffCodeCommand` + Handler
- [ ] `CreateIndependentAgentCommand` + Handler
- [ ] `ConfigureCommissionRuleCommand` + Handler + Validator
- [ ] `CreateBookingCommand` + Handler (with safeguards)
- [ ] `CompleteBookingCommand` + Handler (finalize commission)
- [ ] `ProcessRefundCommand` + Handler (reverse commission)
- [ ] `ProcessCommissionPayoutCommand` + Handler
- [ ] `CreateDisputeTicketCommand` + Handler
- [ ] `ResolveDisputeCommand` + Handler
- [ ] `RotateCodeCommand` + Handler
- [ ] `AuthorizeFreelanceAgentCommand` + Handler

**Queries:**

- [ ] `GetAgentCommissionBalanceQuery`
- [ ] `GetTenantCommissionReportsQuery`
- [ ] `GetSuperAdminAuditLogsQuery`
- [ ] `GetCommissionHistoryQuery`
- [ ] `GetPayoutDetailsQuery`
- [ ] `GetDisputeHistoryQuery`

**Behaviors (Pipeline):**

- [ ] Validation behavior
- [ ] Logging behavior
- [ ] Performance monitoring behavior
- [ ] Audit trail behavior

**Tests:**
- [ ] Command handler tests (happy path)
- [ ] Error scenario tests
- [ ] Validation tests
- [ ] Integration tests

**Estimated Time:** 3-4 days

---

### PHASE 4: API Endpoints (Week 3-4)

**Admin Management Endpoints:**

```
POST   /api/v1/admin/admins              → CreateAdmin
PUT    /api/v1/admin/admins/{id}         → UpdateAdmin
DELETE /api/v1/admin/admins/{id}         → DeactivateAdmin
POST   /api/v1/admin/admins/{id}/assign-tenant → AssignTenant
GET    /api/v1/admin/audit-logs          → GetAuditLogs
```

**Agent Management Endpoints:**

```
POST   /api/v1/staff-agents              → CreateStaffAgent
GET    /api/v1/staff-agents/{id}         → GetStaffAgent
PUT    /api/v1/staff-agents/{id}         → UpdateStaffAgent
POST   /api/v1/staff-agents/{id}/rotate-code → RotateCode

POST   /api/v1/independent-agents        → CreateIndependentAgent
GET    /api/v1/independent-agents/{id}   → GetIndependentAgent
```

**Commission Configuration Endpoints:**

```
POST   /api/v1/commission-rules          → CreateCommissionRule
GET    /api/v1/commission-rules          → ListCommissionRules
PUT    /api/v1/commission-rules/{id}     → UpdateCommissionRule
POST   /api/v1/freelance-agents/{id}/authorize → AuthorizeAgent
```

**Commission Query Endpoints:**

```
GET    /api/v1/staff-agents/me/commission-balance → GetMyCommission
GET    /api/v1/tenants/{id}/commission-reports   → GetTenantReport
GET    /api/v1/admin/commission-summary          → GetPlatformSummary
```

**Dispute Management Endpoints:**

```
POST   /api/v1/disputes                  → CreateDisputeTicket
GET    /api/v1/disputes/{id}             → GetDisputeTicket
PUT    /api/v1/disputes/{id}/resolve     → ResolveDispute
```

**Tests:**
- [ ] Integration tests (API + Database)
- [ ] Authorization tests
- [ ] Error response tests
- [ ] Data validation tests

**Estimated Time:** 2-3 days

---

### PHASE 5: Testing & Validation (Week 4)

**Unit Tests:**
- [ ] All domain entities
- [ ] All validators
- [ ] All calculations
- [ ] Target: 80%+ code coverage

**Integration Tests:**
- [ ] API + Database flows
- [ ] Commission workflows end-to-end
- [ ] Refund scenarios
- [ ] Payout processing

**Security Tests:**
- [ ] Authorization enforcement
- [ ] Tenant isolation
- [ ] Fraud prevention
- [ ] Code sharing attempts

**Performance Tests:**
- [ ] Commission calculation at scale
- [ ] Payout processing (1000+ agents)
- [ ] Query performance
- [ ] Audit log queries

**UAT Execution:**
- [ ] Load Seed-Data.sql
- [ ] Execute UAT-1 (Role Hierarchy) → All pass?
- [ ] Execute UAT-2 (Agent Models) → All pass?
- [ ] Execute UAT-3 (Safeguards) → All critical pass?
- [ ] Execute UAT-4 (Workflows) → All pass?
- [ ] Document all issues
- [ ] Get sign-offs

**Estimated Time:** 3-4 days

---

### PHASE 6: Dashboards & Reporting (Week 4-5)

**Agent Commission Dashboard:**
- [ ] My Commission Balance (current month)
- [ ] Commission History (past 12 months)
- [ ] Finalized vs. Pending breakdown
- [ ] Next payout date
- [ ] Commission by booking (details)

**Tenant Commission Reports:**
- [ ] Staff commission summary
- [ ] Per-agent breakdown
- [ ] Freelance agent performance
- [ ] Monthly trend chart
- [ ] Export to PDF/CSV

**SuperAdmin Audit Dashboard:**
- [ ] Audit log viewer (all actions)
- [ ] Filter by: User, Action, Date, Tenant
- [ ] Anomaly detector
- [ ] Commission variance reports
- [ ] Dispute resolution tracker

**Tests:**
- [ ] Dashboard load tests
- [ ] Report accuracy tests
- [ ] Export functionality tests

**Estimated Time:** 2-3 days

---

## 📋 Total Implementation Timeline

| Phase | Days | Total |
|-------|------|-------|
| Phase 1: Entities & Repos | 3-4 | 3-4 |
| Phase 2: Safeguards | 4-5 | 7-9 |
| Phase 3: Use Cases | 3-4 | 10-13 |
| Phase 4: API | 2-3 | 12-16 |
| Phase 5: Testing | 3-4 | 15-20 |
| Phase 6: Dashboards | 2-3 | 17-23 |

**Total: 4-5 weeks (20-23 days)**

---

## 🎯 Pre-Implementation Checklist

Before starting development:

- [ ] All team members read CLAUDE.md (Sections 10-11)
- [ ] Architect reviews ROLE_HIERARCHY_AND_COMMISSION_SYSTEM.md
- [ ] Database team reviews schema design
- [ ] QA team reviews UAT documentation
- [ ] All infrastructure ready (dev, staging, prod)
- [ ] Git repository set up and branches created
- [ ] CI/CD pipeline configured
- [ ] Code review process defined
- [ ] Testing standards established
- [ ] Deployment checklist prepared

---

## 🔍 Code Quality Standards

**For All Code:**
- [ ] Clean Architecture principles followed
- [ ] Domain logic in Domain layer (no anemic models)
- [ ] CQRS pattern for commands/queries
- [ ] Proper dependency injection
- [ ] No direct database queries in handlers
- [ ] Error handling with Result pattern

**For Safeguards Specifically:**
- [ ] All validation in domain layer
- [ ] No bypassing safeguards
- [ ] Comprehensive error messages
- [ ] Audit trail for all safeguard violations
- [ ] Performance: <100ms for calculation

**Code Review Checklist:**
- [ ] Architecture patterns followed
- [ ] No security vulnerabilities
- [ ] No hardcoded values
- [ ] Proper error handling
- [ ] Tests included and passing
- [ ] Documentation complete

---

## 🚀 Deployment Checklist

**Before Deploying to Staging:**
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Code review approved
- [ ] Database migrations tested
- [ ] Rollback plan documented

**Before Deploying to Production:**
- [ ] UAT pass with sign-offs
- [ ] Staging deployment successful
- [ ] Performance testing complete
- [ ] Security review approved
- [ ] Customer communication ready
- [ ] Support team trained
- [ ] Monitoring/alerting configured

---

## 📊 Success Criteria

Implementation is successful when:

✅ All code committed and reviewed  
✅ All tests passing (80%+ coverage)  
✅ UAT complete with zero critical failures  
✅ Safeguards verified working  
✅ Dashboards functional  
✅ Performance acceptable  
✅ Documentation complete  
✅ Team trained  
✅ All sign-offs obtained  
✅ Deployed to production  

---

## 📞 Questions & Support

**Architecture Questions:** 
- Review: ROLE_HIERARCHY_AND_COMMISSION_SYSTEM.md
- Ask: Architect/Lead

**Implementation Questions:**
- Review: CLAUDE.md (Sections 10-11)
- Ask: Development Lead

**Testing Questions:**
- Review: UAT-README.md
- Ask: QA Lead

**Security Concerns:**
- Review: Safeguard descriptions
- Ask: Security Lead

---

**Document Status:** Final  
**Last Updated:** 2026-06-11  
**Next Update:** After Phase 1 completion

