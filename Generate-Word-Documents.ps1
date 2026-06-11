# Generate Professional Word Documents with Diagrams
# Simple, reliable approach using Word COM objects

$OutputPath = "c:\LitXus Systems\LitXusTravel\Documentation"

if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Create Word app
$word = New-Object -ComObject Word.Application
$word.Visible = $false

function Create-Document {
    param($title, $filename, $content)

    Write-Host "Creating: $filename..."

    $doc = $word.Documents.Add()
    $selection = $word.Selection

    # Add title
    $selection.Font.Name = "Calibri"
    $selection.Font.Size = 28
    $selection.Font.Bold = $true
    $selection.TypeText($title)
    $selection.TypeParagraph()

    # Add subtitle
    $selection.Font.Size = 11
    $selection.Font.Bold = $false
    $selection.Font.Italic = $true
    $selection.TypeText("Date: 2026-06-10`n")
    $selection.TypeParagraph()

    # Add content
    $selection.Font.Size = 11
    $selection.Font.Bold = $false
    $selection.Font.Italic = $false
    $selection.TypeText($content)

    # Save (12 = wdFormatDocx)
    $doc.SaveAs("$OutputPath\$filename", 12)
    $doc.Close($false)

    Write-Host "✓ Created: $filename"
}

# ============================================================================
# Document 1: Role Hierarchy
# ============================================================================

$content1 = @"
ROLE HIERARCHY OVERVIEW

SuperAdmin (Full Platform Control)
├─ Create other Admins
├─ Create global subscription packages
├─ Oversee all dashboards (read-only)
├─ Audit all Admin activities
└─ Dispute resolution authority


Admin - Variant A: Platform Admin
├─ Manage operational aspects
├─ Assign/manage tenants
├─ View all tenant data (functional areas)
├─ Cannot delete core configs
└─ Cannot override system settings


Admin - Variant B: Tenant Admin (assigned to 1+ tenants)
├─ Create/manage tenants
├─ Manage packages & pricing
├─ Create staff agents
└─ Configure commission rules


StaffAgent (Internal Employees)
├─ Scoped to single tenant
├─ Create/manage tours
├─ Book customers on tours
├─ Earn commissions on sales
└─ Access: tenant1.LitXusTravel.com


IndependentAgent (Freelancers)
├─ Work with multiple tenants
├─ Resell tenant tours
├─ Own subscription & white-label site
├─ Earn commission on bookings
└─ Access: agent1.LitXusTravel.com


SUPERADMIN PERMISSIONS

Permissions:
├─ Create Admin users (both variants)
├─ Create global subscription packages
├─ View all Tenant data (with audit)
├─ View all Agent data
├─ View all Commission data
├─ Create dispute resolution tickets
├─ Force override commissions (emergency + dual-control)
└─ Oversee all dashboards (read-only)

Audit Trail (Every Action Logged):
├─ Who (SuperAdmin user ID)
├─ What (action taken)
├─ When (timestamp)
├─ Where (which Tenant/Agent)
├─ Why (reason/comment)
├─ Before/After (state change)
└─ IP address


PLATFORM ADMIN RESPONSIBILITIES

Manages operational aspects:
├─ Assign and manage tenants
├─ View all tenant data (functional areas)
├─ Create global packages (recommended task)
├─ Support tenant operations
├─ Cannot delete core system configs
└─ Cannot override system settings

All actions audited by SuperAdmin


TENANT ADMIN RESPONSIBILITIES

Full control within assigned tenant(s):
├─ Create and manage tours
├─ Configure pricing & overrides
├─ Create and manage staff agents
├─ Configure commission rules
├─ View commission reports (own tenant only)
└─ Cannot access other tenants

Token Claims:
{
  "role": "Admin",
  "scope": "Tenant",
  "tenantId": "guid-here"
}
"@

Create-Document "Role Hierarchy & Authorization" "1-Role-Hierarchy.docx" $content1

# ============================================================================
# Document 2: Agent Models
# ============================================================================

$content2 = @"
AGENT TYPE 1: TENANT STAFF (INTERNAL EMPLOYEES)

