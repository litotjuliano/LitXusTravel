# Packages

## Overview

Master packages are created by admins and form the shared catalog. Travel agents (tenants) sync
packages to their portal and may apply overrides (custom title, price, image, contact details).
The merged result (master + override with NULL-coalescing) is what the public website displays.
Tenants may also create portal-only packages (not from master) or extend packages to the master
catalog for other tenants to discover.

## Requirements

### Requirement: SPEC-ADMIN-001 — Create Master Package

An authenticated Admin MUST be able to create a new master package in the shared catalog.
The package starts in `Draft` visibility and MUST NOT appear on any public website until published.

#### Scenario: Admin creates a valid package
- Given: An authenticated Admin user
- When: POST /api/v1/admin/packages with title, destination, basePrice > 0, durationDays > 0
- Then: 201 Created with package id, title, destination, basePrice, currency, durationDays, visibility: "Draft"

#### Scenario: Missing required fields
- Given: An authenticated Admin user
- When: POST /api/v1/admin/packages with missing title or destination or basePrice ≤ 0
- Then: 400 Bad Request with validation error details

#### Scenario: Unauthenticated request
- Given: No JWT token
- When: POST /api/v1/admin/packages
- Then: 401 Unauthorized

---

### Requirement: SPEC-ADMIN-002 — List Master Packages

An authenticated Admin MUST be able to list all master packages with filtering, sorting, and pagination.

#### Scenario: List with default pagination
- Given: An authenticated Admin with packages in the database
- When: GET /api/v1/admin/packages
- Then: 200 OK with `{ data: [...], pagination: { page, pageSize, totalCount, totalPages } }`

#### Scenario: Filter by visibility status
- Given: An authenticated Admin
- When: GET /api/v1/admin/packages?status=Published
- Then: 200 OK with only Published packages

#### Scenario: Sort by price descending
- Given: An authenticated Admin
- When: GET /api/v1/admin/packages?sortBy=price&sortOrder=desc
- Then: 200 OK with packages sorted highest price first

---

### Requirement: SPEC-ADMIN-003 — Publish Master Package

An authenticated Admin MUST be able to publish a Draft master package, making it visible to the
public and available for tenant sync. A package MUST have a description before it can be published.

#### Scenario: Publish a draft package with description
- Given: An authenticated Admin and a package in Draft state with a non-empty description
- When: POST /api/v1/admin/packages/{id}/publish
- Then: 200 OK; package visibility changes to "Published"

#### Scenario: Publish package without description
- Given: A package in Draft state with no description
- When: POST /api/v1/admin/packages/{id}/publish
- Then: 400 Bad Request — "Package must have a description before publishing"

#### Scenario: Publish already-published package
- Given: A package already in Published state
- When: POST /api/v1/admin/packages/{id}/publish
- Then: 400 Bad Request — "Only draft packages can be published"

#### Scenario: Package not found
- Given: An authenticated Admin
- When: POST /api/v1/admin/packages/{non-existent-id}/publish
- Then: 404 Not Found

---

### Requirement: SPEC-ADMIN-006 — Sync Packages to Tenant (Admin-initiated)

An authenticated Admin MUST be able to sync one or more master packages to a specific tenant,
creating TenantPackage records with empty overrides.

#### Scenario: Admin syncs published packages to a tenant
- Given: Published master packages and an existing tenant
- When: POST /api/v1/admin/tenants/{tenantId}/packages/sync with masterPackageIds list
- Then: 200 OK with sync results including synced count

#### Scenario: Attempt to sync draft package
- Given: A package in Draft visibility
- When: Admin attempts to sync it to a tenant
- Then: The package is skipped or returns an error — Draft packages MUST NOT be synced

---

### Requirement: SPEC-ADMIN-007 — Get Tenant Packages with Overrides (Admin view)

An authenticated Admin MUST be able to view all packages synced to a specific tenant,
including override values and merge resolution.

#### Scenario: Admin views tenant package list
- Given: A tenant with synced packages (some with overrides)
- When: GET /api/v1/admin/tenants/{tenantId}/packages
- Then: 200 OK with resolved packages showing merged master + override values

---

### Requirement: SPEC-ADMIN-008 — Update Package Override (Admin)

An authenticated Admin MUST be able to update the override fields for a tenant's package.

#### Scenario: Admin updates a price override
- Given: A tenant package with a synced master
- When: PUT /api/v1/admin/tenants/{tenantId}/packages/{packageId}/override with price: 5999
- Then: 200 OK; resolved package shows overridden price; master price unchanged

---

### Requirement: SPEC-ADMIN-009 — Unsync Package from Tenant

An authenticated Admin MUST be able to remove a synced package from a tenant's catalog,
deleting the TenantPackage and its PackageOverride.

#### Scenario: Admin unsyncs a package
- Given: A tenant with a synced package
- When: DELETE /api/v1/admin/tenants/{tenantId}/packages/{packageId}
- Then: 200 OK; package no longer appears in tenant's catalog

---

### Requirement: SPEC-TENANT-001 — Sync Master Packages to Tenant (Agent-initiated)

An authenticated Agent MUST be able to add published master packages to their catalog.
Only packages with `Published` visibility may be synced.
Each package MUST create a PackageOverride with all fields NULL (not customized).

#### Scenario: Agent syncs valid published packages
- Given: An authenticated Agent and a list of published master package IDs
- When: POST /api/v1/tenants/{tenantId}/packages/sync with masterPackageIds
- Then: 200 OK; packages appear in agent's catalog with no overrides applied

#### Scenario: Agent syncs a Draft package
- Given: A package in Draft state
- When: Agent attempts to sync it
- Then: The package is excluded from results — Draft packages MUST NOT be synced

