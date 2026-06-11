# Generate Advanced Diagram Documents for LitXusTravel
# Includes workflow diagrams, process flows, and visual architectures

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

    # Save
    $doc.SaveAs("$OutputPath\$filename", 12)
    $doc.Close($false)

    Write-Host "✓ Created: $filename"
}

# ============================================================================
# Document 5: Workflow Diagrams - Commission Accrual & Payout
# ============================================================================

$content5 = @"
COMMISSION WORKFLOWS


WORKFLOW 1: STAFF AGENT BOOKING & COMMISSION ACCRUAL

Step 1: Tenant Admin creates Staff Agent
├─ AgentId: STAFF-001
├─ Name: John
├─ Code: STAFF-JOHN-001
├─ TenantId: TENANT-A
└─ CommissionRule: 10% per booking

Step 2: Customer books tour (Staff makes booking)
├─ BookingId: BOOKING-001
├─ TourId: TOUR-100
├─ StaffAgentId: STAFF-001
├─ BookingPrice: $500
├─ Status: "Pending"
└─ CreatedAt: 2026-06-10

Step 3: System creates commission accrual (PRELIMINARY)
├─ CommissionAccrualId: COMM-001
├─ AgentId: STAFF-001
├─ BookingId: BOOKING-001
├─ CommissionAmount: $50 (10% of $500)
├─ Status: "Accrued" (not yet paid)
└─ Note: Only finalized on tour completion

Step 4a: Customer completes tour (HAPPY PATH)
├─ BookingStatus: "Completed"
├─ CompletedAt: 2026-06-15
└─ Trigger: Commission status → "Finalized"

Step 4b: Customer cancels tour (REFUND PATH)
├─ BookingStatus: "Cancelled"
├─ CancelledAt: 2026-06-11
└─ Action: CommissionAccrual → "Reversed" (not paid)

Step 5: Monthly commission payout cycle
├─ Period: June 1-30, 2026
├─ Aggregate: All finalized commissions for STAFF-001
│  └─ Total: $5,000 (if 100 bookings × 10% × $500)
├─ Apply minimum threshold: $100 (met)
├─ Status: "Pending Payout"
└─ Action: Send to payment processor

Step 6: Commission payout processed
├─ PayoutId: PAYOUT-JUNE-001
├─ AgentId: STAFF-001
├─ Amount: $5,000
├─ Status: "Paid"
├─ ProcessedAt: 2026-07-01
└─ TransactionId: TXN-2026-07-001


WORKFLOW 2: INDEPENDENT AGENT RESELLING & COMMISSION

Step 1: Independent Agent created by SuperAdmin
├─ AgentId: AGENT-001
├─ Name: Travel Influencer
├─ Subscription: Premium Tier
├─ WhiteLabelDomain: travelinfluencer.LitXusTravel.com
└─ AuthorizedTenants: [TENANT-A, TENANT-B, TENANT-C]

Step 2: Tenant A configures freelance commission
├─ CommissionRuleId: RULE-001
├─ TenantId: TENANT-A
├─ AgentId: AGENT-001
├─ BaseCommission: 20%
├─ AllowMarkup: true (up to $50)
├─ TieredRates:
│  ├─ 0-100 bookings: 15%
│  ├─ 101-500 bookings: 20%
│  └─ 500+ bookings: 25%
└─ MinimumThreshold: $100

Step 3: Customer visits agent's white-label site
├─ URL: travelinfluencer.LitXusTravel.com
├─ Browses Tenant A's tour catalog
├─ Sees tour (Base: $500, Agent markup: +$50)
└─ Clicks "Book" on $550 tour

Step 4: Booking created (Auto-attributed to Agent)
├─ BookingId: BOOKING-A-001
├─ TourId: TOUR-A-100
├─ IndependentAgentId: AGENT-001
├─ TenantId: TENANT-A
├─ BookingPrice: $550
├─ Status: "Pending"
└─ Source: "freelance_agent_website"

Step 5: Commission accrual created
├─ CommissionAccrualId: COMM-A-001
├─ AgentId: AGENT-001
├─ BookingId: BOOKING-A-001
├─ BaseAmount: $550
├─ CommissionPercentage: 20% (current tier)
├─ CommissionAmount: $110 (20% of $550)
├─ Status: "Accrued"
├─ Note: Markup not commission, agent keeps $50 + $110 = $160
└─ TenantEarnings: $400 (original $500 price)

Step 6: Tour completed
├─ BookingStatus: "Completed"
├─ CompletedAt: 2026-06-20
└─ Commission status: "Finalized"

