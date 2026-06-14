# Public Website

## Overview

The public website is the white-label tenant storefront, accessible at `{subdomain}.lvh.me` in
development and `{subdomain}.litxustravel.com` in production. It requires no authentication.
All data is resolved from the tenant identified by subdomain — packages show merged
(master + override) values, and visitors can submit inquiries directly.

## Requirements

### Requirement: SPEC-PUBLIC-001 — Get Website Metadata

Anyone MUST be able to fetch basic metadata for a tenant's website using their subdomain.
This is used by the Next.js public website to set branding, title, and display information.

#### Scenario: Valid subdomain returns metadata
- Given: A tenant with subdomain "travelpro" exists
- When: GET /api/v1/public/websites/travelpro
- Then: 200 OK with `{ companyName, subdomain, contactEmail }` and any branding fields

#### Scenario: Unknown subdomain
- Given: No tenant with subdomain "unknown"
- When: GET /api/v1/public/websites/unknown
- Then: 404 Not Found

---

### Requirement: SPEC-PUBLIC-002 — List Packages for Public Website

Anyone MUST be able to list packages available on a tenant's public website.
The response MUST show only `Published` packages.
The response MUST use NULL-coalescing merge (master + tenant overrides).
Admin-created Published packages not yet synced to the tenant SHOULD also appear.

#### Scenario: Visitor browses packages on tenant website
- Given: A tenant with synced Published packages (some with overrides)
- When: GET /api/v1/public/websites/{subdomain}/packages
- Then: 200 OK with resolved packages; overridden fields show agent values; master fields shown where not overridden

#### Scenario: Draft packages are excluded
- Given: A tenant has a package that is still in Draft visibility
- When: GET /api/v1/public/websites/{subdomain}/packages
- Then: The Draft package does NOT appear in the response

#### Scenario: Admin-published packages appear without explicit sync
- Given: An admin Published package that is NOT yet synced to this tenant
- When: GET /api/v1/public/websites/{subdomain}/packages
- Then: The package SHOULD appear in the public listing (from the master catalog directly)

#### Scenario: Filter by category
- Given: Packages in "Beach" and "Adventure" categories
- When: GET /api/v1/public/websites/{subdomain}/packages?category=Beach
- Then: Only Beach packages are returned

#### Scenario: Home page — featured and popular packages
- Given: Tenant website home page loads
- When: GET /api/v1/public/websites/{subdomain}/packages?pageSize=6
- Then: 200 OK with up to 6 packages, preferring IsFeatured=true and IsPopular=true items

#### Scenario: Package from other tenant does not appear
- Given: A package extended to master by Tenant B
- When: Tenant A's public website loads
- Then: Tenant B's package MUST NOT appear on Tenant A's website

---

### Requirement: SPEC-PUBLIC-003 — Get Package Detail for Public Website

Anyone MUST be able to fetch full details of a specific package on a tenant's public website.
The response MUST include merged (master + override) values, full description, itinerary,
images, and contact details.

#### Scenario: Visitor views a package detail page
- Given: A published package synced to the tenant
- When: GET /api/v1/public/websites/{subdomain}/packages/{packageId}
- Then: 200 OK with full resolved package including description, itinerary, images, price, contact info

#### Scenario: Draft package not accessible on public website
- Given: A package in Draft state
- When: GET /api/v1/public/websites/{subdomain}/packages/{packageId}
- Then: 404 Not Found — Draft packages are not publicly accessible

#### Scenario: Package does not belong to tenant
- Given: A package belonging to a different tenant
- When: Visitor attempts to access it via this tenant's subdomain
- Then: 404 Not Found — never expose cross-tenant data

---

### Requirement: SPEC-PUBLIC-SERVERSIDE — Next.js Server-Side Data Fetching

The public website home page and packages page MUST fetch live data server-side using the subdomain
from the request host header. It MUST NOT show mock/hardcoded data when a subdomain is detected.

#### Scenario: Home page on tenant subdomain shows real packages
- Given: Next.js server receives request at `travelpro.lvh.me:3001`
- When: Home page renders
- Then: Page calls API with subdomain "travelpro"; displays real packages from the API; no mock data

#### Scenario: Home page on plain localhost falls back to mock data
- Given: Next.js server receives request at `localhost:3001` (no subdomain)
- When: Home page renders
- Then: Page MAY display mock/demo packages for development preview purposes