Definition:     Internal staff hired by Tenant to manage tours
Creation:       Created by Tenant Admin (NOT SuperAdmin/Admin)
Scope:          Single Tenant only
Access:         Via Tenant's white-label: tenant1.LitXusTravel.com

Capabilities:
├─ View/manage assigned Tenant's tours
├─ Create tours (if permitted)
├─ Book customers on tours
├─ Generate sales & earn commissions
├─ Cannot see other Tenant's data
└─ Cannot create other agents


STAFF AGENT COMMISSION OPTIONS

Tenant Admin configures per staff member or by rule:

Fixed Amount Commission:
└─ $50 per booking (flat rate)

Percentage Commission:
└─ 10% of booking price (percentage-based)

Tiered by Volume:
├─ 0-50 bookings: 8%
├─ 51-100 bookings: 10%
└─ 100+ bookings: 12%

Hybrid (Salary + Commission):
├─ Base salary: $2,000/month
└─ + Commission per booking

Payout Configuration (Tenant Admin decides):
├─ Frequency: Monthly (configurable)
├─ Approval: Automatic or Manual review
└─ Minimum Threshold: $X before payout


AGENT TYPE 2: INDEPENDENT AGENT (FREELANCER)

Definition:     Independent agents/resellers working with multiple tenants
Creation:       Created by SuperAdmin or Admin
Scope:          Can work with multiple Tenants (resell catalogs)
Access:         Own white-label: agent1.LitXusTravel.com

Capabilities:
├─ Browse available Tenant tour packages
├─ Resell Tenant tours on own white-label site
├─ Earn commission on bookings made through own site
├─ Can markup tours (within Tenant limits)
├─ Own subscription tier
└─ Own white-label website provisioned automatically


INDEPENDENT AGENT COMMISSION EXAMPLE

Scenario: Agent resells Tenant A's tour

Tour Base Price:        $500
Agent Commission Rate:   20% (configured by Tenant)
Customer Pays:          $500

Revenue Split:
├─ Tenant Earns:        $400 (80%)
└─ Agent Earns:         $100 (20% commission)


WITH MARKUP (Optional)

Agent can markup tours within Tenant limits:

Tour Base Price:        $500
Markup (if allowed):    +10% ($50 max)
Customer Pays:          $550

