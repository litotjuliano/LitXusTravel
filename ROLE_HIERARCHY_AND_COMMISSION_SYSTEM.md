# Role Hierarchy & Commission System Architecture
**Date:** 2026-06-10  
**Discussion:** LitXusTravel Role Definitions, Agent Models, and Commission Safeguards

---

## Table of Contents
1. [Role Hierarchy](#role-hierarchy)
2. [Agent Types & Models](#agent-types--models)
3. [Commission System](#commission-system)
4. [Commission Safeguards](#commission-safeguards)
5. [Data Model](#data-model)
6. [Implementation Checklist](#implementation-checklist)

---

## Role Hierarchy

### SuperAdmin (Full Platform Control)
**Created by:** System (bootstrap)  
**Managed by:** You (LitXus Owner)

**Permissions:**
- Create other Admin users
- Create global subscription packages
- View all Tenant data (with audit trail)
- View all Agent data
- View all commission data
- Create dispute resolution tickets
- Force override commissions (emergency only, dual-control)
- Oversee all dashboards (read-only)

**Audit Trail:** Every action logged with:
- Who (SuperAdmin user ID)
- What (action taken)
- When (timestamp)
- Where (which Tenant/Agent affected)
- Why (reason/comment)
- Before/After (what changed)
- IP address

---

### Admin (Platform or Tenant Scope)
**Created by:** SuperAdmin only  
**Can create:** Cannot create other Admins

**Two Variants:**

#### Variant A: Platform Admin
- Below SuperAdmin in platform hierarchy
- Manages operational aspects (tenants, subscriptions, system config)
- Views all tenant data (functional areas)
- Cannot delete core configs or override system settings
- Can assign tenants to themselves or other admins
- Audit: SuperAdmin can track all Admin activities

**Token claim:**
```json
{
  "role": "Admin",
  "scope": "Platform"
}
```

#### Variant B: Tenant Admin (Assigned to Tenant)
- Manages single or multiple assigned tenants
- Full control within assigned tenant(s)
- Can create tours, manage packages, pricing, overrides
- Can create/manage tenant staff (internal agents)
- Cannot access other tenants
- Can configure commission rules for their tenants

**Token claim:**
```json
{
  "role": "Admin",
  "scope": "Tenant",
  "tenantId": "guid-here"
}
```

---

## Agent Types & Models

### Agent Type 1: Tenant Staff (Internal Employees)

**Definition:** Internal staff hired by Tenant to manage tours and sales

**Creation:** Created by Tenant Admin (not SuperAdmin/Admin)

**Scope:** Single Tenant only

**Access:** Via Tenant's white-label website (`tenant1.LitXusTravel.com`)

**Capabilities:**
- View/manage assigned Tenant's tours
- Can create tours (if permitted by Tenant Admin)
- Can book customers on tours (generate sales)
- Cannot see other Tenant's data
- Commission: Yes (configurable by Tenant Admin)

**Commission Model:**
```
Tenant Admin configures:
  ├─ Per Tour Booked: Fixed amount or percentage
  ├─ Per Tour Completed: Fixed amount or percentage
  ├─ Per Revenue Generated: Percentage of booking
  ├─ Hybrid: Base salary + commission (optional)
  ├─ Tiered: Volume-based commission rates
  ├─ Payout: Monthly (configurable)
  ├─ Approval: Auto or Manual (configurable)
  └─ Minimum threshold: $X before payout (configurable)
```

---

### Agent Type 2: Independent Agent (Freelancer/Reseller)

**Definition:** Independent agents/resellers who work with multiple tenants

**Creation:** Created by SuperAdmin or Admin

**Scope:** Can work with multiple Tenants (browse catalog, resell)

**Access:** Own white-label website (`agent1.LitXusTravel.com`) + permission to resell Tenant tours

**Capabilities:**
- View available Tenant tour packages
- Resell Tenant tours on own white-label site
- Earn commission on bookings made through own site
- Can markup tours (within Tenant's configured limits)
- Own subscription & white-label website (independent)

**Commission Model:**
```
When reselling Tenant tours:
  ├─ Base Commission: % of booking price
  ├─ Tiered by volume:
  │  ├─ 0-100 bookings/month: 15%
  │  ├─ 101-500 bookings/month: 20%
  │  └─ 500+ bookings/month: 25%
  ├─ Markup allowance (optional):
  │  ├─ Max amount: e.g., $50
  │  ├─ Max percentage: e.g., 10%
  │  └─ Whichever is lower
  ├─ Tenant approval: Required for markups over threshold
  └─ Commission only on verified/completed bookings
```

---

## Commission System

### Staff Sales Attribution (Tour Created by Tenant, Sold by Staff)

```
Scenario: Staff Agent books customer on Tenant's tour

Option 1: Direct Booking (no code)
  Customer visits: tenant1.LitXusTravel.com
  → Staff Agent books them in system
  → Commission applies immediately
  → Tenant: 100% revenue
  → Staff: Commission per rule

Option 2: Code-Based Booking (code attribution)
  Customer uses staff referral code: "STAFF-JOHN-001"
  → Booking attributed to Staff Agent John
  → Commission triggers if no code, staff still gets sale
```

**Staff Code Structure:**
- Format: `STAFF-{FirstName}-{Sequence}` (e.g., `STAFF-JOHN-001`)
- Unique per agent
- Plain text (no short URLs)
- Tenant Admin sees which code was used for each booking
- Can be rotated/changed by Tenant Admin

---

### Freelance Agent Commission (Reselling Tenant Tours)

```
Scenario: Freelance Agent resells Tenant A's tour on own site

Customer visits: agent1.LitXusTravel.com
  → Sees Tenant A's tour (curated)
  → Clicks "Book"
  → Booking auto-attributed to Agent
  → No code needed

Commission Split:
  Booking Price: $500
  Commission Rate: 20% (configured by Tenant A)
  Agent Earnings: $100 (20% of $500)
  Tenant Earnings: $400 (80% of $500)

With Markup (if allowed):
  Booking Price: $550 ($500 + $50 markup)
  Agent Earnings: $100 (commission) + $50 (markup) = $150
  Tenant Earnings: $400 (original $500 base)
```

---

## Commission Safeguards

### Safeguard 1: Refund/Cancellation Reversal

**Problem:** Staff creates booking, gets commission, customer cancels before tour

**Solution:**
- Commission only finalizes on tour completion (not booking creation)
- If booking cancelled before completion → Commission reversed
- Payout includes only completed/finalized commissions
- If refund occurs after payout → Deduct from next payout

**Implementation:**
```csharp
CommissionAccrual status flow:
  Booking Made → "Accrued"
       ↓
  Tour Completed → "Finalized"
       ↓
  Payment Processed → "Paid"
  
If Cancelled/Refunded:
  Any Accrued/Pending → "Reversed"
  Already Paid → "Refunded" (deducted next cycle)
```

---

### Safeguard 2: Self-Booking Prevention

**Problem:** Staff books own tour with own code to earn commission

**Solution:**
- Block self-referrals: Staff cannot use own code
- Validation: Check `BookingAgent.Id != StaffCode.OwnerId`
- Audit: Flag suspicious patterns (same IP, same card)
- Alert: Tenant Admin notified of attempts

**Implementation:**
```csharp
public class CreateBookingCommandHandler
{
    public async Task Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var currentAgent = await GetCurrentAgent(ct);
        
        // Check for self-booking
        if (request.ReferralCode != null)
        {
            var codeOwner = await GetCodeOwner(request.ReferralCode, ct);
            if (codeOwner.Id == currentAgent.Id)
                throw new DomainException("Cannot use own referral code");
        }
        
        // Continue with booking...
    }
}
```

---

### Safeguard 3: Code Sharing Prevention

**Problem:** Staff gives code to friend, friend books, staff gets credit

**Solution:**
- Codes must be personal/unique (JOHN-001 only for John)
- Code rotation: Change codes periodically (monthly recommended)
- IP/Location audit: Track where code is used
- Alert: Unusual geography changes or high-volume usage

**Implementation:**
```csharp
public class TrackCodeUsageCommandHandler
{
    public async Task Handle(TrackCodeUsageCommand request, CancellationToken ct)
    {
        var codeUsage = new CodeUsageAudit
        {
            Code = request.Code,
            UsedAt = DateTime.UtcNow,
            CustomerIp = request.CustomerIp,
            CustomerLocation = request.CustomerLocation,
            BookingId = request.BookingId
        };
        
        await _codeUsageRepository.AddAsync(codeUsage, ct);
        
        // Alert if suspicious
        var recentUsages = await GetRecentCodeUsages(request.Code, ct);
        if (HasAnomalies(recentUsages))
            await _alertingService.NotifyTenantAdmin(
                "Suspicious code usage detected", ct);
    }
}
```

---

### Safeguard 4: Tiered Commission Gaming Prevention

**Problem:** Agent artificially inflates bookings to hit higher tier

**Solution:**
- Only completed/verified bookings count toward tier
- Minimum booking value threshold
- Audit: Review sudden spikes in volume
- Tiered evaluation: Reset monthly, based on completed bookings only

**Implementation:**
```csharp
public class EvaluateAgentTierCommand
{
    // Only count COMPLETED bookings from last 30 days
    var completedBookings = await _bookingRepository
        .GetAsync(b => 
            b.AgentId == agent.Id &&
            b.Status == BookingStatus.Completed &&
            b.CompletedAt >= DateTime.UtcNow.AddDays(-30), ct);
    
    var currentTier = DetermineTier(completedBookings.Count());
    
    // Alert if spike detected
    var previousTier = agent.CurrentCommissionTier;
    if (currentTier > previousTier && currentTier.JumpedBy > 1)
        await _auditService.LogAnomalyAsync(
            $"Agent {agent.Id} jumped {previousTier} → {currentTier}", ct);
}
```

---

### Safeguard 5: Markup Price Validation

**Problem:** Agent marks up $500 tour to $10,000

**Solution:**
- Max markup amount: Fixed dollar limit (e.g., max $50)
- Max markup percentage: 10%
- Whichever is lower
- Tenant approval: Required for markups over threshold
- Validation: Alert if markup > 15% or booking > original + 10%

**Implementation:**
```csharp
public class ValidateMarkupCommand
{
    public async Task Handle(ValidateMarkupCommand request, CancellationToken ct)
    {
        var tour = await _tourRepository.GetByIdAsync(request.TourId, ct);
        var agent = await _agentRepository.GetByIdAsync(request.AgentId, ct);
        var rules = await _tenantRulesRepository
            .GetMarkupRulesAsync(tour.TenantId, ct);
        
        var maxMarkupAmount = rules.MaxMarkupDollars; // e.g., $50
        var maxMarkupPercent = rules.MaxMarkupPercent; // e.g., 10%
        
        var markupAmount = request.ProposedPrice - tour.BasePrice;
        var markupPercent = (markupAmount / tour.BasePrice) * 100;
        
        // Enforce lower limit
        if (markupAmount > maxMarkupAmount)
            throw new DomainException(
                $"Markup exceeds max ${maxMarkupAmount}");
        
        if (markupPercent > maxMarkupPercent)
            throw new DomainException(
                $"Markup exceeds {maxMarkupPercent}%");
        
        // Require approval if over threshold
        if (markupAmount > rules.ApprovalThreshold)
        {
            var approval = await CreateMarkupApprovalRequest(
                tour.TenantId, agent.Id, request.ProposedPrice, ct);
            
            // Wait for approval
            await _approvalService.AwaitApprovalAsync(approval.Id, ct);
        }
    }
}
```

---

### Safeguard 6: Duplicate Booking Detection

**Problem:** Customer books same tour twice; both count as separate sales

**Solution:**
- Dedup logic: Prevent same customer + tour + date combo
- Booking validation: Must have different dates/times
- Customer warning: "You already have this booking"

**Implementation:**
```csharp
public class ValidateBookingAvailabilityCommand
{
    var existingBooking = await _bookingRepository.GetAsync(
        b => b.CustomerId == request.CustomerId &&
             b.TourId == request.TourId &&
             b.TourDate == request.TourDate &&
             b.Status != BookingStatus.Cancelled, ct);
    
    if (existingBooking != null)
        throw new DomainException(
            "Customer already has booking for this tour date");
}
```

---

### Safeguard 7: Departing Staff Commission Policy

**Problem:** Staff leaves; commission accrued but not paid yet

**Solution:**
- Commission finalizes only after tour completion (not booking)
- Clear policy: "Paid only if staff active on payout date"
- Escrow option: Hold commission 30 days after departure
- Audit: Track all departing staff commission payments

**Policy:**
```
Commission Finalization:
  Tour Completed → Commission Finalized
  Staff Leaves → Commission locked
  
  If commission pending payout:
    Option A: Payout automatically (if staff left in good standing)
    Option B: Hold 30 days (escrow period)
    Option C: Forfeit (if termination for cause)
    
  Policy configured by Tenant Admin
```

---

### Safeguard 8: Refund/Reversal Tracking

**Problem:** Refund occurs after payout; no clear mechanism to reverse

**Solution:**
- Refund creates reverse commission accrual
- If already paid → Deducted from next payout
- Audit trail shows original commission + reversal
- Agent can dispute refunds (via dispute resolution)

**Implementation:**
```csharp
public class ProcessRefundCommand
{
    // Refund the booking
    booking.Status = BookingStatus.Refunded;
    
    // Reverse commission
    var commissionToReverse = await _commissionRepository
        .GetAsync(c => c.BookingId == booking.Id, ct);
    
    if (commissionToReverse != null)
    {
        if (commissionToReverse.Status == CommissionStatus.Paid)
        {
            // Create reverse accrual for next payout
            var reversal = CommissionAccrual.CreateReversal(
                commissionToReverse, "Booking Refunded");
            await _commissionRepository.AddAsync(reversal, ct);
        }
        else
        {
            // Not yet paid, just cancel
            commissionToReverse.Status = CommissionStatus.Cancelled;
            await _commissionRepository.UpdateAsync(
                commissionToReverse, ct);
        }
    }
}
```

---

### Safeguard 9: Tiered Commission Abuse Prevention

**Problem:** Agent creates tiered rule that advantages them unfairly

**Solution:**
- SuperAdmin/Admin approval required for tiered rules
- Audit: Track who created rule and when
- Review: Quarterly review of abuse patterns
- Cap: Max tier commission capped (e.g., max 30%)

---

### Safeguard 10: Dispute Resolution Workflow

**Why not direct SuperAdmin override:**
- Breaks audit trail
- Can't trace root cause
- Enables abuse

**Better approach:**

```
SuperAdmin identifies issue
    ↓
Creates Dispute Ticket:
  ├─ Description
  ├─ Proposed fix
  ├─ Reason code
  └─ Sends to Tenant Admin
    ↓
Tenant Admin reviews → Approves/Rejects
    ↓
If approved: System recalculates automatically
    ↓
Audit shows:
  ├─ Original calculation
  ├─ Dispute ticket #
  ├─ Justification
  └─ New calculation

Emergency Force Override:
  ├─ Mandatory comment/reason
  ├─ Email notification to Tenant
  ├─ Flagged as "Force Override" in audit
  └─ Requires approval from another SuperAdmin (dual-control)
```

---

## Data Model

### Core Entities

```csharp
// Admin User
public class AdminUser : AggregateRoot
{
    public AdminUserId Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public AdminRole Role { get; private set; } // SuperAdmin, Admin
    public AdminScope Scope { get; private set; } // Platform, Tenant
    public Guid? AssignedTenantId { get; private set; } // if Tenant scope
    public bool IsActive { get; private set; }
    public List<Guid> ManagedTenantIds { get; private set; } // which tenants they manage
}

// Tenant Staff Agent
public class StaffAgent : AggregateRoot
{
    public StaffAgentId Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string UniqueCode { get; private set; } // STAFF-JOHN-001
    public DateTime CodeIssuedAt { get; private set; }
    public DateTime? CodeExpiresAt { get; private set; } // for rotation
    public bool IsActive { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? DepartedAt { get; private set; }
}

// Independent Agent
public class IndependentAgent : AggregateRoot
{
    public IndependentAgentId Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public SubscriptionPlan CurrentSubscription { get; private set; }
    public string WhiteLabelDomain { get; private set; } // agent1.LitXusTravel.com
    public List<Guid> TenantIdsAuthorized { get; private set; } // which tenants they can resell
    public bool IsActive { get; private set; }
}

// Commission Rule
public class CommissionRule : AggregateRoot
{
    public CommissionRuleId Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid? AgentId { get; private set; } // if specific agent
    public CommissionTrigger Trigger { get; private set; } // BookingCreated, TourCompleted, RevenueGenerated
    public decimal Amount { get; private set; } // $ or %
    public bool IsPercentage { get; private set; }
    public List<TieredCommission> TieredRates { get; private set; }
    public CommissionFrequency PayoutFrequency { get; private set; } // Monthly
    public bool AutoApprove { get; private set; } // Auto or Manual
    public decimal MinimumThreshold { get; private set; } // Min $ to trigger payout
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }
}

// Commission Accrual
public class CommissionAccrual : AggregateRoot
{
    public CommissionAccrualId Id { get; private set; }
    public Guid AgentId { get; private set; } // StaffAgent or IndependentAgent
    public Guid TenantId { get; private set; }
    public Guid CommissionRuleId { get; private set; }
    public CommissionTrigger TriggerType { get; private set; }
    public Guid SourceId { get; private set; } // BookingId, TourId, etc.
    public decimal CommissionAmount { get; private set; }
    public decimal? CommissionPercentage { get; private set; }
    public decimal BaseAmount { get; private set; } // tour price, booking price, etc.
    public CommissionStatus Status { get; private set; } // Accrued, Pending, Paid, Reversed, Cancelled
    public DateTime AccruedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Guid? PayoutId { get; private set; } // Reference to payout batch
    public List<AuditEntry> AuditLog { get; private set; }
    public string? DisputeTicketId { get; private set; } // if under dispute
}

// Code Usage Audit
public class CodeUsageAudit : Entity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public DateTime UsedAt { get; private set; }
    public string CustomerIp { get; private set; }
    public string? CustomerLocation { get; private set; }
    public Guid BookingId { get; private set; }
    public Guid? StaffAgentId { get; private set; }
    public Guid TenantId { get; private set; }
}

// Commission Payout
public class CommissionPayout : AggregateRoot
{
    public CommissionPayoutId Id { get; private set; }
    public Guid AgentId { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime PayoutPeriodStart { get; private set; }
    public DateTime PayoutPeriodEnd { get; private set; }
    public List<Guid> CommissionAccrualIds { get; private set; }
    public decimal TotalAmount { get; private set; }
    public PayoutStatus Status { get; private set; } // Pending, Approved, Processed, Failed
    public DateTime ProcessedAt { get; private set; }
    public string? TransactionId { get; private set; } // bank transfer reference
}

// Dispute Resolution Ticket
public class DisputeResolutionTicket : AggregateRoot
{
    public DisputeTicketId Id { get; private set; }
    public Guid SuperAdminId { get; private set; }
    public Guid CommissionAccrualId { get; private set; }
    public string Description { get; private set; }
    public string ProposedFix { get; private set; }
    public DisputeReasonCode ReasonCode { get; private set; } // Bug, Miscalculation, etc.
    public DisputeStatus Status { get; private set; } // Open, Pending Review, Approved, Rejected
    public Guid? ReviewedByTenantAdminId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public decimal? OriginalAmount { get; private set; }
    public decimal? AdjustedAmount { get; private set; }
}

// Audit Log (all SuperAdmin actions)
public class AuditLog : Entity
{
    public Guid Id { get; private set; }
    public Guid SuperAdminId { get; private set; }
    public AuditAction Action { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? AffectedEntityType { get; private set; }
    public Guid? AffectedEntityId { get; private set; }
    public Guid? AffectedTenantId { get; private set; }
    public Guid? AffectedAgentId { get; private set; }
    public string? Reason { get; private set; }
    public string? BeforeState { get; private set; } // JSON
    public string? AfterState { get; private set; } // JSON
    public string ClientIp { get; private set; }
}
```

---

## Implementation Checklist

### Phase 1: Core Entities & Repositories
- [ ] Create AdminUser aggregate (SuperAdmin, Admin roles)
- [ ] Create StaffAgent aggregate (per-tenant staff)
- [ ] Create IndependentAgent aggregate (freelancer model)
- [ ] Create CommissionRule aggregate
- [ ] Create CommissionAccrual aggregate
- [ ] Create CodeUsageAudit entity
- [ ] Create CommissionPayout aggregate
- [ ] Create DisputeResolutionTicket aggregate
- [ ] Create AuditLog entity
- [ ] Create repositories for all entities
- [ ] Create unit of work

### Phase 2: Commission Safeguards
- [ ] Implement Safeguard 1: Refund/Cancellation Reversal
- [ ] Implement Safeguard 2: Self-Booking Prevention
- [ ] Implement Safeguard 3: Code Sharing Prevention & Audit
- [ ] Implement Safeguard 4: Tiered Commission Gaming Detection
- [ ] Implement Safeguard 5: Markup Price Validation
- [ ] Implement Safeguard 6: Duplicate Booking Detection
- [ ] Implement Safeguard 7: Departing Staff Policy
- [ ] Implement Safeguard 8: Refund/Reversal Tracking
- [ ] Implement Safeguard 9: Tiered Commission Cap
- [ ] Implement Safeguard 10: Dispute Resolution Workflow

### Phase 3: Use Cases (Commands/Queries)
- [ ] CreateAdminCommand & handler
- [ ] AssignTenantToAdminCommand
- [ ] CreateStaffAgentCommand
- [ ] RotateStaffCodeCommand
- [ ] CreateIndependentAgentCommand
- [ ] ConfigureCommissionRuleCommand
- [ ] CreateBookingCommand (with safeguards)
- [ ] CompleteBookingCommand (finalize commission)
- [ ] ProcessRefundCommand (reverse commission)
- [ ] ProcessCommissionPayoutCommand
- [ ] CreateDisputeTicketCommand
- [ ] ResolveDisputeCommand
- [ ] GetAgentCommissionBalanceQuery
- [ ] GetTenantCommissionReportsQuery
- [ ] GetSuperAdminAuditLogsQuery

### Phase 4: API Endpoints
- [ ] POST /api/v1/admin/admins (SuperAdmin only)
- [ ] POST /api/v1/admin/admins/{id}/assign-tenant
- [ ] POST /api/v1/tenants/{tenantId}/staff-agents
- [ ] POST /api/v1/tenants/{tenantId}/staff-agents/{id}/rotate-code
- [ ] GET /api/v1/staff-agents/me/commission-balance
- [ ] GET /api/v1/tenants/{tenantId}/commission-reports
- [ ] POST /api/v1/admin/disputes (SuperAdmin creates)
- [ ] GET /api/v1/admin/audit-logs (SuperAdmin only)

### Phase 5: Validation & Testing
- [ ] Unit tests for all commission safeguards
- [ ] Integration tests for commission accrual flow
- [ ] Integration tests for dispute resolution workflow
- [ ] Integration tests for audit logging
- [ ] Load test: Commission payout at scale
- [ ] Security test: Try to exploit loopholes

### Phase 6: Dashboards & Reporting
- [ ] Agent commission dashboard (balance, history, pending)
- [ ] Tenant commission reports (per-agent metrics)
- [ ] SuperAdmin audit dashboard
- [ ] Commission anomaly detection reports

---

## Key Business Rules

1. **Commission only on completed tours** (not bookings)
2. **Staff codes are personal and unique** (STAFF-JOHN-001)
3. **Refunds reverse commissions** automatically
4. **Self-booking is blocked** by system
5. **Markups capped** at fixed amount or percentage (whichever is lower)
6. **Dispute resolution** required for SuperAdmin overrides
7. **All actions audited** with full context
8. **Tiered commissions** based on completed bookings only
9. **Departing staff** commission policy defined by Tenant
10. **Independent agents** can resell multiple Tenant tours

---

**Document Status:** Complete  
**Ready for Implementation:** Yes  
**Review Date:** 2026-06-10