Step 7: Monthly payout for Agent
├─ Period: June 2026
├─ Bookings completed: 450
├─ Current tier: 20% (101-500 range)
├─ Total commission: $45,000
├─ Status: "Pending Payout"
└─ Next payout date: July 1, 2026


WORKFLOW 3: COMMISSION DISPUTE RESOLUTION

Step 1: SuperAdmin identifies discrepancy
├─ Issue: Commission calculated incorrectly
├─ Example: System applied 15% instead of 20%
└─ Impact: Agent underpaid by $500

Step 2: SuperAdmin creates dispute ticket
├─ DisputeTicketId: DISPUTE-001
├─ CommissionAccrualId: COMM-001
├─ Description: "System applied wrong tier rate"
├─ ProposedFix: "Recalculate using 20% rate"
├─ ReasonCode: "Miscalculation"
├─ Status: "Open"
└─ SentToTenantAdminAt: 2026-06-10

Step 3: Tenant Admin reviews dispute
├─ Reviews: Original calculation (15%) vs. correct (20%)
├─ Verifies: Agent had 150 bookings (should be 20% tier)
├─ Decision: "APPROVED"
└─ Status: "Pending Recalculation"

Step 4: System recalculates automatically
├─ OriginalAmount: $7,500 (15% of $50,000)
├─ AdjustedAmount: $10,000 (20% of $50,000)
├─ Difference: +$2,500 (agent refund)
├─ NewStatus: "Resolved"
└─ AuditLog entry: Full history recorded

Step 5: Adjustment payout issued
├─ AdjustmentPayoutId: ADJUST-001
├─ AgentId: AGENT-001
├─ Amount: $2,500 (difference)
├─ Type: "Dispute Resolution"
├─ ProcessedAt: 2026-06-15
└─ Note: Added to next regular payout

Audit Trail:
├─ Original: 15%, $7,500, Paid 2026-06-01
├─ Dispute: DISPUTE-001, created 2026-06-10
├─ Review: Approved by TENANT-ADMIN-001
├─ Recalc: 20%, $10,000 total
├─ Adjust: $2,500 difference, Paid 2026-06-15
└─ All changes timestamped & logged


WORKFLOW 4: REFUND & COMMISSION REVERSAL

Step 1: Booking completed, commission paid
├─ BookingId: BOOKING-001
├─ CommissionAmount: $100
├─ Status: "Paid"
├─ PayoutId: PAYOUT-001
└─ ProcessedDate: 2026-06-01

Step 2: Customer requests refund (July 1)
├─ Reason: "Didn't enjoy tour"
├─ Status: "Refund Requested"
└─ CreatedAt: 2026-07-01

Step 3: Refund processed
├─ RefundId: REFUND-001
├─ BookingId: BOOKING-001
├─ Amount: $500
├─ Status: "Processed"
└─ ProcessedAt: 2026-07-05

Step 4: System detects refund
├─ Query: Find CommissionAccrual for BOOKING-001
├─ Found: CommissionAccrualId: COMM-001, Status: "Paid"
├─ Decision: Commission was already paid
└─ Action: Create reverse accrual

Step 5: Reverse commission created
├─ ReverseAccrualId: COMM-REVERSE-001
├─ OriginalCommissionId: COMM-001
├─ Amount: -$100 (reversal)
├─ Reason: "Booking refunded"
├─ Status: "Pending"
└─ ScheduledForNextPayout: 2026-08-01

Step 6: Next payout cycle (August)
├─ Regular commissions: $8,000
├─ Reversals: -$100
├─ Net payout: $7,900
├─ Note: Agent notified of reversal
└─ TransactionId: TXN-AUG-001

Audit Trail:
├─ Booking completed: 2026-06-15
├─ Commission accrued: COMM-001, $100, Status: Accrued
├─ Commission finalized: Status: Finalized
├─ Payout processed: PAYOUT-001, $100 Paid 2026-06-01
├─ Refund requested: 2026-07-01
├─ Refund processed: REFUND-001, 2026-07-05
├─ Reversal created: COMM-REVERSE-001, -$100
├─ Reversal applied: PAYOUT-AUG, 2026-08-01
└─ Final net: Agent received $100, then -$100 reversal
"@

Create-Document "Commission Workflows" "5-Workflows.docx" $content5

# ============================================================================
# Document 6: Process Flows - Setup & Operations
# ============================================================================

$content6 = @"
PROCESS FLOWS & SETUP PROCEDURES


PROCESS FLOW 1: TENANT STAFF AGENT HIRING & SETUP

Initial Setup by Tenant Admin:

