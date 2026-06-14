# Authentication & Authorization

## Overview

The platform uses JWT Bearer tokens for all authenticated endpoints. Tokens carry role and
tenant claims that are validated per request. The role hierarchy is:
`SuperAdmin → Admin → Agent (Tenant)`. Every API request that requires authentication MUST
validate both the role claim and, where applicable, the tenant claim against the URL parameter.

## Requirements

### Requirement: SPEC-AUTH-001 — Login

Any user with valid credentials MUST be able to authenticate and receive a JWT token.
The token MUST embed the user's role, tenantId (if applicable), and subject (userId).

#### Scenario: Agent logs in successfully
- Given: A tenant agent user with email and password in the database
- When: POST /api/v1/auth/login with valid email and password
- Then: 200 OK with `{ token, expiresAt, user: { id, email, firstName, lastName, role, tenantId } }`

#### Scenario: Admin logs in successfully
- Given: An admin user (no tenantId)
- When: POST /api/v1/auth/login with valid credentials
- Then: 200 OK with token; JWT claims include `role: "Admin"` but no `tenantId` claim

#### Scenario: Invalid credentials
- Given: Wrong password or unknown email
- When: POST /api/v1/auth/login
- Then: 401 Unauthorized — "Invalid credentials"

#### Scenario: Missing fields
- Given: Empty email or password
- When: POST /api/v1/auth/login
- Then: 400 Bad Request with validation error

---

### Requirement: SPEC-AUTH-002 — JWT Claim Structure

Tokens MUST carry the correct claims for downstream authorization to work.

#### Scenario: Agent token claims
- Given: A successful login by a tenant agent
- When: JWT is decoded
- Then: Claims include `sub` (userId), `role: "Agent"`, `tenantId: "<guid>"`

#### Scenario: Admin token claims
- Given: A successful login by an admin
- When: JWT is decoded
- Then: Claims include `sub` (userId), `role: "Admin"` — no `tenantId` claim

#### Scenario: SuperAdmin token claims
- Given: A successful login by a super admin
- When: JWT is decoded
- Then: Claims include `sub` (userId), `role: "SuperAdmin"` — no `tenantId` claim

---

### Requirement: SPEC-AUTH-003 — Role-Based Access Control

API endpoints MUST enforce role-based access.
Admin endpoints MUST require `Admin` or `SuperAdmin` role.
Tenant endpoints MUST require `Agent` or `Admin` role plus matching tenantId.
Public endpoints MUST require no authentication.

#### Scenario: Agent accesses admin endpoint
- Given: A valid Agent JWT token
- When: GET /api/v1/admin/packages
- Then: 403 Forbidden — Agent role is not authorized for admin endpoints

#### Scenario: Admin accesses tenant endpoint for any tenant
- Given: A valid Admin JWT token
- When: GET /api/v1/tenants/{anyTenantId}/packages
- Then: 200 OK — Admin role is authorized to access any tenant's data

#### Scenario: Agent accesses another tenant's endpoint
- Given: Agent JWT with tenantId = "tenant-a"
- When: GET /api/v1/tenants/tenant-b/packages
- Then: 403 Forbidden — tenantId in URL does not match JWT claim

#### Scenario: Unauthenticated request to protected endpoint
- Given: No Authorization header
- When: GET /api/v1/admin/packages
- Then: 401 Unauthorized

---

### Requirement: SPEC-AUTH-004 — Update User Profile

An authenticated user MUST be able to update their own profile (first name, last name).

#### Scenario: User updates their name
- Given: An authenticated user (any role)
- When: PUT /api/v1/auth/profile with firstName and lastName
- Then: 200 OK; user profile is updated

---

### Requirement: SPEC-ROLE-HIERARCHY — Role Hierarchy

The platform enforces a four-tier role hierarchy. Higher roles inherit access to lower-role
operations within their scope.

#### Scenario: SuperAdmin has full platform control
- Given: A SuperAdmin user
- When: Accessing any endpoint
- Then: Access is granted — SuperAdmin bypasses all role restrictions

#### Scenario: Admin can manage tenants and master packages
- Given: An Admin user
- When: POST /api/v1/admin/tenants, POST /api/v1/admin/packages
- Then: 200/201 responses — Admin role is authorized for all admin operations

#### Scenario: Agent is scoped to their tenant only
- Given: An Agent user with tenantId = "tenant-a"
- When: Accessing any endpoint under /api/v1/tenants/tenant-a/
- Then: 200 response — Agent is authorized for their own tenant

#### Scenario: Agent cannot create master packages
- Given: An Agent user
- When: POST /api/v1/admin/packages
- Then: 403 Forbidden — package creation is admin-only
