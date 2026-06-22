# Tenant Dashboard UI

## Overview

The tenant dashboard is the white-label workspace for Tenant Admins and Staff Agents to manage
their packages, inquiries, tours, bookings, staff, and commission setup. It is a Next.js app under
`web/tenant-dashboard/`, sharing the same TailAdmin-derived component library and layout shell as
the admin portal (`web/admin-portal/`) — fixed always-dark collapsible sidebar, sticky light header,
Framer Motion, Tailwind CSS v4.

Tech: Next.js App Router, TailAdmin-derived components, Framer Motion, Tailwind CSS v4.
For shared layout/header/component behavior (sidebar collapse, mobile drawer, ActionMenu, Modal,
Select, dark-mode pattern), see `openspec/specs/admin-portal/spec.md` — the tenant dashboard
reuses the identical pattern via its own copies of `AppSidebar`, `AppHeader`, `Backdrop`,
`Dropdown`/`DropdownItem`, and `Modal` under `web/tenant-dashboard/src/components/`.

---

## Page Map

```
/                    → Dashboard Overview (stat cards)
/packages            → Synced/owned package list with override management
/inquiries           → Inquiry & CRM list
/tours               → Tour scheduling (per-package departure dates, capacity)
/bookings            → Customer bookings, referral code tracking
/staff               → Staff agent management, referral code rotation
/commission          → Commission rule configuration + agent statement lookup
/settings            → Tenant settings
```

---

## Requirements

### Requirement: SPEC-TENANT-LAYOUT — Shared TailAdmin Sidebar Navigation

The tenant dashboard sidebar MUST use the same collapsible always-dark TailAdmin shell as the
admin portal (see SPEC-ADMIN-LAYOUT), with a tenant-specific nav item set: Dashboard, Packages,
Inquiries, Tours, Bookings, Staff, Commission, Settings — in that order.

#### Scenario: Tenant sidebar nav order
- Given: A Tenant Admin or Staff Agent loads any `/(dashboard)` route
- When: Sidebar renders
- Then: Nav items appear in order: Dashboard, Packages, Inquiries, Tours, Bookings, Staff, Commission, Settings; the item matching the current route is highlighted active

---

### Requirement: SPEC-TENANT-TOURS — Tour Scheduling Page

The tours page MUST list the tenant's scheduled tours (Package, Date, Capacity `booked/capacity`,
Status) fetched live from `tenantApi.getTours(tenantId)`, with a create form (Package ID, Tour Date,
Capacity, Base Price) and a "Mark Complete" action available only on `Scheduled` tours.

#### Scenario: Completing a tour finalizes commissions
- Given: A tour with status `Scheduled` exists
- When: Tenant clicks "Mark Complete" in the tour's ActionMenu
- Then: `tenantApi.completeTour` is called; success toast reads "Tour completed — commissions finalized"; tour list reloads (see SPEC-COMMISSION-FINALIZE in `commission/spec.md` for the finalization rule)

#### Scenario: Creating a tour
- Given: Tenant clicks "New Tour" and fills Package ID, Tour Date, Capacity, Base Price
- When: Form is submitted
- Then: `tenantApi.createTour(tenantId, form)` is called; on success the form closes, fields reset, and the list reloads

---

### Requirement: SPEC-TENANT-BOOKINGS — Booking Management Page

The bookings page MUST list the tenant's bookings (Customer, Tour Date, Amount, Referral Code,
Status) fetched live from `tenantApi.getBookings(tenantId)`, support creating a booking (Tour ID,
Customer Name/Email, Tour Date, optional Referral Code), and support cancelling a `Confirmed`
booking.

#### Scenario: Cancelling a booking reverses commission
- Given: A booking with status `Confirmed` exists
- When: Tenant clicks "Cancel" in the booking's ActionMenu
- Then: `tenantApi.cancelBooking(tenantId, bookingId, reason)` is called; success toast reads "Booking cancelled — commissions reversed" (see SPEC-COMMISSION-REFUND-REVERSAL safeguard in `commission/spec.md`)

#### Scenario: Cancel action only available on confirmed bookings
- Given: The bookings table renders
- When: A row's status is not `Confirmed`
- Then: No ActionMenu/Cancel action is rendered for that row

---

### Requirement: SPEC-TENANT-STAFF — Staff Agent Management Page

The staff page MUST list the tenant's staff agents (Name/Email, Referral Code, Status) fetched live
from `tenantApi.getStaffAgents(tenantId)`, support creating a staff agent (Name, Email), and support
rotating an agent's referral code.

#### Scenario: Rotating a staff agent's referral code
- Given: A staff agent row exists
- When: Tenant clicks the rotate-code icon button
- Then: `tenantApi.rotateStaffAgentCode(tenantId, agentId)` is called; success toast shows the new code; list reloads (supports the code-rotation fraud safeguard in `commission/spec.md`)

---

### Requirement: SPEC-TENANT-COMMISSION — Commission Rule & Statement Page

The commission page MUST have two sections: (1) a commission rules table (Type, Trigger, Rate,
Min. Value, Status) with a create-rule form (Trigger, Amount, Is Percentage, Minimum Threshold),
both backed by `tenantApi.getCommissionRules` / `tenantApi.configureCommissionRule`; (2) an agent
statement lookup by Agent ID showing Accrued/Finalized/Paid/Reversed totals and line items, backed
by `tenantApi.getCommissionStatement`.

#### Scenario: Creating a commission rule
- Given: Tenant fills the New Commission Rule form (Trigger, Amount, Is Percentage, Min. Booking Value)
- When: Form is submitted
- Then: `tenantApi.configureCommissionRule(tenantId, form)` is called; on success the form closes and the rules table reloads

#### Scenario: Looking up an agent's commission statement
- Given: Tenant enters an Agent ID and clicks "Load"
- When: `tenantApi.getCommissionStatement(tenantId, agentId)` resolves
- Then: Four summary tiles (Accrued, Finalized, Paid, Reversed) and a line-item table (Description, Amount, Status) render below the lookup input

---

## Key Reusable Components

Shared 1:1 with the admin portal — see `openspec/specs/admin-portal/spec.md` § Key Reusable
Components for `StatusBadge`, `ActionMenu`, `Modal`, `Select`, and the dark-mode pattern. The
tenant dashboard maintains its own copies of these components (not a shared package) under
`web/tenant-dashboard/src/components/`, so changes must be applied to both apps independently.

### Data Source Pattern
Unlike the admin portal's Billing/Users pages (which currently render mock client-side data, see
SPEC-ADMIN-BILLING / SPEC-ADMIN-USERS), all tenant-dashboard pages listed above are wired live to
`tenantApi` and read `tenantId` from `localStorage.getItem("litxus_tenant_id")`.