Step 1: Create Staff Agent
├─ Navigate: Admin Dashboard → Staff Management
├─ Click: "Add New Staff Agent"
├─ Form:
│  ├─ Name: John Smith
│  ├─ Email: john@tenant.com
│  ├─ Phone: (optional)
│  └─ Role: Sales Agent
└─ Click: "Create"

Step 2: System generates agent
├─ AgentId: Auto-generated (GUID)
├─ UniqueCode: Auto-generated (STAFF-JOHN-001)
├─ Status: "Active"
├─ CreatedAt: Current timestamp
└─ Notification: Email sent to John

Step 3: Assign commission rule
├─ Navigate: Agents → John Smith → Commission
├─ Option A: Use default rule
│  └─ "Use tenant commission rule (10%)"
├─ Option B: Custom rule
│  ├─ Type: Percentage
│  ├─ Rate: 15%
│  └─ Trigger: Per booking
└─ Click: "Apply"

Step 4: John accesses white-label website
├─ URL: tenant.LitXusTravel.com
├─ Login: john@tenant.com (password reset email)
├─ Dashboard shows:
│  ├─ "Create Tour" button
│  ├─ "View Tours" list
│  ├─ "My Commission Balance: $0"
│  └─ "Manage Referral Code: STAFF-JOHN-001"
└─ Ready to start selling

Step 5: John creates & sells tours
├─ Create tour:
│  ├─ Tour name, destination, price
│  └─ Click "Publish"
├─ Book customer:
│  ├─ Customer info, tour selection, date
│  └─ Click "Confirm Booking"
├─ Commission accrues:
│  ├─ Per booking: 15% of $500 = $75
│  └─ "Commission Balance: $75"
└─ (Finalizes on tour completion)

Step 6: Commission payout
├─ Monthly: June 1-30
├─ Total bookings: 40
├─ Total commission: $3,000 (40 × $75)
├─ Payout: Processed June 30
└─ Notification: "Commission paid $3,000"

Departing Staff Workflow:

Step 1: Staff leaves tenant
├─ John gives notice: "Last day: June 30"
└─ Tenant Admin marks: "Departing"

Step 2: Tenant configures policy
├─ Setting: What to do with pending commission
├─ Options:
│  ├─ A) Auto-pay (default)
│  ├─ B) Hold 30 days (escrow)
│  └─ C) Forfeit
└─ Chosen: Option A (Auto-pay)

Step 3: Final month commission
├─ Period: June 1-30
├─ Bookings: 50
├─ Commission earned: $3,750
├─ Payout: Processed on June 30
├─ Status: "Paid"
└─ Note: No further commission after John leaves

Step 4: Commission locked
├─ Query: Any pending commission for John?
├─ Result: None (all finalized & paid)
├─ Status: "Closed"
└─ Audit: Department left 2026-06-30, final payout processed


PROCESS FLOW 2: INDEPENDENT AGENT SUBSCRIPTION & SETUP

Step 1: SuperAdmin creates Independent Agent
├─ Navigate: Admin → Independent Agents
├─ Click: "Create New Agent"
├─ Form:
│  ├─ Name: Travel Influencer Inc
│  ├─ Email: info@travelinfluencer.com
│  ├─ Phone: (optional)
│  └─ Subscription Tier: Premium
└─ Click: "Create"

Step 2: System provisions white-label website
├─ AgentId: Auto-generated
├─ Domain: travelinfluencer.LitXusTravel.com
├─ SSL: Auto-provisioned
├─ Status: "Provisioning..." → "Active"
└─ Setup time: ~5 minutes

Step 3: Agent logs in & configures
├─ Login: info@travelinfluencer.com
├─ Dashboard:
│  ├─ "Browse available tenant tours"
│  ├─ "Manage my catalog"
│  ├─ "View my commission balance"
│  └─ "View analytics"
└─ Completes profile setup

Step 4: Tenant adds agent to their catalog
├─ Tenant Admin navigates: Settings → Freelance Agents
├─ Clicks: "Add Agent"
├─ Selects: "Travel Influencer Inc"
├─ Sets commission rule:
│  ├─ Base: 20%
│  ├─ Tiered: 15%/20%/25%
│  ├─ Markup: Allow up to $50
│  └─ Min threshold: $100
└─ Confirms: "Agent authorized"

