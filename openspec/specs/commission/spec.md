# Commission System & Role Hierarchy

## Overview

LitXusTravel uses a four-tier role hierarchy: SuperAdmin → Admin (Platform or Tenant scope) →
StaffAgent / IndependentAgent. Commission is earned by agents on completed tours, with 10
mandatory safeguards that prevent fraud, self-booking, and audit-trail gaps.

Commission MUST finalize only on tour **completion**, never on booking creation. Refunds and
cancellations MUST automatically reverse accrued commissions.

---

## Requirements

### Requirement: SPEC-ROLE-SUPERADMIN — SuperAdmin Capabilities

The platform MUST support a SuperAdmin role with unrestricted cross-tenant access, audit logging,
and dispute resolution authority. SuperAdmin MUST be the only role that can create other Admins.

#### Scenario: SuperAdmin creates an Admin
- Given: An authenticated SuperAdmin
- When: POST /api/v1/admin/admins with name, email, role, scope
- Then: 201 Created; new Admin user can log in; action logged in AuditLog with who/what/when/where/why

#### Scenario: SuperAdmin views all tenant data
- Given: An authenticated SuperAdmin
- When: GET /api/v1/admin/tenants or any cross-tenant endpoint
- Then: 200 OK with data from all tenants; no tenant filter applied

#### Scenario: Non-SuperAdmin tries to create an Admin
- Given: An authenticated Platform Admin
- When: POST /api/v1/admin/admins
- Then: 403 Forbidden — only SuperAdmin can create Admins

---

### Requirement: SPEC-ROLE-ADMIN — Admin Role Variants

Admins come in two scopes. Platform Admins manage operational aspects across all tenants.
Tenant Admins are assigned to one or more tenants and have full control within those tenants only.

#### Scenario: Platform Admin lists all tenants
- Given: An authenticated Admin with scope "Platform"
- When: GET /api/v1/admin/tenants
- Then: 200 OK with all tenants; no restriction by tenant

#### Scenario: Tenant Admin manages their assigned tenant
- Given: An Admin with scope "Tenant" and tenantId = "tenant-a"
- When: POST /api/v1/admin/packages or GET /api/v1/admin/tenants/tenant-a/packages
- Then: 200/201 — Tenant Admin can manage their assigned tenant's packages

#### Scenario: Tenant Admin cannot access other tenants
- Given: An Admin with scope "Tenant" and tenantId = "tenant-a"
- When: GET /api/v1/admin/tenants/tenant-b/packages
- Then: 403 Forbidden

#### Scenario: Tenant Admin creates a staff agent
- Given: An Admin with scope "Tenant"
- When: POST /api/v1/tenants/{tenantId}/staff-agents with name, email
- Then: 201 Created; staff agent is scoped to this tenant; unique code generated (format: STAFF-{Name}-{Seq})

---

### Requirement: SPEC-COMMISSION-RULE — Configure Commission Rules

A Tenant Admin MUST be able to configure commission rules for their staff agents.
Rules support Fixed, Percentage, Tiered, and Hybrid models. Tiered rule changes MUST require
SuperAdmin or Admin approval.

#### Scenario: Tenant Admin configures a percentage commission
- Given: An authenticated Tenant Admin
- When: POST /api/v1/tenants/{tenantId}/commission-rules with type=Percentage, amount=10, trigger=TourCompleted
- Then: 201 Created; rule becomes active; staff earn 10% of booking price on tour completion

#### Scenario: Tenant Admin configures tiered commission
- Given: An authenticated Tenant Admin
- When: POST with type=Tiered, tiers=[{min:0,max:100,rate:15},{min:101,max:500,rate:20},{min:501,rate:25}]
- Then: 201 Created; pending Admin approval before activating (tiered rules require approval)

#### Scenario: Independent agent resells at markup
- Given: A commission rule allowing markups with maxMarkupDollars=50, maxMarkupPercent=10
- When: Independent agent sets tour price to $550 (original $500 + $50 markup)
- Then: Markup is accepted; agent earns commission on base $500 + $50 markup retention

---

### Requirement: SPEC-SAFEGUARD-1 — Refund/Cancellation Reversal

Commission MUST only finalize on tour completion. Cancellations and refunds MUST automatically
reverse all associated accrued commissions.

#### Scenario: Commission status flow — happy path
- Given: A booking is created with a matching commission rule
- When: Booking → Created (commission status: Accrued) → Tour Completed (status: Finalized) → Payout (status: Paid)
- Then: Agent receives payout only after tour completion

#### Scenario: Cancellation before tour — reverses Accrued commission
- Given: A booking with commission status "Accrued" (not yet Finalized)
- When: Booking is cancelled
- Then: Commission status changes to "Reversed"; no payout is generated

