# Generate Professional Word Documents for LitXusTravel Architecture
# This script creates formatted Word documents with diagrams and descriptions

param(
    [string]$OutputPath = "c:\LitXus Systems\LitXusTravel\Documentation",
    [switch]$OpenAfter = $true
)

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Create Word application
$word = New-Object -ComObject Word.Application
$word.Visible = $false

function Add-Heading {
    param($doc, $text, $level = 1)
    $range = $doc.Content
    $range.InsertAfter($text + "`n")
    $range.ParagraphFormat.Style = "Heading $level"
    $range.ParagraphFormat.SpaceAfter = 12
    return $range
}

function Add-Paragraph {
    param($doc, $text, $spacing = 6, $indent = 0)
    $range = $doc.Content
    $range.InsertAfter($text + "`n")
    $range.ParagraphFormat.SpaceAfter = $spacing
    if ($indent -gt 0) {
        $range.ParagraphFormat.LeftIndent = $indent * 14.2  # Points
    }
    return $range
}

function Add-CodeBlock {
    param($doc, $text)
    $range = $doc.Content
    $range.InsertAfter($text + "`n")
    $range.Font.Name = "Courier New"
    $range.Font.Size = 10
    $range.Font.Color = 8421504  # Gray
    $range.ParagraphFormat.LeftIndent = 28.35  # 0.5 inch
    $range.ParagraphFormat.SpaceAfter = 12
    return $range
}

function Add-Table {
    param($doc, $rows, $cols, $data)
    $table = $doc.Tables.Add($doc.Content.Paragraphs.Item($doc.Content.Paragraphs.Count).Range, $rows, $cols)
    $table.Style = "Light Grid Accent 1"

    for ($r = 1; $r -le $rows; $r++) {
        for ($c = 1; $c -le $cols; $c++) {
            $cell = $table.Cell($r, $c)
            if ($null -ne $data[$r-1][$c-1]) {
                $cell.Range.Text = $data[$r-1][$c-1]
                if ($r -eq 1) {
                    $cell.Range.Font.Bold = $true
                }
            }
        }
    }
    return $table
}

# ============================================================================
# Document 1: Role Hierarchy & Authorization
# ============================================================================

Write-Host "Creating: Role Hierarchy & Authorization.docx..."

$doc1 = $word.Documents.Add()

# Title
$title = Add-Heading $doc1 "LitXusTravel Role Hierarchy & Authorization" 1
$title.Range.Font.Size = 24
$title.Range.Font.Bold = $true

Add-Paragraph $doc1 "Complete role definitions, permissions, and delegation model"
Add-Paragraph $doc1 "Date: 2026-06-10" 0

# --------
Add-Heading $doc1 "Role Hierarchy Overview" 2

Add-CodeBlock $doc1 @"
SuperAdmin (Full Platform Control)
  ├─ Create other Admins
  ├─ Create global subscription packages
  ├─ Oversee all dashboards (read-only)
  ├─ Audit all Admin activities
  └─ Dispute resolution authority

Admin (Two Variants)
  ├─ Platform Admin
  │  ├─ Manage operational aspects
  │  ├─ Assign tenants
  │  └─ Cannot create other Admins
  └─ Tenant Admin (assigned to 1+ tenants)
     ├─ Create tenants
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
"@

# --------
Add-Heading $doc1 "SuperAdmin Permissions" 2

Add-Paragraph $doc1 "SuperAdmin has full platform control with complete audit trail:"

Add-CodeBlock $doc1 @"
Permissions:
  ├─ Create Admin users (both Platform & Tenant scope)
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
"@

# --------
Add-Heading $doc1 "Admin Role (Two Variants)" 2

Add-Heading $doc1 "Variant A: Platform Admin" 3

Add-Paragraph $doc1 "Helps SuperAdmin manage operational aspects of the platform."

Add-CodeBlock $doc1 @"
Responsibilities:
  ├─ Manage operational aspects
  ├─ Assign/manage tenants
  ├─ View all tenant data (functional areas)
  ├─ Cannot delete core configs
  ├─ Cannot override system settings
  └─ Actions audited by SuperAdmin

