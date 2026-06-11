# LitXusTravel - Specification for Claude Code

## Executive Summary

**LitXusTravel** is a multi-tenant SaaS platform for travel package distribution.

**Core Innovation:** Package Synchronization Engine
- Tour operators create master packages
- Travel agents sync and customize without affecting master
- Master updates auto-sync (protecting agent customizations)
- Multi-tenant with complete data isolation

**Timeline:** 12 weeks to MVP

---

## Technology Stack

- **Backend:** C# + ASP.NET Core 8
- **Database:** MS SQL Server (Code First EF Core)
- **Architecture:** Clean Architecture
- **API Documentation:** OpenAPI/Swagger (auto-generated)
- **Audit:** Application-level (track all changes)
- **Version Control:** GitHub (branch strategy: main, develop, feature/*, bugfix/*)

---

## Project Structure

```
LitXusTravel.sln
├── src/
│   ├── LitXusTravel.Core/           (Domain layer)
│   ├── LitXusTravel.Application/    (Business logic)
│   ├── LitXusTravel.Infrastructure/ (Data access)
│   └── LitXusTravel.API/            (Controllers, Swagger)
├── tests/
│   ├── LitXusTravel.UnitTests/
│   ├── LitXusTravel.IntegrationTests/
│   └── LitXusTravel.E2E/
└── docs/
```

---

## Database Entities

**Core Entities:**
1. **Operator** - Tour operator (creates master packages)
2. **MasterPackage** - Source of truth (operator-owned)
3. **PackageOverride** - Agent customizations (CRITICAL)
4. **Tenant** - Travel agent account
5. **TenantUser** - Agent login
6. **Inquiry** - Customer lead
7. **CrmActivity** - Lead activity tracking
8. **Subscription** - Billing
9. **AuditLog** - Change tracking (application-level)
10. **Notification** - System messages

**Relationships:**
- Operator → MasterPackage (1:N)
- MasterPackage → PackageOverride (1:N)
- Tenant → PackageOverride (1:N)
- Tenant → TenantUser (1:N)
- Tenant → Inquiry (1:N)
- Inquiry → CrmActivity (1:N)

**Key Features:**
- All tables have multi-tenancy support (TenantId)
- Soft delete (DeletedAt field)
- Audit fields (CreatedBy, UpdatedBy, CreatedAt, UpdatedAt)
- Code First: Models define database

---

## OpenSpecs - Business Specifications

OpenSpecs define WHAT each endpoint must do. Claude Code implements per spec.

### Admin APIs

#### SPEC-ADMIN-001: Create Master Package

```
Endpoint: POST /api/admin/packages
Auth: Required (Admin)

Input:
- operatorId (uuid)
- title (string, required)
- subtitle (string, optional)
- description (string)
- durationDays (int, > 0)
- priceBase (decimal, > 0)
- currency (string, default: MYR)
- itinerary (json)
- highlights (json)
- imageHeroUrl (url)
- destinationPrimary (string)

Output (201):
- id, operatorId, title, price, status, createdAt

Errors:
- 400: Invalid input (missing fields, price ≤ 0)
- 401: Unauthorized
- 403: Forbidden (not admin)

Rules:
- Status defaults to 'draft'
- Audit: Log creation (who, what, when)
- Event: PackageCreated
```

#### SPEC-ADMIN-002: List Master Packages

```
Endpoint: GET /api/admin/packages
Auth: Required (Admin)

Query:
- status (draft|published|archived)
- destination (string)
- page (int, default: 1)
- pageSize (int, default: 20)
- sortBy (createdAt|title|price)
- sortOrder (asc|desc)

Output (200):
- data: [packages]
- pagination: {page, pageSize, totalCount}

Errors:
- 400: Invalid query params
- 401: Unauthorized
```

#### SPEC-ADMIN-003: Publish Master Package

```
Endpoint: POST /api/admin/packages/{packageId}/publish
Auth: Required (Admin)

Input: Empty (action is implicit)

Output (200):
- id, status: "published", publishedAt

Errors:
- 400: Cannot publish (not draft, validation errors)
- 404: Package not found

Rules:
- Only draft → published
- Audit: Log publish (who, packageId, timestamp)
- Event: PackagePublished
- Notify operators via email
```

#### SPEC-ADMIN-004: Create Tenant (Auto-provision)

```
Endpoint: POST /api/admin/tenants
Auth: Required (Admin)

Input:
- companyName (string, required)
- email (string, required)
- phone (string)
- country (string)

Output (201):
- id, companyName, subdomain, email, credentials

Process:
1. Generate unique subdomain (auto-generated)
2. Create Tenant record
3. Create TenantUser (admin login)
4. Create Subscription (trial)
5. Send welcome email with credentials

Errors:
- 400: Invalid input
- 409: Subdomain exists

Rules:
- Subdomain format: company-name-{number}
- Generate temp password
- IsTrial = true, TrialEndsAt = +30 days
- Audit: Log provisioning

Event: TenantProvisioned
```

#### SPEC-ADMIN-005: List Tenants

```
Endpoint: GET /api/admin/tenants
Auth: Required (Admin)

Query:
- status (active|paused|archived)
- page (int)
- pageSize (int)

Output (200):
- data: [tenants with stats]
- pagination

Stats per tenant:
- syncedPackagesCount
- totalInquiries
- conversionRate
```

### Tenant (Agent) APIs

#### SPEC-TENANT-001: Sync Master Package to Tenant

```
Endpoint: POST /api/tenants/{tenantId}/packages/sync
Auth: Required (Agent)
Path: tenantId must match JWT tenant claim

Input:
- masterPackageIds: [uuid, uuid, ...]

Output (200):
- synced: [{id, masterPackageId, syncedAt}]
- failed: [{masterPackageId, reason}]

Process:
1. Check each package is published
2. Check not already synced (no duplicate overrides)
3. Create PackageOverride with ALL fields NULL
   - NULL means "use master value"
   - NOT NULL means "use agent customization"
4. Publish event: PackageSynced

Errors:
- 400: Invalid package IDs, empty list
- 403: Tenant mismatch
- 404: Tenant/package not found

Rules:
- Tenant can only sync published packages
- No limit on syncs (MVP)
- Audit: Log sync (who, packages, tenantId)
```

#### SPEC-TENANT-002: Get Synced Packages (List)

```
Endpoint: GET /api/tenants/{tenantId}/packages
Auth: Required (Agent)

Query:
- page, pageSize
- sortBy (title|price|syncedAt)

Output (200):
- data: [resolved packages]
- pagination

Resolved Package includes:
- masterData (from MasterPackage)
- overrideData (from PackageOverride)
- merged: Using NULL logic
  * If override.field IS NULL → use master.field
  * If override.field IS NOT NULL → use override.field

Info:
- syncedAt
- isCustomized (true if any field overridden)
```

#### SPEC-TENANT-003: Get Single Package (Resolved)

```
Endpoint: GET /api/tenants/{tenantId}/packages/{packageId}
Auth: Required (Agent)

Output (200):
- Full resolved package
- master: original values
- override: agent customizations
- merged: resolved (NULL logic applied)
- customized: which fields are customized

Display which fields agent customized.
```

#### SPEC-TENANT-004: Update Package Override

```
Endpoint: PUT /api/tenants/{tenantId}/packages/{packageId}/override
Auth: Required (Agent)

Input (partial):
- title (optional)
- price (optional, > 0)
- imageHeroUrl (optional)
- contactPhone (optional)
- contactWhatsapp (optional)

Output (200):
- Updated override

Process:
1. Validate only synced packages
2. Partial update (only provided fields)
3. Publish event: OverrideUpdated
4. Invalidate website cache

Errors:
- 400: Invalid data, package not synced
- 403: Tenant mismatch
- 404: Not found

Rules:
- Cannot update if locked
- Audit: Log updates (old → new)
```

#### SPEC-TENANT-005: Unsync Package

```
Endpoint: DELETE /api/tenants/{tenantId}/packages/{packageId}
Auth: Required (Agent)

Output (200):
- Success message

Process:
1. Delete PackageOverride
2. Publish event: PackageUnsynced
3. Invalidate website cache

Errors:
- 403: Tenant mismatch
- 404: Not found
```

#### SPEC-TENANT-006: List Inquiries

```
Endpoint: GET /api/tenants/{tenantId}/inquiries
Auth: Required (Agent)

Query:
- status (new|contacted|quoted|booked|lost)
- page, pageSize
- sortBy (createdAt|updatedAt)

Output (200):
- data: [inquiries]
- pagination
- stats: {new, contacted, quoted, booked, lost}

Show:
- Customer name, email, phone
- Package name
- Status
- CreatedAt
- FirstResponseAt (if contacted)
```

#### SPEC-TENANT-007: Get Inquiry Detail

```
Endpoint: GET /api/tenants/{tenantId}/inquiries/{inquiryId}
Auth: Required (Agent)

Output (200):
- Full inquiry details
- All CrmActivity records (timeline)

Timeline shows:
- Created
- Contacted
- Quoted
- Status changes
- Notes added

Errors:
- 403: Tenant mismatch
- 404: Not found
```

#### SPEC-TENANT-008: Update Inquiry Status

```
Endpoint: PUT /api/tenants/{tenantId}/inquiries/{inquiryId}
Auth: Required (Agent)

Input:
- status (new|contacted|quoted|booked|lost)

Output (200):
- Updated inquiry

Process:
1. Update status
2. Create CrmActivity record
3. Set timestamps:
   - contacted → FirstResponseAt
   - quoted → QuotedAt
   - booked/lost → ClosedAt
4. Audit: Log status change

Errors:
- 403: Tenant mismatch
- 404: Not found
```

#### SPEC-TENANT-009: Add Inquiry Activity (Note)

```
Endpoint: POST /api/tenants/{tenantId}/inquiries/{inquiryId}/activity
Auth: Required (Agent)

Input:
- type (note|contacted|quoted)
- notes (string)

Output (201):
- Created activity

Process:
1. Create CrmActivity
2. Audit: Log activity

Errors:
- 403: Tenant mismatch
- 404: Not found
```

#### SPEC-TENANT-010: Get Inquiry Stats

```
Endpoint: GET /api/tenants/{tenantId}/inquiries/stats
Auth: Required (Agent)

Output (200):
- new: count
- contacted: count
- quoted: count
- booked: count
- lost: count
- conversionRate: % (booked / total)
- avgResponseTime: minutes
- thisMonth: count
```

#### SPEC-TENANT-011: Get Audit Trail

```
Endpoint: GET /api/tenants/{tenantId}/audit
Auth: Required (Agent)

Query:
- entityType (Package|Inquiry)
- entityId (optional)
- days (30)
- page, pageSize

Output (200):
- data: [audit logs]
- pagination

Shows:
- Who (user)
- What (action: created|updated|deleted)
- When (timestamp)
- Changes (old → new)

Errors:
- 403: Tenant mismatch
```

### Public APIs (No Authentication)

#### SPEC-PUBLIC-001: Get Website Metadata

```
Endpoint: GET /api/public/websites/{subdomain}
Auth: Not required

Output (200):
- companyName
- logo
- primaryColor
- secondaryColor
- syncedPackagesCount

Errors:
- 404: Subdomain not found
```

#### SPEC-PUBLIC-002: List Packages (Website)

```
Endpoint: GET /api/public/websites/{subdomain}/packages
Auth: Not required

Query:
- category (optional)
- destination (optional)

Output (200):
- data: [resolved packages]

Note: Returns RESOLVED packages (master + overrides merged)
```

#### SPEC-PUBLIC-003: Get Package Detail (Website)

```
Endpoint: GET /api/public/websites/{subdomain}/packages/{packageId}
Auth: Not required

Output (200):
- Full resolved package
- Itinerary
- Images
- Price (with override)
- Contact (with override)
- WhatsApp link

Errors:
- 404: Subdomain or package not found
```

#### SPEC-PUBLIC-004: Submit Inquiry (Customer)

```
Endpoint: POST /api/public/websites/{subdomain}/inquiries
Auth: Not required (public form)

Input:
- customerName (string, required)
- customerEmail (string, required)
- customerPhone (string, required)
- masterPackageId (uuid, optional)
- message (string, required, min 10 chars)
- numberOfPax (int, optional)
- preferredTravelDates (string, optional)

Output (201):
- inquiryId
- status: "new"
- message: "Thank you. Agent will contact you."

Process:
1. Create Inquiry (status='new')
2. Create CrmActivity (type='created')
3. Publish event: InquiryCreated
4. Send notifications:
   - Email to agent
   - WhatsApp click-to-chat link
5. Audit: Log submission (minimal info)

Errors:
- 400: Missing fields, invalid format
- 404: Subdomain not found
- 422: Message too short

Rules:
- No authentication required
- Public endpoint
- Immediate agent notification
```

---

## Critical Business Rules

### Package Synchronization (Load-Bearing)

**NULL Semantics (CRITICAL):**
- Override field IS NULL → Use master value
- Override field IS NOT NULL → Use agent value
- Example: If agent sets price=5999, it stays 5999 even if master changes to 4999

**Master Updates:**
- When master package updates, sync to ALL agents
- Update only: lastMasterSyncAt timestamp
- DO NOT overwrite agent customizations (override fields)
- Protect agent's customizations

**Locked Packages:**
- If override.isLocked = true, no syncing happens
- Prevents accidental overwrites

### Multi-Tenancy (Security-Critical)

**Tenant Isolation:**
- Every query MUST filter by TenantId
- Cross-tenant access = 403 Forbidden
- Audit per tenant

**JWT Token:**
- Claims: userId, tenantId, role
- Validate tenantId in URL matches JWT claim
- No exceptions

### Audit Logging (Application-Level)

**Track:**
- Who (userId)
- What (action: created|updated|deleted, entity details)
- When (timestamp)
- Where (IP address, user agent)
- TenantId (for tenant isolation in audit)

**Changes:**
- Old values
- New values
- Queryable by entity, user, date range

**No Sensitive Data:**
- Don't log passwords, credit cards, API keys
- Don't log in public endpoints

### Notifications

**When to Notify:**
- Package synced → Agent (email)
- Package published → Operators (email)
- Inquiry created → Agent (email + WhatsApp)
- Inquiry status changed → Customer (email)

**WhatsApp:**
- Click-to-chat links
- Pre-formatted message
- Format: https://wa.me/{phone}?text={message}

---

## Week-by-Week Implementation

### Week 1-2: Foundation
- Create solution (Clean Architecture)
- Create entity models
- Setup DbContext (Code First)
- Create initial migration
- Setup Swagger/OpenAPI
- Create base repositories
- Setup audit logging infrastructure

### Week 3-4: Admin APIs
- Implement SPEC-ADMIN-001 to 005
- Create admin services
- Add OpenAPI documentation
- Create validators
- Add unit tests

### Week 5-6: Sync Engine (CRITICAL)
- Implement SPEC-TENANT-001, 002, 003, 004, 005
- Implement sync logic (NULL semantics)
- Implement override updates
- Create event handlers
- Create 9 critical tests for sync logic
- Implement audit logging for all sync operations

### Week 7-8: Website Provisioning
- Implement website rendering
- Implement subdomain resolution
- Implement caching
- Implement SPEC-PUBLIC-001, 002, 003
- Add tests

### Week 9-10: Inquiry & CRM
- Implement SPEC-TENANT-006, 007, 008, 009, 010
- Implement SPEC-PUBLIC-004
- Implement notification service
- Implement audit tracking for inquiries
- Add tests

### Week 11-12: Frontend & Testing
- Create Blazor/React dashboards
- Create tenant admin UI
- Create public website pages
- Create integration tests
- Create E2E tests
- Performance testing

---

## API Authentication & Authorization

**JWT Token:**
- Claims: userId, tenantId, role
- Header: Authorization: Bearer {token}

**Roles:**
- Admin (can access /api/admin/*)
- Agent (can access /api/tenants/{own-tenantId}/*)
- Public (no role needed for /api/public/*)

**Validation:**
- Every endpoint validates JWT token
- Every endpoint validates tenantId (path matches JWT claim)
- Throw 401 if unauthorized, 403 if forbidden

---

## Error Handling

**Standard Error Response:**
```
{
  "status": 400,
  "error": "BadRequest",
  "message": "Price must be greater than 0",
  "details": [optional error details]
}
```

**HTTP Status Codes:**
- 200: Success
- 201: Created
- 400: Bad Request (validation)
- 401: Unauthorized (no token)
- 403: Forbidden (tenant mismatch, wrong role)
- 404: Not Found
- 409: Conflict (duplicate)
- 422: Unprocessable (semantic error)
- 500: Server Error

---

## GitHub Branch Strategy

**Branches:**
- **main** - Production (protected, requires PR review and tests passing)
- **develop** - Integration (base for feature branches)
- **feature/*** - Feature development (branch from develop, merge back)
- **bugfix/*** - Bug fixes (branch from develop, merge back)
- **release/*** - Release candidates (from develop, merge to main)

**Commit Convention:**
```
[TYPE] Short description

TYPE: feat|fix|docs|refactor|test|chore

Example:
[feat] Implement package sync endpoint
[fix] Fix multi-tenancy isolation bug
[test] Add sync engine tests
```

---

## Success Criteria (Week 12)

**Code:**
- ✅ 50+ C# classes
- ✅ Clean Architecture implemented
- ✅ Code First EF Core setup
- ✅ All migrations created

**Features:**
- ✅ All OpenSpecs implemented
- ✅ Multi-tenancy enforced (100%)
- ✅ Sync engine working (NULL semantics)
- ✅ Audit logging on all changes
- ✅ OpenAPI documentation complete

**Testing:**
- ✅ 30+ comprehensive tests
- ✅ >80% test coverage
- ✅ 9 critical sync engine tests passing
- ✅ Integration tests passing

**Security:**
- ✅ JWT authentication
- ✅ Tenant isolation verified
- ✅ Parameterized queries (no SQL injection)
- ✅ Input validation on all endpoints

**Documentation:**
- ✅ OpenAPI spec complete
- ✅ Architecture documented
- ✅ Setup guide for developers

---

## Instructions for Claude Code

**When Claude Code uses this spec:**

1. **Read all OpenSpecs** (Section: OpenSpecs - Business Specifications)
   - Understand what each endpoint must do
   - Understand business rules
   - Understand error handling

2. **Implement per OpenSpec**
   - Create entity models
   - Create services
   - Create controllers with OpenAPI documentation
   - Follow OpenSpecs exactly

3. **Add Audit Logging**
   - Track changes at application level
   - Include user, timestamp, changes

4. **Test Comprehensively**
   - Unit tests for services
   - Integration tests for full flows
   - 9 critical tests for sync engine (Week 5-6)

5. **Follow Clean Architecture**
   - Separate concerns
   - Dependency injection
   - Testable code

6. **Document with OpenAPI**
   - Auto-generate from code
   - Include request/response examples
   - Include error codes

7. **Multi-Tenancy**
   - Filter ALL queries by TenantId
   - Validate JWT tenantId matches URL
   - Test isolation

8. **GitHub Strategy**
   - Follow branch strategy
   - Follow commit conventions
   - Meaningful commit messages

---

## Summary

**This specification is Claude Code's instruction set.**

It provides:
- ✅ What to build (OpenSpecs)
- ✅ How data relates (entities)
- ✅ Business rules
- ✅ Error handling
- ✅ Audit requirements
- ✅ Timeline
- ✅ Testing strategy

**Claude Code should:**
- ✅ Implement EVERY OpenSpec exactly
- ✅ Handle EVERY error case
- ✅ Add audit logging to ALL changes
- ✅ Create comprehensive tests
- ✅ Document with OpenAPI
- ✅ Follow Clean Architecture
- ✅ Enforce multi-tenancy

**Result:** Production-ready LitXusTravel in 12 weeks.

---

## Ready?

Share this specification with Claude Code and ask it to generate week by week.

Claude Code will handle:
- Clean Architecture implementation
- Code organization
- Design patterns
- Best practices
- Detailed implementation

You define WHAT (this spec), Claude Code builds HOW.