Step 5: Agent adds tenant tours to catalog
├─ Agent navigates: Catalog → Add Tours
├─ Filters by: Tenant A
├─ Selects tours:
│  ├─ Beach Resort Package ($500)
│  ├─ Mountain Adventure ($800)
│  └─ City Tour ($200)
├─ Sets optional markup per tour
│  ├─ Beach: +$50 (can sell for $550)
│  ├─ Mountain: +0 (sell at $800)
│  └─ City: +$25 (can sell for $225)
└─ Publishes: Tours go live on site

Step 6: Agent starts selling
├─ Customers visit: travelinfluencer.LitXusTravel.com
├─ Browse: Agent's curated catalog
├─ Book: Tours auto-attributed to agent
├─ Commission: Accrues per booking
└─ Payout: Monthly automated


PROCESS FLOW 3: CUSTOMER BOOKING JOURNEY (WITH CODE ATTRIBUTION)

Standard Booking (No Code):
├─ Customer visits: tenant.LitXusTravel.com
├─ Browses tours
├─ Clicks: "Book Tour"
├─ No staff involved
└─ NO COMMISSION to any staff

Booking via Staff Code:
├─ Customer receives code: "STAFF-JOHN-001"
│  └─ Via: Email, text, or referral link
├─ Customer visits: tenant.LitXusTravel.com
├─ Enters code at checkout: "STAFF-JOHN-001"
├─ System validates:
│  ├─ Code exists? Yes
│  ├─ Code active? Yes
│  ├─ Code expired? No
│  └─ Code belongs to tenant? Yes
├─ Booking created:
│  ├─ BookingId: BOOKING-001
│  ├─ StaffAgentId: STAFF-JOHN-001
│  ├─ ReferralCode: STAFF-JOHN-001
│  └─ Commission attributed: Yes
└─ John gets commission on this booking

Booking via Independent Agent Site:
├─ Customer visits: travelinfluencer.LitXusTravel.com
├─ Browses tours from Tenant A
├─ Clicks: "Book Tour"
├─ NO CODE NEEDED
├─ System auto-attributes:
│  ├─ IndependentAgentId: AGENT-001
│  ├─ TenantId: TENANT-A
│  ├─ Source: "freelance_agent_website"
│  └─ Commission: Automatic
└─ Agent gets commission on this booking

Booking Journey Timeline:
├─ 2026-06-10: Customer books (Commission accrued)
├─ 2026-06-15: Tour completes (Commission finalized)
├─ 2026-06-30: Monthly payout cycle (Commission pending)
├─ 2026-07-01: Payment processed (Commission paid)
└─ 2026-07-05: Agent sees funds in account


PROCESS FLOW 4: COMMISSION CONFIGURATION OPTIONS

Tenant Admin's Commission Configuration Dashboard:

Main Settings:
├─ Section 1: Commission Type
│  ├─ ☑ Staff Agent Commission
│  └─ ☑ Independent Agent Commission
│
├─ Section 2: Staff Agent Rules
│  ├─ Default rule (all staff):
│  │  ├─ Type: Percentage
│  │  ├─ Value: 10%
│  │  ├─ Trigger: Per booking
│  │  └─ Payout: Monthly, Auto-approve, Min $50
│  │
│  └─ Per-agent overrides:
│     ├─ John Smith: 15% (higher performer)
│     ├─ Jane Doe: 8% (part-time)
│     └─ [Add custom rules]
│
├─ Section 3: Independent Agent Rules
│  ├─ Tenant A tours:
│  │  ├─ Base: 20%
│  │  ├─ Markup allowed: $50 max
│  │  ├─ Tier 1 (0-100): 15%
│  │  ├─ Tier 2 (101-500): 20%
│  │  └─ Tier 3 (500+): 25%
│  │
│  └─ Tenant B tours (different category):
│     ├─ Base: 15%
│     ├─ Markup allowed: None
│     └─ Fixed: 15% (no tiers)
│
└─ Section 4: Payout Settings
   ├─ Frequency: Monthly / Weekly / Custom
   ├─ Approval: Auto-approve / Manual review
   ├─ Threshold: Minimum $100 to trigger payout
   └─ Save: [Save Settings]
"@

Create-Document "Process Flows & Setup" "6-Process-Flows.docx" $content6

# ============================================================================
# Document 7: Architecture Diagrams - Data & System
# ============================================================================

$content7 = @"
SYSTEM ARCHITECTURE & DATA RELATIONSHIPS


ARCHITECTURE 1: USER HIERARCHY & RELATIONSHIPS