Token Claim:
  {
    "role": "Admin",
    "scope": "Platform",
    "sub": "user-id"
  }
"@

Add-Heading $doc1 "Variant B: Tenant Admin" 3

Add-Paragraph $doc1 "Manages one or more assigned tenants and their operations."

Add-CodeBlock $doc1 @"
Responsibilities:
  ├─ Full control within assigned tenant(s)
  ├─ Create/manage tours and packages
  ├─ Configure pricing & overrides
  ├─ Create/manage staff agents
  ├─ Configure commission rules
  ├─ View commission reports (own tenant)
  └─ Cannot access other tenants

Token Claim:
  {
    "role": "Admin",
    "scope": "Tenant",
    "tenantId": "guid-here",
    "sub": "user-id"
  }
"@

# Save Document 1
$doc1.SaveAs("$OutputPath\1-Role-Hierarchy-and-Authorization.docx")
$doc1.Close()

# ============================================================================
# Document 2: Agent Models & Commission System
# ============================================================================

Write-Host "Creating: Agent Models & Commission System.docx..."

$doc2 = $word.Documents.Add()

# Title
$title = Add-Heading $doc2 "Agent Models & Commission System" 1
$title.Range.Font.Size = 24
$title.Range.Font.Bold = $true

Add-Paragraph $doc2 "Comprehensive guide to Agent Type 1 (Staff) and Type 2 (Freelancers)"
Add-Paragraph $doc2 "Date: 2026-06-10" 0

# --------
Add-Heading $doc2 "Agent Type 1: Tenant Staff (Internal Employees)" 2

Add-CodeBlock $doc2 @"
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
"@

Add-Heading $doc2 "Staff Agent Commission Configuration" 3

Add-Paragraph $doc2 "Tenant Admin configures commission per staff member or by rule:"

Add-CodeBlock $doc2 @"
Commission Options:
  ├─ Fixed Amount
  │  └─ $50 per booking
  ├─ Percentage
  │  └─ 10% of booking price
  ├─ Tiered by Volume
  │  ├─ 0-50 bookings: 8%
  │  ├─ 51-100 bookings: 10%
  │  └─ 100+ bookings: 12%
  ├─ Hybrid
  │  ├─ Base salary (optional)
  │  └─ + Commission per booking
  └─ Custom Rules
     └─ Can vary by staff member/role

Payout Configuration (Tenant Admin decides):
  ├─ Frequency: Monthly (configurable)
  ├─ Approval: Automatic or Manual
  └─ Minimum Threshold: $X before payout
"@

# --------
Add-Heading $doc2 "Agent Type 2: Independent Agent (Freelancer)" 2

Add-CodeBlock $doc2 @"
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
"@

Add-Heading $doc2 "Independent Agent Commission (Reselling)" 3

Add-Paragraph $doc2 "When Independent Agent resells Tenant tours:"

Add-CodeBlock $doc2 @"
Example Scenario:
  Tour Base Price:     $500
  Agent Commission:    20% (configured by Tenant)
  Customer Pays:       $500
  ──────────────────────────
  Tenant Earns:        $400 (80%)
  Agent Earns:         $100 (20% commission)

With Markup (if allowed):
  Tour Base Price:     $500
  Agent Markup:        +10% allowed ($50 max)
  Customer Pays:       $550
  ──────────────────────────
  Tenant Earns:        $400 (original $500)
  Agent Earns:         $100 (commission) + $50 (markup) = $150

Commission Rules (Tenant Configures):
  ├─ Base Commission: % of booking price
  ├─ Tiered by Volume:
  │  ├─ 0-100 bookings/month: 15%
  │  ├─ 101-500 bookings/month: 20%
  │  └─ 500+ bookings/month: 25%
  ├─ Markup Allowance:
  │  ├─ Max Amount: $X
  │  ├─ Max Percentage: Y%
  │  └─ Whichever is lower
  └─ Can vary by:
     ├─ Tour category
     ├─ Destination
     ├─ Season/promotion
     └─ Individual agent
"@

# Save Document 2
$doc2.SaveAs("$OutputPath\2-Agent-Models-and-Commission-System.docx")
$doc2.Close()