#### Scenario: Refund after payout — deducts from next payout
- Given: A commission already in status "Paid"
- When: A refund is processed for that booking
- Then: A negative CommissionAccrual (reversal) is created; it is deducted from the agent's next payout batch

---

### Requirement: SPEC-SAFEGUARD-2 — Self-Booking Prevention

Staff agents MUST NOT be able to use their own referral code on a booking they initiate.
The system MUST enforce this validation at the domain level, not just the UI.

#### Scenario: Staff tries to use own code
- Given: A booking is being created by Staff Agent John (STAFF-JOHN-001)
- When: The booking includes referralCode = "STAFF-JOHN-001"
- Then: 422 Unprocessable — "Cannot use own referral code"; booking is rejected

#### Scenario: Staff uses another agent's code
- Given: Staff Agent John creates a booking with code "STAFF-MARY-001"
- When: Commission is validated
- Then: Booking accepted; commission accrues to Mary, not John

---

### Requirement: SPEC-SAFEGUARD-3 — Code Sharing Detection

The system MUST detect anomalous referral code usage (unusual geography, high volume) and
alert the Tenant Admin. Codes are unique per agent and should be rotated monthly.

#### Scenario: Code used from unusual location
- Given: A code "STAFF-JOHN-001" is normally used from Malaysia
- When: Code is used from a foreign IP within 1 hour of normal usage
- Then: CodeUsageAudit record is flagged; Tenant Admin receives alert notification

#### Scenario: Tenant Admin rotates a staff code
- Given: An authenticated Tenant Admin
- When: POST /api/v1/tenants/{tenantId}/staff-agents/{agentId}/rotate-code
- Then: 200 OK; old code expires immediately; new code issued with format STAFF-{Name}-{NextSeq}

---

### Requirement: SPEC-SAFEGUARD-4 — Tier Gaming Prevention

Tiered commission tiers MUST be evaluated based on completed bookings only. A monthly reset
MUST occur. Sudden tier jumps MUST trigger an anomaly alert.

#### Scenario: Tier calculation uses only completed bookings
- Given: An agent has 200 total bookings this month but only 80 completed tours
- When: Tier evaluation runs at month end
- Then: Agent qualifies for Tier 1 (0–100 completed) not Tier 2 — pending bookings do not count

#### Scenario: Suspicious tier jump triggers alert
- Given: An agent normally has 50 completed tours/month
- When: Agent jumps two tiers in one month (e.g., 50 → 600)
- Then: Anomaly event logged; SuperAdmin or Admin receives alert for manual review

---

### Requirement: SPEC-SAFEGUARD-5 — Markup Price Validation

Independent agents MUST NOT exceed the tenant-configured markup limits (lower of dollar cap
or percentage cap). Markups above the approval threshold require Tenant Admin approval.

#### Scenario: Markup within limits — accepted
- Given: Rule allows max $50 or 10% markup; tour base price = $500
- When: Agent sets price to $545 ($45 markup, 9%)
- Then: Booking accepted; markup is within both limits

#### Scenario: Markup exceeds dollar limit — rejected
- Given: Rule allows max $50 markup; tour base price = $500
- When: Agent sets price to $600 ($100 markup)
- Then: 422 Unprocessable — "Markup $100 exceeds max $50"

#### Scenario: Markup requires approval
- Given: ApprovalThreshold = $30; agent proposes $40 markup (within $50 cap)
- When: Booking is submitted
- Then: Booking is held pending Tenant Admin approval; agent notified of pending status

---

### Requirement: SPEC-SAFEGUARD-6 — Duplicate Booking Detection

The system MUST reject bookings where the same customer has already confirmed the same tour
on the same date.

#### Scenario: Duplicate booking rejected
- Given: Customer C has a confirmed booking for Tour T on 2026-07-15
- When: Another booking is created for Customer C, Tour T, 2026-07-15
- Then: 422 Unprocessable — "Customer already has a booking for this tour date"

#### Scenario: Same customer, different date — allowed
- Given: Customer C has a confirmed booking for Tour T on 2026-07-15
- When: New booking is created for Tour T on 2026-08-20
- Then: 201 Created — different date is allowed

---

### Requirement: SPEC-SAFEGUARD-7 — Departing Staff Commission Policy

When a staff agent departs, their accrued commissions MUST be handled per the Tenant's
configured policy: Auto-Pay, Escrow (30-day hold), or Forfeit.