┌─────────────────────────────────────────────────────┐
│                    SUPERADMIN                        │
│  (You - Full Platform Control)                      │
│  - Create other Admins                              │
│  - Create global packages                           │
│  - Oversee all dashboards                           │
│  - Audit trail on all actions                       │
└────────────────┬────────────────────────────────────┘
                 │
          ┌──────┴──────┐
          │             │
    ┌─────▼────┐   ┌────▼──────┐
    │  ADMIN-1 │   │  ADMIN-2  │
    │ Platform │   │  Tenant   │
    │   Admin  │   │   Admin   │
    └─────┬────┘   └────┬──────┘
          │             │
          │        ┌────▼──────────┐
          │        │               │
          │   ┌────▼─────┐    ┌────▼─────┐
          │   │ TENANT-A │    │ TENANT-B │
          │   │          │    │          │
          │   └────┬──────┘   └────┬─────┘
          │        │               │
          │   ┌────▼────┐     ┌────▼───┐
          │   │  STAFF  │     │ STAFF  │
          │   │ AGENTS  │     │AGENTS  │
          │   │ 1-3 per │     │1-3 per │
          │   │ tenant  │     │tenant  │
          │   └─────────┘     └────────┘
          │
          └───────────────────────┐
                                  │
                            ┌─────▼──────────────┐
                            │INDEPENDENT AGENTS  │
                            │  (Freelancers)     │
                            │ Can resell any     │
                            │ tenant's tours     │
                            └────────────────────┘


ARCHITECTURE 2: BOOKING & COMMISSION FLOW

STAFF AGENT BOOKING:

                    Tenant A
                       │
    ┌──────────────────┼──────────────────┐
    │                  │                  │
    ▼                  ▼                  ▼
┌────────────┐  ┌──────────────┐  ┌──────────────┐
│   Tour 1   │  │    Tour 2    │  │    Tour 3    │
│ Base: $500 │  │ Base: $800   │  │ Base: $200   │
└────┬───────┘  └──────┬───────┘  └──────┬───────┘
     │                 │                 │
     └─────────────────┼─────────────────┘
                       │
                    BOOKING
                       │
     ┌─────────────────┼──────────────────┐
     │                 │                  │
     ▼                 ▼                  ▼
┌──────────┐     ┌──────────┐      ┌──────────┐
│ Staff    │     │ Staff    │      │Customer  │
│John      │     │Jane      │      │Direct    │
│Code:001  │     │Code:002  │      │No Code   │
└────┬─────┘     └────┬─────┘      └──────────┘
     │                │
     ├────────────────┼────────────────┐
     │                │                │
     ▼                ▼                ▼
┌─────────────────────────────────────────────┐
│   Commission Accrual (Preliminary)          │
│  ├─ John: $50 (10% of $500)                │
│  ├─ Jane: $80 (10% of $800)                │
│  └─ Direct: $0 (no code)                   │
└─────────────┬───────────────────────────────┘
              │
              ▼
        (Tour Completes)
              │
              ▼
┌─────────────────────────────────────────────┐
│   Commission Finalized                      │
│  ├─ John: $50 ✓ (finalized)                │
│  ├─ Jane: $80 ✓ (finalized)                │
│  └─ Direct: $0 (no commission)             │
└─────────────┬───────────────────────────────┘
              │
              ▼
        (Monthly Payout)
              │
              ▼
┌─────────────────────────────────────────────┐
│   Commission Paid                           │
│  ├─ John: $50 ✓ (paid)                     │
│  ├─ Jane: $80 ✓ (paid)                     │
│  └─ Direct: $0                             │
└─────────────────────────────────────────────┘


INDEPENDENT AGENT BOOKING:

Independent Agent: Travel Influencer
    ├─ Domain: travelinfluencer.LitXusTravel.com
    └─ Authorized tenants: A, B, C

     ┌──────────────────────────────────────┐
     │  Agent's Curated Catalog              │
     ├──────────────────────────────────────┤
     │ From Tenant A:                       │
     │  ├─ Beach Tour ($500) [Markup +$50]  │
     │  ├─ Mountain Tour ($800) [Markup +0] │
     │  └─ City Tour ($200) [Markup +$25]   │
     │                                      │
     │ From Tenant B:                       │
     │  ├─ Desert Safari ($600) [No markup] │
     │  └─ Jungle Trek ($700) [No markup]   │
     └──────────────────────────────────────┘
              │
              ▼
         CUSTOMER BOOKS
              │
              ▼
     ┌────────────────────────┐
     │ Booking Details:       │
     ├────────────────────────┤
     │ IndependentAgentId:    │
     │   AGENT-001            │
     │ TenantId: TENANT-A     │
     │ TourId: TOUR-001       │
     │ BookingPrice: $550     │
     │   (Base $500 +         │
     │    Markup $50)         │
     │ Status: Pending        │
     └────────┬───────────────┘
              │
              ▼
    ┌──────────────────────────┐
    │ Commission Accrued:      │
    ├──────────────────────────┤
    │ AgentId: AGENT-001       │
    │ CommissionAmount:        │
    │   $110 (20% of $550)     │
    │ Status: Accrued          │
    │ (Pending completion)     │
    └──────────┬───────────────┘
               │
               ▼
         (Tour Completes)
               │
               ▼
    ┌──────────────────────────┐
    │ Commission Finalized:    │
    ├──────────────────────────┤
    │ Amount: $110 ✓           │
    │ Status: Finalized        │
    │ Ready for payout         │
    └──────────┬───────────────┘
               │
               ▼
         (Monthly Payout)
               │
               ▼
    ┌──────────────────────────┐
    │ Commission Paid:         │
    ├──────────────────────────┤
    │ Amount: $110 ✓           │
    │ Status: Paid             │
    │ PayoutId: PAYOUT-001     │
    └──────────────────────────┘