# ============================================================================
# Document 3: Commission Safeguards
# ============================================================================

Write-Host "Creating: Commission Safeguards.docx..."

$doc3 = $word.Documents.Add()

# Title
$title = Add-Heading $doc3 "Commission Safeguards: Fraud Prevention" 1
$title.Range.Font.Size = 24
$title.Range.Font.Bold = $true

Add-Paragraph $doc3 "10 critical safeguards to prevent commission fraud and abuse"
Add-Paragraph $doc3 "Date: 2026-06-10" 0

# --------
Add-Heading $doc3 "Safeguard 1: Refund/Cancellation Reversal" 2

Add-Paragraph $doc3 "Prevents commission payment for cancelled or refunded bookings."

Add-CodeBlock $doc3 @"
Problem:
  Staff creates booking → Gets commission accrued
  Customer cancels next day → Refund processed
  Commission already paid/scheduled?

Risk:
  Staff: Create fake bookings, get commission, cancel before payout
  Company: Revenue lost, commission paid for nothing

Solution:
  ✓ Commission finalizes ONLY on tour completion (not booking)
  ✓ If cancelled/refunded before completion → Commission reversed
  ✓ Payout includes only completed/finalized commissions
  ✓ If refund occurs after payout → Deduct from next payout
"@

# --------
Add-Heading $doc3 "Safeguard 2: Self-Booking Prevention" 2

Add-Paragraph $doc3 "Blocks staff from earning commission on their own bookings."

Add-CodeBlock $doc3 @"
Problem:
  Staff books own tour with own code
  Gets commission on their own 'sale'

Risk:
  Staff: 'Create' fake revenue to earn commission
  Company: Loses money on fake bookings

Solution:
  ✓ Block self-referrals: Staff cannot use own code
  ✓ System validation: Check StaffId ≠ CodeOwnerId
  ✓ Audit: Flag suspicious patterns (same IP, same card)
  ✓ Alert: Tenant Admin notified of attempts
"@

# --------
Add-Heading $doc3 "Safeguard 3: Code Sharing Prevention" 2

Add-Paragraph $doc3 "Prevents staff from sharing referral codes with others."

Add-CodeBlock $doc3 @"
Problem:
  Staff gives code 'STAFF-JOHN-001' to friend
  Friend books tours, John gets credit

Risk:
  Wrong person gets commission
  Quota tracking becomes meaningless

Solution:
  ✓ Codes must be personal/unique (JOHN-001 only for John)
  ✓ Code rotation: Change codes periodically (monthly)
  ✓ Audit: Track IP/location using code
  ✓ Alert: Unusual geography changes or high-volume usage
"@

# --------
Add-Heading $doc3 "Safeguard 4: Tiered Commission Gaming" 2

Add-Paragraph $doc3 "Prevents artificially inflating bookings to reach higher commission tiers."

Add-CodeBlock $doc3 @"
Problem:
  Freelance agent at 498 bookings (20% rate)
  Creates 10 fake small bookings to hit 500+ tier (25%)
  Gets higher commission rate

Risk:
  Artificially inflates sales metrics
  Gamed commission rates

Solution:
  ✓ Only COMPLETED/verified bookings count toward tier
  ✓ Minimum booking value threshold enforced
  ✓ Audit: Review sudden spikes in volume
  ✓ Monthly tier reset based on finalized bookings
"@

# --------
Add-Heading $doc3 "Safeguard 5: Markup Price Validation" 2

Add-Paragraph $doc3 "Prevents excessive price markups on tour packages."

Add-CodeBlock $doc3 @"
Problem:
  Tenant allows 10% markup
  Freelance agent marks $500 tour up to $10,000
  Sells at inflated price, earns huge commission

Risk:
  Tenant's brand damaged (unfair pricing)
  Customer complaints & charge disputes

Solution:
  ✓ Max markup: Lower of ($X amount OR Y% percentage)
  ✓ Validation: Reject markup > original + 10%
  ✓ Approval: Require approval for high markups
  ✓ Customer sees: Original price + markup breakdown
"@

# --------
Add-Heading $doc3 "Safeguard 6: Duplicate Booking Detection" 2

