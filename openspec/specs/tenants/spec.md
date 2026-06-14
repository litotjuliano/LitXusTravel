# Tenants

## Overview

Tenants are travel agent businesses (e.g., "Travel Pro Agency") operating on the platform.
Each tenant gets a white-label website on a subdomain, a portal to manage packages and inquiries,
and optional commission rules for their staff. Admins create and manage tenants; each tenant's
data is completely isolated from other tenants.

## Requirements

### Requirement: SPEC-ADMIN-004 — Create Tenant with Auto-provisioning

An authenticated Admin MUST be able to create a new tenant. The system MUST auto-generate a
unique subdomain, create an admin login, and start a trial subscription.

#### Scenario: Admin creates a tenant with valid data
- Given: An authenticated Admin
- When: POST /api/v1/admin/tenants with name and contactEmail
- Then: 201 Created with tenant id, name, subdomain (auto-generated), and initial credentials

#### Scenario: Duplicate company name / subdomain conflict
- Given: A tenant "Travel Pro" already exists
- When: Admin creates another tenant with the same name
- Then: The system SHOULD generate a unique subdomain variation (e.g., `travel-pro-2`)

#### Scenario: Missing required fields
- Given: An authenticated Admin
- When: POST /api/v1/admin/tenants with missing name or contactEmail
- Then: 400 Bad Request with validation details

---

### Requirement: SPEC-ADMIN-005 — List Tenants with Pagination

An authenticated Admin MUST be able to list all tenants with filtering and pagination.

#### Scenario: Admin lists all tenants
- Given: An authenticated Admin
- When: GET /api/v1/admin/tenants
- Then: 200 OK with `{ data: [tenants], pagination: { page, pageSize, totalCount, totalPages } }`

#### Scenario: Filter by status
- Given: Tenants in Active, Trial, and Archived states
- When: GET /api/v1/admin/tenants?status=Trial
- Then: 200 OK with only Trial tenants

#### Scenario: Sort by name
- Given: Multiple tenants
- When: GET /api/v1/admin/tenants?sortBy=name&sortOrder=asc
- Then: 200 OK with tenants sorted A→Z

---

### Requirement: SPEC-TENANT-SETTINGS — Get and Update Tenant Settings

An authenticated Agent MUST be able to read and update their tenant's settings,
including the default currency used for all package pricing display.

#### Scenario: Agent gets tenant settings
- Given: An authenticated Agent
- When: GET /api/v1/admin/tenants/{tenantId}/settings
- Then: 200 OK with `{ defaultCurrency: "MYR" }` (or configured value)

#### Scenario: Agent updates default currency
- Given: An authenticated Agent
- When: PUT /api/v1/admin/tenants/{tenantId}/settings with `{ defaultCurrency: "USD" }`
- Then: 200 OK; subsequent package listings use USD as display currency

---

### Requirement: SPEC-MULTI-TENANCY — Tenant Data Isolation

The system MUST enforce strict data isolation between tenants.
No tenant MUST ever be able to access another tenant's packages, inquiries, or data.

#### Scenario: Agent accesses another tenant's packages
- Given: Agent A authenticated with tenantId = "tenant-a"
- When: GET /api/v1/tenants/tenant-b/packages
- Then: 403 Forbidden — tenant ID in URL does not match JWT claim

#### Scenario: Global query filters prevent data leaks
- Given: A database with data from multiple tenants
- When: Any query runs through the application
- Then: EF Core global query filters automatically apply TenantId = currentTenant.Id to every query

#### Scenario: Admin bypasses tenant filter for cross-tenant visibility
- Given: An authenticated Admin (SuperAdmin or Platform Admin)
- When: GET /api/v1/admin/tenants/{tenantId}/packages
- Then: 200 OK — Admin role is authorized to view any tenant's data

---

### Requirement: SPEC-SUBDOMAIN — Tenant Subdomain Resolution

The public website and tenant portal MUST resolve the correct tenant from the request subdomain.
A request to `travelpro.lvh.me` MUST scope all data to the "Travel Pro" tenant.

#### Scenario: Valid subdomain resolves to tenant
- Given: A tenant with subdomain "travelpro"
- When: Request arrives at `travelpro.lvh.me`
- Then: Middleware sets ICurrentTenant to the Travel Pro tenant; all queries filter by that tenant

#### Scenario: Unknown subdomain
- Given: No tenant with subdomain "unknown"
- When: Request arrives at `unknown.lvh.me`
- Then: Public endpoints return 404 "Website not found"