#### Scenario: Tenant ID mismatch
- Given: An Agent whose JWT tenantId differs from the URL tenantId
- When: POST /api/v1/tenants/{otherId}/packages/sync
- Then: 403 Forbidden

---

### Requirement: SPEC-TENANT-002 — Get Agent's Package Catalog

An authenticated Agent MUST be able to list all packages in their catalog with pagination and sorting.
The response MUST use NULL-coalescing merge: if an override field is NULL, use the master value.

#### Scenario: Agent lists their packages
- Given: A tenant with owned packages and synced packages (some with overrides)
- When: GET /api/v1/tenants/{tenantId}/packages
- Then: 200 OK with resolved packages; overridden fields show agent values, others show master values

#### Scenario: Sort by title ascending
- Given: A tenant with multiple packages
- When: GET /api/v1/tenants/{tenantId}/packages?sortBy=title&sortOrder=asc
- Then: 200 OK with packages sorted A→Z by resolved title

#### Scenario: Visibility field in response
- Given: A tenant with an extended package (CreatedByTenantId = tenantId) still in Draft
- When: GET /api/v1/tenants/{tenantId}/packages
- Then: The package appears with `visibility: "Draft"` so the agent knows it is not yet public

---

### Requirement: SPEC-TENANT-004 — Update Package Override

An authenticated Agent MUST be able to partially update their package override.
Only provided fields SHOULD be updated — omitted fields MUST remain NULL (not changed).
A locked package MUST NOT be overridable.

#### Scenario: Agent overrides the price
- Given: A synced package with no overrides
- When: PUT /api/v1/tenants/{tenantId}/packages/{packageId}/override with price: 4999
- Then: 200 OK; resolved price is 4999; other fields still use master values

#### Scenario: Agent removes a price override (sets back to master)
- Given: A package with an existing price override
- When: PUT /api/v1/tenants/{tenantId}/packages/{packageId}/override with price: null
- Then: 200 OK; resolved price reverts to master's price

#### Scenario: Override locked package
- Given: A package with isLocked = true
- When: Agent tries to update override
- Then: 400 Bad Request — "Package is locked and cannot be customized"

---

### Requirement: SPEC-TENANT-005 — Unsync Package

An authenticated Agent MUST be able to remove a synced package from their catalog.
The TenantPackage and its PackageOverride MUST be deleted.

#### Scenario: Agent unsyncs a package
- Given: A tenant with a synced package
- When: DELETE /api/v1/tenants/{tenantId}/packages/{packageId}
- Then: 200 OK; package no longer in tenant catalog; override is deleted

---

### Requirement: SPEC-TENANT-006 — Create Tenant-Owned Package

An authenticated Agent MUST be able to create a package that is exclusive to their portal.
This package has no master; all data lives in the PackageOverride.
Optionally, the agent MAY extend the package to the master catalog (ExtendToMaster flag).

#### Scenario: Agent creates a portal-only package
- Given: An authenticated Agent
- When: POST /api/v1/tenants/{tenantId}/packages with title, destination, price, duration and ExtendToMaster: false
- Then: 201 Created; package appears in agent's catalog as "Owned"; not visible to other tenants

#### Scenario: Agent creates and extends to master catalog
- Given: An authenticated Agent
- When: POST /api/v1/tenants/{tenantId}/packages with ExtendToMaster: true
- Then: 201 Created; a master Package record is created with CreatedByTenantId = tenantId; package starts as Draft

---

### Requirement: SPEC-TENANT-007 — Generate Package Photo from Unsplash

An authenticated Agent MUST be able to generate a featured image for a package using Unsplash,
based on the package's destination and category. The generated URL is stored in the PackageOverride.

#### Scenario: Photo generated successfully
- Given: A package with a destination set and a valid Unsplash API key configured
- When: POST /api/v1/tenants/{tenantId}/packages/{packageId}/generate-photo
- Then: 200 OK with `{ featuredImageUrl: "https://images.unsplash.com/..." }`; override updated

#### Scenario: No Unsplash key configured
- Given: Unsplash API key is empty in configuration
- When: POST /api/v1/tenants/{tenantId}/packages/{packageId}/generate-photo
- Then: 400 Bad Request — "Photo generation unavailable. Configure the Unsplash API key."

---

### Requirement: SPEC-ADMIN-PHOTO — Generate Package Photo (Admin)

An authenticated Admin MUST be able to generate a featured image for a master package using Unsplash.
The generated URL is stored directly on the master Package entity via SetFeaturedImage().

#### Scenario: Admin generates photo for master package
- Given: A master package with a destination set and a valid Unsplash API key
- When: POST /api/v1/admin/packages/{id}/generate-photo
- Then: 200 OK with `{ featuredImageUrl: "..." }`; master package FeaturedImageUrl updated

---

### Requirement: SPEC-TENANT-PUBLISH — Publish Tenant-Extended Package

An authenticated Agent MUST be able to publish a package they extended to the master catalog
(ExtendToMaster), changing its visibility from Draft to Published so it appears on the public website.
The package MUST have a description before publishing.

#### Scenario: Agent publishes their extended Draft package
- Given: An extended package (CreatedByTenantId = tenantId) with visibility Draft and a description
- When: POST /api/v1/tenants/{tenantId}/packages/{packageId}/publish
- Then: 200 OK — package visibility becomes Published; appears on public website

#### Scenario: Agent tries to publish a portal-only owned package
- Given: A portal-only package (IsOwnedPackage = true, no master)
- When: POST /api/v1/tenants/{tenantId}/packages/{packageId}/publish
- Then: 400 Bad Request — "Portal-only packages cannot be published through this endpoint"

#### Scenario: Agent tries to publish another tenant's package
- Given: A master package extended by a different tenant
- When: Current agent tries to publish it
- Then: 400 Bad Request — "You can only publish packages you own"