Note: Tenant A receives:
  Base price: $500 ✓
  Agent keeps markup: $50
  Agent gets commission: $110 (20% of $550)


ARCHITECTURE 3: COMMISSION STATUS FLOW

┌─────────────┐
│   ACCRUED   │  Booking made, commission calculated
│             │  Not yet paid, pending tour completion
└──────┬──────┘
       │
       │ (Tour completes)
       ▼
┌─────────────┐
│ FINALIZED   │  Tour completed, commission locked in
│             │  Ready for payout in next cycle
└──────┬──────┘
       │
       │ (Monthly payout cycle)
       ▼
┌─────────────┐
│   PENDING   │  Finalized commissions awaiting payment
│   PAYOUT    │  Threshold met, scheduled for payout
└──────┬──────┘
       │
       │ (Payment processed)
       ▼
┌─────────────┐
│    PAID     │  Commission paid to agent
│             │  Settlement complete
└─────────────┘

Alternative Paths:

If Refunded Before Completion:
    ACCRUED → REVERSED → (Not paid, no payout)

If Refunded After Payout:
    ACCRUED → FINALIZED → PAID → REFUNDED
    (Next payout includes -amount)


ARCHITECTURE 4: DATA RELATIONSHIPS

Tenant
  │
  ├─ Has many: Tours
  │  └─ Tour
  │     ├─ BookingPrice
  │     └─ Created by: StaffAgent or by Tenant
  │
  ├─ Has many: StaffAgents
  │  └─ StaffAgent
  │     ├─ UniqueCode (e.g., STAFF-JOHN-001)
  │     └─ Has many: CommissionRules
  │
  ├─ Has many: CommissionRules
  │  └─ CommissionRule
  │     ├─ Applies to: StaffAgent or all staff
  │     ├─ Trigger: BookingCreated, TourCompleted
  │     └─ Amount: Fixed, Percentage, or Tiered
  │
  └─ Receives many: Bookings
     └─ Booking
        ├─ CreatedBy: StaffAgent or Customer (direct)
        ├─ HasMany: CommissionAccruals
        └─ When tour completes:
           └─ CommissionAccruals finalize


IndependentAgent
  │
  ├─ AuthorizedTenants: [List of tenant IDs]
  │  └─ Can resell tours from these tenants
  │
  ├─ HasMany: CommissionRules (per tenant)
  │  └─ CommissionRule
  │     ├─ TenantId
  │     ├─ BaseCommission: %
  │     ├─ AllowMarkup: true/false
  │     └─ TieredRates: [0-100: 15%, 101-500: 20%, ...]
  │
  └─ Receives many: Bookings (from own site)
     └─ Booking
        ├─ IndependentAgentId
        ├─ TenantId
        ├─ BookingPrice (base + markup)
        └─ CommissionAccruals auto-created


CommissionAccrual
  │
  ├─ AgentId: StaffAgent or IndependentAgent
  ├─ BookingId: Reference to booking
  ├─ CommissionRuleId: Rule that applies
  ├─ Status flow: ACCRUED → FINALIZED → PAID
  │
  └─ When booking refunded:
     └─ REVERSED or REFUNDED


Monthly Payout Process:
  │
  ├─ Query: All FINALIZED commissions for Agent
  ├─ Filter: In current month
  ├─ Sum: Total commission
  ├─ Check: Meets minimum threshold?
  │  ├─ Yes: Create CommissionPayout
  │  └─ No: Wait for next month
  └─ If created: Marked as PENDING PAYOUT
"@