Revenue Split:
├─ Tenant Base:         $400 (original $500)
├─ Markup to Agent:     $50 (agent's addition)
├─ Commission on Base:  $100 (20% of $500)
└─ Agent Total Earns:   $150

Markup Rules (Tenant Configures):
├─ Max Amount: $50
├─ Max Percentage: 10%
├─ Whichever is lower (enforced)
├─ Tenant approval needed for high markups
└─ Customer sees original price + markup breakdown


COMMISSION CONFIGURATION FOR FREELANCE AGENTS

Base Commission:
└─ 20% of booking price (example)

Tiered by Volume:
├─ 0-100 bookings/month: 15%
├─ 101-500 bookings/month: 20%
└─ 500+ bookings/month: 25%

Can Vary By:
├─ Tour category
├─ Destination
├─ Season/promotion
└─ Individual agent

Important Notes:
├─ Only COMPLETED bookings count toward commissions
├─ Markups capped at lower of fixed $ or %
├─ Tenant approval required for high markups
└─ Commission finalizes on tour completion only
"@

Create-Document "Agent Models & Commission System" "2-Agent-Models.docx" $content2

# ============================================================================
# Document 3: Safeguards
# ============================================================================

$content3 = @"
COMMISSION SAFEGUARDS: FRAUD PREVENTION

10 Critical Safeguards to Prevent Fraud and Abuse


SAFEGUARD 1: REFUND/CANCELLATION REVERSAL

Problem:
Staff creates booking → Gets commission accrued
Customer cancels next day → Refund processed
Commission already paid/scheduled?

Risk:
├─ Staff: Create fake bookings, get commission, cancel before payout
└─ Company: Revenue lost, commission paid for nothing

Solution:
✓ Commission finalizes ONLY on tour completion (not booking)
✓ If cancelled/refunded before completion → Commission reversed
✓ Payout includes only completed/finalized commissions
✓ If refund occurs after payout → Deduct from next payout

Status Flow:
Booking Made → "Accrued"
       ↓
Tour Completed → "Finalized"
       ↓
Payment Processed → "Paid"

If Cancelled/Refunded:
Any Accrued/Pending → "Reversed"
Already Paid → "Refunded" (deducted next cycle)


SAFEGUARD 2: SELF-BOOKING PREVENTION

Problem:
Staff books own tour with own referral code
Gets commission on their own 'sale'

Risk:
├─ Staff: 'Create' fake revenue to earn commission
└─ Company: Loses money on fake bookings

Solution:
✓ Block self-referrals: Staff cannot use own code
✓ System validation: Check StaffId ≠ CodeOwnerId
✓ Audit: Flag suspicious patterns (same IP, same card)
✓ Alert: Tenant Admin notified of attempts


SAFEGUARD 3: CODE SHARING PREVENTION

Problem:
Staff gives code 'STAFF-JOHN-001' to friend
Friend books tours, John gets credit

Risk:
├─ Wrong person gets commission
├─ Quota tracking becomes meaningless
└─ Code attribution corrupted

Solution:
✓ Codes must be personal/unique (JOHN-001 only for John)
✓ Code rotation: Change codes periodically (monthly)
✓ Audit: Track IP/location using code
✓ Alert: Unusual geography changes or high-volume usage


SAFEGUARD 4: TIERED COMMISSION GAMING

Problem:
Freelance agent at 498 bookings (20% rate)
Creates 10 fake small bookings to hit 500+ tier (25%)
Gets higher commission rate

Risk:
├─ Artificially inflates sales metrics
└─ Gamed commission rates

Solution:
✓ Only COMPLETED/verified bookings count toward tier
✓ Minimum booking value threshold enforced
✓ Audit: Review sudden spikes in volume
✓ Monthly tier reset based on finalized bookings


SAFEGUARD 5: MARKUP PRICE VALIDATION

Problem:
Tenant allows 10% markup
Freelance agent marks $500 tour up to $10,000
Sells at inflated price, earns huge commission

Risk:
├─ Tenant's brand damaged (unfair pricing)
├─ Customer complaints
└─ Charge disputes increase

Solution:
✓ Max markup: Lower of ($X amount OR Y% percentage)
✓ Validation: Reject markup > original + 10%
✓ Approval: Require approval for high markups
✓ Customer sees: Original price + markup breakdown


SAFEGUARD 6: DUPLICATE BOOKING DETECTION

Problem:
Customer books same tour twice (accident or intentional)
Both bookings attributed to staff code
Staff gets commission twice

Risk:
├─ Duplicate revenue counted
└─ Inflated commission calculations

Solution:
✓ Dedup logic: Prevent same customer + tour + date combo
✓ Validation: Must have different dates/times
✓ Customer warning: 'You already have this booking'
✓ System rejects duplicates at booking time


SAFEGUARD 7: DEPARTING STAFF POLICY

Problem:
Staff leaves; commission accrued but not paid yet
Who gets the commission?
Staff tries to claim after departure?

Risk:
├─ Disputed payouts
├─ Staff claims vs. Tenant disputes
└─ Unclear ownership

Solution:
✓ Commission finalizes only after tour completion
✓ Clear policy: 'Paid only if staff active on payout date'
✓ Escrow option: Hold commission 30 days after departure
✓ Audit: Track all departing staff commission payments
✓ Tenant configures policy: Auto-pay / Escrow / Forfeit


SAFEGUARD 8: REFUND REVERSAL TRACKING

Problem:
Refund occurs after payout
No clear mechanism to reverse commission

Risk:
└─ Lost revenue without compensation

Solution:
✓ Refund creates reverse commission accrual
✓ If already paid → Deducted from next payout
✓ Audit trail shows:
   Original commission + Reversal + Net impact
✓ Agent can dispute refunds (dispute resolution)


SAFEGUARD 9: TIERED COMMISSION CAPS

Problem:
Tenant creates tiered rule with unfair/excessive rates
Admin doesn't catch unrealistic rates

Risk:
├─ Unsustainable commission expenses
└─ Tenant loses money on high-volume sales

Solution:
✓ Max tier commission capped (e.g., max 30%)
✓ SuperAdmin/Admin approval for tier changes
✓ Audit: Track who created/modified rules and when
✓ Quarterly: Review abuse patterns


SAFEGUARD 10: DISPUTE RESOLUTION (NOT DIRECT OVERRIDE)

Why NOT Direct SuperAdmin Override:
✗ Breaks audit trail
✗ Can't trace root cause
✗ Enables abuse
✗ Unclear who made the change

Better Approach: Structured Dispute Resolution

SuperAdmin identifies issue
        ↓
Creates Dispute Ticket:
├─ Description of problem
├─ Proposed fix
├─ Reason code (bug, miscalculation, etc.)
└─ Sends to Tenant Admin for review
        ↓
Tenant Admin reviews:
├─ Approves fix, OR
└─ Rejects (with feedback)
        ↓
If approved: System recalculates automatically
        ↓
Audit shows:
├─ Original calculation
├─ Dispute ticket #
├─ Justification
└─ New calculation (transparent)

Emergency Force Override (Rare Cases):
├─ Mandatory comment/reason
├─ Email notification to Tenant
├─ Flagged as 'Force Override' in audit
└─ Requires approval from another SuperAdmin (dual-control)
"@

Create-Document "Commission Safeguards" "3-Safeguards.docx" $content3

# ============================================================================
# Document 4: Implementation
# ============================================================================

$content4 = @"
DATA MODELS & IMPLEMENTATION GUIDE


CORE DOMAIN ENTITIES


AdminUser (Role Management)
├─ Id (GUID)
├─ Name, Email
├─ Role (SuperAdmin, Admin)
├─ Scope (Platform, Tenant)
├─ AssignedTenantId (if Tenant scope)
├─ ManagedTenantIds (list of tenant IDs)
└─ IsActive


StaffAgent (Internal Employees)
├─ Id (GUID)
├─ TenantId
├─ Name, Email
├─ UniqueCode (STAFF-JOHN-001)
├─ CodeIssuedAt, CodeExpiresAt
├─ IsActive, JoinedAt, DepartedAt
└─ CommissionRules (assigned)


IndependentAgent (Freelancers)
├─ Id (GUID)
├─ Name, Email
├─ CurrentSubscription
├─ WhiteLabelDomain (agent1.LitXusTravel.com)
├─ TenantIdsAuthorized (can resell which tenants)
└─ IsActive


CommissionRule (Configuration)
├─ Id (GUID)
├─ TenantId
├─ AgentId (specific agent, optional)
├─ Trigger (BookingCreated, TourCompleted, etc.)
├─ Amount ($ or %)
├─ IsPercentage (true/false)
├─ TieredRates (volume-based rates)
├─ PayoutFrequency (Monthly)
├─ AutoApprove (true/false)
├─ MinimumThreshold ($ amount)
├─ EffectiveFrom, EffectiveTo
└─ IsActive


CommissionAccrual (Earnings Tracking)
├─ Id (GUID)
├─ AgentId, TenantId
├─ CommissionRuleId
├─ TriggerType, SourceId
├─ CommissionAmount, Percentage
├─ BaseAmount (tour price, booking price, etc.)
├─ Status (Accrued/Pending/Paid/Reversed/Cancelled)
├─ AccruedAt, PaidAt
├─ PayoutId (reference to payout batch)
├─ AuditLog (all changes tracked)
└─ DisputeTicketId (if under dispute)


CommissionPayout (Settlement)
├─ Id (GUID)
├─ AgentId, TenantId
├─ PayoutPeriodStart, PayoutPeriodEnd
├─ CommissionAccrualIds (list)
├─ TotalAmount
├─ Status (Pending/Approved/Processed/Failed)
├─ ProcessedAt (timestamp)
└─ TransactionId (bank reference)


DisputeResolutionTicket (Conflict Resolution)
├─ Id (GUID)
├─ SuperAdminId
├─ CommissionAccrualId
├─ Description, ProposedFix
├─ ReasonCode (Bug, Miscalculation, etc.)
├─ Status (Open/Pending/Approved/Rejected)
├─ ReviewedByTenantAdminId
├─ CreatedAt, ResolvedAt
├─ OriginalAmount, AdjustedAmount
└─ Audit trail


AuditLog (All SuperAdmin Actions)
├─ Id (GUID)
├─ SuperAdminId
├─ Action (what was done)
├─ Timestamp, Reason
├─ AffectedEntityType, AffectedEntityId
├─ AffectedTenantId, AffectedAgentId
├─ BeforeState, AfterState (JSON)
└─ ClientIp


IMPLEMENTATION PHASES


Phase 1: Core Entities & Repositories
├─ Create domain aggregates
├─ Create repository interfaces
├─ Implement EF Core mappings
└─ Create unit tests (domain invariants)

Phase 2: Commission Safeguards
├─ Implement all 10 safeguards
├─ Add validation logic
├─ Create domain events
└─ Unit test each safeguard

Phase 3: Use Cases (Commands/Queries)
├─ CreateAdminCommand
├─ CreateStaffAgentCommand
├─ ConfigureCommissionRuleCommand
├─ CreateBookingCommand (with safeguards)
├─ ProcessCommissionPayoutCommand
├─ CreateDisputeTicketCommand
├─ ResolveDisputeCommand
└─ Various query handlers

Phase 4: API Endpoints
├─ Admin management endpoints
├─ Commission configuration endpoints
├─ Commission balance endpoints
├─ Commission reports endpoints
└─ Dispute management endpoints

Phase 5: Validation & Testing
├─ Unit tests for safeguards
├─ Integration tests for flows
├─ Load testing at scale
├─ Security testing (fraud attempts)
└─ UAT scripts

Phase 6: Dashboards & Reporting
├─ Agent commission dashboard
├─ Tenant commission reports
├─ SuperAdmin audit dashboard
└─ Anomaly detection reports


DATABASE TABLES

Tables (PascalCase, no underscore):
├─ AdminUsers
├─ StaffAgents
├─ IndependentAgents
├─ CommissionRules
├─ CommissionAccruals
├─ CommissionPayouts
├─ CodeUsageAudits
├─ DisputeResolutionTickets
└─ AuditLogs

Indexes:
├─ IX_StaffAgents_TenantId_IsActive
├─ IX_CommissionAccruals_AgentId_Status
├─ IX_CommissionAccruals_BookingId
├─ IX_CodeUsageAudits_Code_UsedAt
├─ IX_AuditLogs_SuperAdminId_Timestamp
└─ IX_DisputeResolutionTickets_Status


FRAUD PREVENTION CHECKLIST

Before Go-Live, Verify:

✓ Commission finalizes on tour COMPLETION, not booking
✓ Refunds reverse accrued commissions automatically
✓ Self-booking blocked (can't use own code)
✓ Code sharing detected (IP/location anomalies)
✓ Tiered commissions based on COMPLETED bookings
✓ Markup capped (lower of $ or %)
✓ Duplicate bookings rejected
✓ Departing staff policy enforced
✓ Refunds tracked & reversed if already paid
✓ Dispute resolution logged with full audit
✓ SuperAdmin overrides require dual-control + audit
✓ Monthly commission anomaly report generated
"@

Create-Document "Data Models & Implementation" "4-Implementation.docx" $content4

# ============================================================================
# Cleanup
# ============================================================================

Write-Host "`n✓ All Word documents created successfully!"
Write-Host "Location: $OutputPath"
Write-Host ""

Get-ChildItem $OutputPath -Filter "*.docx" | ForEach-Object {
    Write-Host "  ✓ $($_.Name) ($([Math]::Round($_.Length/1KB))KB)"
}

Write-Host ""
Write-Host "Opening documents folder..."
Start-Process explorer.exe $OutputPath

# Quit Word
$word.Quit()