#### Scenario: Staff departs — escrow policy
- Given: Tenant is configured with DeparturePolicy = Escrow30Days
- When: Staff agent is marked as departed (DepartedAt set)
- Then: All Accrued/Finalized commissions are locked; held 30 days before auto-payout

#### Scenario: Staff departs — forfeit policy
- Given: Tenant is configured with DeparturePolicy = Forfeit
- When: Staff agent is marked as departed
- Then: All pending commissions set to status "Forfeited"; audit log entry created

---

### Requirement: SPEC-SAFEGUARD-8 — Refund Reversal Tracking

Every refund MUST create a traceable reversal entry linked to the original commission accrual.
If commission was already paid, the reversal MUST appear as a negative in the next payout.

#### Scenario: Full audit trail for refund
- Given: Booking B had commission paid in January payout
- When: Booking B is refunded in February
- Then: CommissionAccrual record shows status "Reversed"; linked PayoutId shows original January batch;
  negative accrual created for February payout deduction

---

### Requirement: SPEC-SAFEGUARD-9 — Tiered Commission Cap

Tiered commission rates MUST be capped at a platform maximum (default 30%). Any rule
configuring a rate above the cap MUST require SuperAdmin approval.

#### Scenario: Rate above 30% requires SuperAdmin approval
- Given: Tenant Admin tries to create a tier with rate = 35%
- When: POST /api/v1/tenants/{tenantId}/commission-rules
- Then: 202 Accepted (pending); rule created in "PendingApproval" status; SuperAdmin notified

#### Scenario: SuperAdmin approves high-rate tier
- Given: A commission rule in "PendingApproval" with rate = 35%
- When: SuperAdmin PUT /api/v1/admin/commission-rules/{id}/approve
- Then: 200 OK; rule becomes active; full audit trail entry created

---

### Requirement: SPEC-SAFEGUARD-10 — Dispute Resolution (No Direct Override)

Commission disputes MUST go through a structured ticket workflow. SuperAdmin cannot directly
override a commission amount without a dispute ticket. Emergency force override requires
dual-control (two SuperAdmin approvals) and is flagged permanently in the audit trail.

#### Scenario: SuperAdmin creates a dispute ticket
- Given: An authenticated SuperAdmin
- When: POST /api/v1/admin/disputes with commissionAccrualId, description, proposedFix, reasonCode
- Then: 201 Created; ticket sent to Tenant Admin for review

#### Scenario: Tenant Admin approves dispute
- Given: A dispute ticket in status "Open"
- When: Tenant Admin PUT /api/v1/tenants/{tenantId}/disputes/{id}/approve
- Then: System recalculates commission automatically; original + adjusted amounts stored; status = "Resolved"

#### Scenario: Emergency force override — requires dual control
- Given: A SuperAdmin initiates emergency override
- When: A second SuperAdmin also approves the override
- Then: Commission is adjusted; AuditLog entry flagged as "ForceOverride" with both approvers' IDs, timestamps, and reason

---

### Requirement: SPEC-COMMISSION-PAYOUT — Commission Payout Processing

The system MUST generate monthly payout batches containing only Finalized commissions.
Minimum payout threshold applies. Payouts require Admin approval before processing.

#### Scenario: Monthly payout batch generated
- Given: End of payout period with agents having Finalized commissions
- When: POST /api/v1/admin/payouts/generate with periodStart, periodEnd
- Then: CommissionPayout records created per agent; includes only Finalized commissions above minimum threshold

#### Scenario: Agent below minimum threshold — excluded
- Given: Agent's finalized commission total = $15; minimum threshold = $50
- When: Payout batch is generated
- Then: Agent is excluded from this batch; their commission rolls over to next period

#### Scenario: Admin approves payout batch
- Given: A payout batch in status "Pending"
- When: Admin PUT /api/v1/admin/payouts/{id}/approve
- Then: Status = "Approved"; payment processing initiated; agents notified

---

## Key Domain Entities

```
CommissionRule         — rule config (type, amount/%, trigger, tiers, frequency)
CommissionAccrual      — per-booking commission record (status: Accrued → Finalized → Paid → Reversed)
CommissionPayout       — monthly batch of Finalized accruals for an agent
CodeUsageAudit         — IP/location record for every referral code use
DisputeResolutionTicket— structured dispute with dual-approval workflow
AuditLog               — immutable record of every SuperAdmin action
```

## Commission Status Flow

```
Booking Created → CommissionAccrual (Accrued)
Tour Completed  → CommissionAccrual (Finalized)
Payout Batch    → CommissionAccrual (Paid)
Cancelled       → CommissionAccrual (Reversed)   [at any stage before Paid]
Refund after Pay→ New negative CommissionAccrual (Reversal) applied to next payout
```