Create-Document "System Architecture Diagrams" "7-Architecture.docx" $content7

# ============================================================================
# Document 8: Quick Reference & Checklists
# ============================================================================

$content8 = @"
QUICK REFERENCE GUIDES & CHECKLISTS


QUICK REFERENCE 1: COMMISSION CALCULATION EXAMPLES

Example 1: Staff Agent - Fixed Amount

Scenario:
├─ Staff Agent: John
├─ Tenant: Travel Co
├─ Commission Rule: \$50 per booking
├─ Tour Price: \$500
└─ Number of bookings: 10 (completed in June)

Calculation:
├─ Booking 1: \$50
├─ Booking 2: \$50
├─ ...
├─ Booking 10: \$50
└─ Total: \$500

Payout:
├─ Period: June 2026
├─ Total: \$500
├─ Meets threshold (\$100)? Yes
└─ Status: Pay on June 30


Example 2: Staff Agent - Percentage

Scenario:
├─ Staff Agent: Jane
├─ Commission Rule: 10% per booking
├─ Bookings completed:
│  ├─ Tour A: \$500 booking × 10% = \$50
│  ├─ Tour B: \$800 booking × 10% = \$80
│  ├─ Tour C: \$300 booking × 10% = \$30
│  └─ Total: 3 bookings
└─ Total commission: \$160

Payout:
├─ Period: June 2026
├─ Total: \$160
├─ Meets threshold (\$100)? Yes
└─ Status: Pay on June 30


Example 3: Independent Agent - Tiered

Scenario:
├─ Independent Agent: Influencer Inc
├─ Completed bookings in June: 250
├─ Commission rules:
│  ├─ 0-100 bookings: 15%
│  ├─ 101-500 bookings: 20%
│  └─ 500+ bookings: 25%
├─ Bookings breakdown:
│  ├─ Bookings 1-100: 100 × \$500 avg = \$50,000 base
│  └─ Bookings 101-250: 150 × \$500 avg = \$75,000 base
└─ Total base: \$125,000

Calculation:
├─ Tier 1 (0-100): \$50,000 × 15% = \$7,500
├─ Tier 2 (101-250): \$75,000 × 20% = \$15,000
└─ Total: \$22,500

Payout:
├─ Period: June 2026
├─ Total: \$22,500
├─ Meets threshold (\$100)? Yes
└─ Status: Pay on June 30


Example 4: Independent Agent - With Markup

Scenario:
├─ Agent: Influencer Inc
├─ Tour base price: \$500
├─ Agent markup allowed: \$50
├─ Agent's selling price: \$550
├─ Commission rule: 20%
└─ Bookings in June: 100 completed

Calculation:
├─ Per booking earned by agent:
│  ├─ Markup: \$50
│  ├─ Commission: \$550 × 20% = \$110
│  └─ Agent total per booking: \$160
├─ All 100 bookings:
│  ├─ Markup total: \$5,000
│  ├─ Commission total: \$11,000
│  └─ Agent total: \$16,000
└─ Tenant receives: \$500 × 100 = \$50,000 (base price)

Payout:
├─ Agent earns: \$16,000
├─ Meets threshold? Yes
└─ Status: Pay on June 30


QUICK REFERENCE 2: COMMISSION STATUS MEANINGS

Status: ACCRUED
├─ Meaning: Booking made, commission calculated
├─ When: Immediately when booking created
├─ Paid? No
├─ Reversible? Yes (if booking cancelled before completion)
└─ Next step: Finalize when tour completes

Status: FINALIZED
├─ Meaning: Tour completed, commission locked in
├─ When: When tour completion date passes
├─ Paid? No, but scheduled
├─ Reversible? Only if refund occurs
└─ Next step: Include in next month's payout

Status: PENDING PAYOUT
├─ Meaning: Commission ready for payment
├─ When: Collected during monthly payout cycle
├─ Paid? No, awaiting payment processing
├─ Reversible? Only if refund or dispute
└─ Next step: Payment processor handles

Status: PAID
├─ Meaning: Commission successfully paid to agent
├─ When: After payment processor confirms
├─ Paid? Yes
├─ Reversible? Yes, if refund/dispute after payment
└─ Next step: Payment complete

Status: REVERSED
├─ Meaning: Commission cancelled (booking cancelled before completion)
├─ When: When booking cancelled
├─ Paid? No
├─ Reversible? Can reverse a reversal (rare)
└─ Next step: None, commission removed

Status: REFUNDED
├─ Meaning: Commission reversal for refund after payment
├─ When: When refund processed post-payout
├─ Paid? Was paid, now reversed
├─ Reversible? Can dispute the reversal
└─ Next step: Deducted from next payout