Add-Paragraph $doc3 "Prevents customers from being charged multiple times for same tour."

Add-CodeBlock $doc3 @"
Problem:
  Customer books same tour twice (accident or intentional)
  Both bookings attributed to staff code
  Staff gets commission twice

Risk:
  Duplicate revenue counted
  Inflated commission calculations

Solution:
  ✓ Dedup logic: Prevent same customer + tour + date combo
  ✓ Validation: Must have different dates/times
  ✓ Customer warning: 'You already have this booking'
  ✓ System rejects duplicates at booking time
"@

# --------
Add-Heading $doc3 "Safeguard 7: Departing Staff Policy" 2

Add-Paragraph $doc3 "Clear policy for commission payment when staff leaves."

Add-CodeBlock $doc3 @"
Problem:
  Staff leaves; commission accrued but not paid yet
  Who gets the commission?
  Staff tries to claim after departure?

Risk:
  Disputed payouts
  Staff claims vs. Tenant disputes
  Unclear ownership

Solution:
  ✓ Commission finalizes only after tour completion
  ✓ Clear policy: 'Paid only if staff active on payout date'
  ✓ Escrow option: Hold commission 30 days after departure
  ✓ Audit: Track all departing staff commission payments
  ✓ Tenant configures policy (Auto-pay/Escrow/Forfeit)
"@

# --------
Add-Heading $doc3 "Safeguard 8: Refund Reversal Tracking" 2

Add-Paragraph $doc3 "Automatic reversal and tracking of refunded commissions."

Add-CodeBlock $doc3 @"
Problem:
  Refund occurs after payout
  No clear mechanism to reverse commission

Risk:
  Lost revenue without compensation
  No way to track refund impact

Solution:
  ✓ Refund creates reverse commission accrual
  ✓ If already paid → Deducted from next payout
  ✓ Audit trail shows:
     Original commission + Reversal + Net impact
  ✓ Agent can dispute refunds (dispute resolution)
"@

# --------
Add-Heading $doc3 "Safeguard 9: Tiered Commission Caps" 2

Add-Paragraph $doc3 "Maximum limits on commission percentages across all tiers."

Add-CodeBlock $doc3 @"
Problem:
  Tenant creates tiered rule with unfair/excessive rates
  Admin doesn't catch unrealistic rates

Risk:
  Unsustainable commission expenses
  Tenant loses money on high-volume sales

Solution:
  ✓ Max tier commission capped (e.g., max 30%)
  ✓ SuperAdmin/Admin approval for tier changes
  ✓ Audit: Track who created/modified rules and when
  ✓ Quarterly: Review abuse patterns
"@

# --------
Add-Heading $doc3 "Safeguard 10: Dispute Resolution (Not Direct Override)" 2

Add-Paragraph $doc3 "Structured workflow instead of SuperAdmin directly overriding commissions."

Add-CodeBlock $doc3 @"
Why NOT Direct Override:
  ✗ Breaks audit trail
  ✗ Can't trace root cause
  ✗ Enables abuse
  ✗ Unclear who made the change and why

Better Approach: Dispute Resolution:
  SuperAdmin identifies issue
        ↓
  Creates Dispute Ticket:
    ├─ Description of problem
    ├─ Proposed fix
    ├─ Reason code (bug, miscalculation, etc.)
    └─ Sends to Tenant Admin for review
        ↓
  Tenant Admin reviews
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

Emergency Force Override (rare):
  ├─ Mandatory comment/reason
  ├─ Email notification to Tenant
  ├─ Flagged as 'Force Override' in audit
  └─ Requires approval from another SuperAdmin (dual-control)
"@

# Save Document 3
$doc3.SaveAs("$OutputPath\3-Commission-Safeguards.docx")
$doc3.Close()

# ============================================================================
# Document 4: Data Models & Implementation
# ============================================================================

Write-Host "Creating: Data Models & Implementation.docx..."

$doc4 = $word.Documents.Add()

# Title
$title = Add-Heading $doc4 "Data Models & Implementation Guide" 1
$title.Range.Font.Size = 24
$title.Range.Font.Bold = $true

