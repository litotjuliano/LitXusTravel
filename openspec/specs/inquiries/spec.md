# Inquiries & CRM

## Overview

Inquiries are customer leads submitted via the public website. Each inquiry is scoped to a
specific tenant and goes through a CRM lifecycle: New → Contacted → Quoted → Booked (or Lost).
Every status change creates a CrmActivity record forming a full audit timeline.
Agents manage inquiries through their portal; the admin can view aggregated stats across all tenants.

## Requirements

### Requirement: SPEC-PUBLIC-004 — Submit Customer Inquiry

A visitor on the public website MUST be able to submit an inquiry without authentication.
The inquiry MUST be scoped to the tenant whose subdomain was used.
The system MUST notify the agent immediately after submission.

#### Scenario: Customer submits a valid inquiry
- Given: A visitor on travelpro.lvh.me/packages with a package selected
- When: POST /api/v1/public/websites/{subdomain}/inquiries with name, email, phone, message (≥ 10 chars)
- Then: 201 Created with inquiryId and status: "new"; agent receives notification

#### Scenario: Missing required fields
- Given: A visitor on the public website
- When: POST inquiry with missing customerName or customerEmail or message
- Then: 400 Bad Request with validation details

#### Scenario: Message too short
- Given: A visitor submitting a message with fewer than 10 characters
- When: POST inquiry
- Then: 422 Unprocessable — "Message must be at least 10 characters"

#### Scenario: Unknown subdomain
- Given: Subdomain does not correspond to any tenant
- When: POST /api/v1/public/websites/{unknown}/inquiries
- Then: 404 Not Found

---

### Requirement: SPEC-TENANT-008 — Update Inquiry Status

An authenticated Agent MUST be able to update the status of an inquiry in their tenant.
The system MUST create a CrmActivity record for every status change.
The system MUST set milestone timestamps automatically (FirstResponseAt, QuotedAt, ClosedAt).

#### Scenario: Agent marks inquiry as Contacted
- Given: An inquiry in "New" status
- When: PUT /api/v1/tenants/{tenantId}/inquiries/{inquiryId} with status: "Contacted"
- Then: 200 OK; inquiry.status = "Contacted"; FirstResponseAt is set; CrmActivity record created

#### Scenario: Agent marks inquiry as Booked
- Given: An inquiry in "Quoted" status
- When: Agent sets status to "Booked"
- Then: 200 OK; ClosedAt timestamp is set; CrmActivity record with type "StatusChanged" created

#### Scenario: Tenant mismatch
- Given: An Agent from tenant A trying to update tenant B's inquiry
- When: PUT /api/v1/tenants/{tenantB}/inquiries/{inquiryId}
- Then: 403 Forbidden

#### Scenario: Inquiry not found
- Given: An authenticated Agent
- When: PUT with a non-existent inquiryId
- Then: 404 Not Found

---

### Requirement: SPEC-TENANT-010 — Get Tenant Inquiry Stats

An authenticated Agent MUST be able to retrieve a summary of their inquiry pipeline,
including counts by status and a conversion rate.

#### Scenario: Agent views their inquiry stats
- Given: An authenticated Agent with inquiries in various states
- When: GET /api/v1/tenants/{tenantId}/inquiries/stats
- Then: 200 OK with counts: `{ new, contacted, quoted, booked, lost, conversionRate, thisMonth }`

#### Scenario: Tenant with no inquiries
- Given: A new tenant with zero inquiries
- When: GET /api/v1/tenants/{tenantId}/inquiries/stats
- Then: 200 OK with all counts = 0, conversionRate = 0

---

### Requirement: SPEC-ADMIN-010 — Get Aggregated Inquiry Stats (Admin)

An authenticated Admin MUST be able to view inquiry statistics aggregated across all tenants.

#### Scenario: Admin views platform-wide stats
- Given: An authenticated Admin
- When: GET /api/v1/admin/inquiries/stats
- Then: 200 OK with total counts across all tenants: `{ total, new, contacted, quoted, booked, lost }`

---

### Requirement: SPEC-CRM-ISOLATION — Inquiry Tenant Isolation

An Agent MUST only be able to see and modify inquiries belonging to their own tenant.
Inquiry data MUST never be visible across tenant boundaries.

#### Scenario: Agent lists their inquiries
- Given: Multiple tenants each with their own inquiries in the database
- When: GET /api/v1/tenants/{tenantId}/inquiries
- Then: Only inquiries for that specific tenantId are returned — never another tenant's inquiries

#### Scenario: Inquiry submitted to Tenant A cannot be accessed by Tenant B
- Given: An inquiry with TenantId = "tenant-a"
- When: Agent from "tenant-b" tries to GET or PUT that inquiry
- Then: 403 Forbidden or 404 Not Found (never 200)