QUICK REFERENCE 3: CODE USAGE GUIDE

Staff Referral Code Format:
├─ Format: STAFF-{FirstName}-{Sequence}
├─ Example: STAFF-JOHN-001, STAFF-JANE-002
├─ Length: 20 characters max
├─ Unique: One per staff member
├─ Transferable: Can be changed/rotated monthly
└─ Shareable: Staff can share for referrals

How to Use Code:
1. Tenant Admin creates staff agent
2. System auto-generates code (STAFF-JOHN-001)
3. Staff shares code with customers
4. Customer enters code at checkout
5. System validates & attributes booking
6. Commission credited to staff

Code Rotation:
├─ Recommended: Monthly
├─ Process: Generate new code (STAFF-JOHN-002)
├─ Old code: Still works for 30 days (grace period)
├─ Reason: Prevent code sharing/misuse
├─ Audit: Track which code used for each booking
└─ Alert: Unusual code patterns flagged

Fraud Detection:
├─ Red flag: Code used from multiple countries
├─ Red flag: Code used 100x per day (unusual volume)
├─ Red flag: Code used immediately after generation
├─ Action: Audit log + alert to Tenant Admin
└─ Review: SuperAdmin can investigate


QUICK REFERENCE 4: MONTHLY PAYOUT CHECKLIST

Before Payout Cycle Starts (End of Month):

☐ All bookings for period finalized?
☐ All refunds processed?
☐ All disputes resolved?
☐ Commission reversals applied?
☐ Tiered commissions correctly calculated?
☐ Minimum threshold met for each agent?

During Payout Cycle (Processing):

☐ Query: All FINALIZED commissions for period
☐ Group: By agent (staff or independent)
☐ Filter: Meets minimum threshold (\$100+)
☐ Sum: Total per agent
☐ Calculate: Tiered rates applied correctly
☐ Check: No self-booking commissions
☐ Review: Markup amounts reasonable
☐ Verify: Refunded bookings excluded

Before Payment Submitted:

☐ Generate: Payout report
☐ Review: All amounts correct
☐ Check: No anomalies detected
☐ Approve: Tenant Admin or SuperAdmin
☐ Audit: Record all details
☐ Verify: Payment method on file
☐ Confirm: Amounts match bank requirements

After Payment Processed:

☐ Mark: CommissionAccrual status → PAID
☐ Record: TransactionId from bank
☐ Update: PayoutId in database
☐ Notify: Agents of payment
☐ Archive: Payout records
☐ Audit: Final verification
☐ Report: Generate payout summary


QUICK REFERENCE 5: DISPUTE RESOLUTION CHECKLIST

Dispute Identified:

☐ Issue described clearly
☐ Expected amount documented
☐ Actual amount calculated
☐ Difference confirmed
☐ Root cause identified
☐ Evidence collected

Dispute Ticket Created:

☐ Unique ticket ID assigned
☐ SuperAdmin who created it logged
☐ CommissionAccrual referenced
☐ Proposed fix documented
☐ Reason code selected
☐ Description detailed
☐ Evidence attached
☐ Tenant Admin notified

Tenant Admin Review:

☐ Review: Original calculation vs. proposed fix
☐ Verify: Tier rates applied correctly
☐ Check: Data consistent
☐ Decision: Approve or reject
☐ Comment: Reasoning documented
☐ Status: Updated to Approved/Rejected

Recalculation (if approved):

☐ New amount calculated
☐ Previous amount recorded
☐ Difference calculated
☐ Audit trail updated
☐ Status: Marked as Resolved
☐ Adjustment: Scheduled for next payout

Communication:

☐ Agent notified of dispute
☐ Agent notified of resolution
☐ Payment adjustment amount specified
☐ Timeline for adjustment payment
☐ Dispute ticket closed
☐ Documentation archived
"@

Create-Document "Quick Reference & Checklists" "8-Quick-Reference.docx" $content8

# ============================================================================
# Cleanup
# ============================================================================

Write-Host "`n✓ All advanced documents created successfully!"
Write-Host "Location: $OutputPath"
Write-Host ""

Get-ChildItem $OutputPath -Filter "*.docx" -ErrorAction SilentlyContinue |
    Sort-Object Name |
    ForEach-Object {
    Write-Host "  ✓ $($_.Name) ($([Math]::Round($_.Length/1KB))KB)"
}

Write-Host ""
Write-Host "Opening documents folder..."
Start-Process explorer.exe $OutputPath

# Quit Word
$word.Quit()