Add-Paragraph $doc4 "Database schema, entities, and implementation checklist"
Add-Paragraph $doc4 "Date: 2026-06-10" 0

# --------
Add-Heading $doc4 "Core Domain Entities" 2

Add-CodeBlock $doc4 @"
AdminUser (Role Management)
  ├─ Id (GUID)
  ├─ Name, Email
  ├─ Role (SuperAdmin, Admin)
  ├─ Scope (Platform, Tenant)
  ├─ AssignedTenantId (if Tenant scope)
  ├─ ManagedTenantIds (list)
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
  ├─ TenantIdsAuthorized (can resell)
  └─ IsActive

CommissionRule (Configuration)
  ├─ Id (GUID)
  ├─ TenantId
  ├─ AgentId (specific agent)
  ├─ Trigger (BookingCreated, TourCompleted, etc.)
  ├─ Amount ($ or %)
  ├─ TieredRates (volume-based)
  ├─ PayoutFrequency (Monthly)
  ├─ AutoApprove (true/false)
  ├─ MinimumThreshold
  └─ EffectiveFrom, EffectiveTo

CommissionAccrual (Earnings Tracking)
  ├─ Id (GUID)
  ├─ AgentId, TenantId
  ├─ CommissionRuleId
  ├─ TriggerType, SourceId
  ├─ CommissionAmount, Percentage
  ├─ Status (Accrued/Pending/Paid/Reversed)
  ├─ AccruedAt, PaidAt
  ├─ PayoutId
  ├─ AuditLog (all changes)
  └─ DisputeTicketId (if disputed)

CommissionPayout (Settlement)
  ├─ Id (GUID)
  ├─ AgentId, TenantId
  ├─ PayoutPeriodStart, PayoutPeriodEnd
  ├─ CommissionAccrualIds (list)
  ├─ TotalAmount
  ├─ Status (Pending/Approved/Processed)
  └─ TransactionId (bank ref)

DisputeResolutionTicket (Conflict Resolution)
  ├─ Id (GUID)
  ├─ SuperAdminId
  ├─ CommissionAccrualId
  ├─ Description, ProposedFix
  ├─ ReasonCode (Bug, Miscalculation, etc.)
  ├─ Status (Open/Pending/Approved/Rejected)
  ├─ ReviewedByTenantAdminId
  ├─ OriginalAmount, AdjustedAmount
  └─ Audit trail

AuditLog (All SuperAdmin Actions)
  ├─ Id (GUID)
  ├─ SuperAdminId
  ├─ Action (what was done)
  ├─ Timestamp, Reason
  ├─ AffectedTenantId, AffectedAgentId
  ├─ BeforeState, AfterState (JSON)
  └─ ClientIp
"@

# --------
Add-Heading $doc4 "Implementation Phases" 2

Add-Paragraph $doc4 "6-phase implementation plan:"

Add-CodeBlock $doc4 @"
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
  └─ Integration tests for flows

Phase 4: API Endpoints
  ├─ Admin management endpoints
  ├─ Commission configuration endpoints
  ├─ Commission balance endpoints
  ├─ Reports endpoints
  └─ Dispute management endpoints

Phase 5: Validation & Testing
  ├─ Unit tests for safeguards
  ├─ Integration tests for flows
  ├─ Load testing
  ├─ Security testing (fraud attempts)
  └─ UAT scripts

Phase 6: Dashboards & Reporting
  ├─ Agent commission dashboard
  ├─ Tenant commission reports
  ├─ SuperAdmin audit dashboard
  └─ Anomaly detection reports
"@

# Save Document 4
$doc4.SaveAs("$OutputPath\4-Data-Models-and-Implementation.docx")
$doc4.Close()

# ============================================================================
# Cleanup
# ============================================================================

Write-Host "`n✓ All documents created successfully!"
Write-Host "Location: $OutputPath"

$doc_files = Get-ChildItem $OutputPath -Filter "*.docx"
$doc_files | ForEach-Object {
    Write-Host "  ✓ $($_.Name)"
}

# Open the folder if requested
if ($OpenAfter) {
    Start-Process $OutputPath
}

# Quit Word
$word.Quit()
