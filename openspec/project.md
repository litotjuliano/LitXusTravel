# LitXusTravel — Project Context

## Platform Overview

**LitXusTravel** is a multi-tenant SaaS platform for travel package distribution. Tour operators
(Admins) create master packages that travel agents (Tenants) sync and customize for their own
white-label websites, without affecting the master or other tenants.

**Core Innovation:** Package Synchronization Engine with NULL-coalescing override semantics.

---

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend API | .NET 8, ASP.NET Core, C# |
| Database | SQL Server, EF Core (Code First) |
| Architecture | Clean Architecture + CQRS (MediatR) |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Frontend (Admin) | Next.js 15, shadcn/ui, Framer Motion, Tailwind CSS |
| Frontend (Public) | Next.js 15, Tailwind CSS |
| Auth | JWT Bearer tokens |
| Photo service | Unsplash API |

---

## Architecture

```
Domain (no external dependencies — entities, value objects, domain events)
   ↓
Application (MediatR commands/queries, FluentValidation, AutoMapper, interfaces)
   ↓
Infrastructure (EF Core, external services, repository implementations)
   ↓
API (ASP.NET Core controllers, middleware, DI setup)
```

**CQRS pattern:** Every feature is a Command (mutation) or Query (read), handled via MediatR.
**Repository + Unit of Work:** All data access goes through `IUnitOfWork` and `IRepository<T>`.

---

## Multi-Tenancy

- **Strategy:** Shared database, automatic TenantId filtering via EF Core global query filters
- **Resolution:** Middleware extracts tenant from subdomain (e.g., `travelpro.lvh.me`)
- **Rule:** Every query MUST include TenantId — cross-tenant access returns 403 Forbidden
- **JWT claims:** `tenantId`, `role`, `sub` — validated on every request

---

## Package Synchronization (Core Business Logic)

```
Master Package (admin-created, Packages table)
   ↓ sync
TenantPackage (link record, TenantPackages table)
   +
PackageOverride (customizations, PackageOverrides table)
   ↓ merge (NULL-coalescing)
Resolved Package (what the public website shows)
```

**NULL semantics (critical):**
- `override.field IS NULL` → use master value (agent has not customized)
- `override.field IS NOT NULL` → use agent value (agent customized this field)
- Agent customizations survive master package updates

**Package types:**
- `IsOwnedPackage = true`: Portal-only package (all data in Override, no master)
- `IsOwnedPackage = false, CreatedByTenantId = null`: Admin system package (master-owned)
- `IsOwnedPackage = false, CreatedByTenantId = tenantId`: Tenant-extended package (shown as "Owned" in portal)

**Visibility states:** `Draft` (hidden from public) → `Published` → `Archived`

---

## Role Hierarchy

```
SuperAdmin → Platform Admin → Tenant Admin → StaffAgent / IndependentAgent
```

- **SuperAdmin:** Full platform control, dispute resolution
- **Admin:** Create tenants, manage master packages
- **Agent (Tenant):** Manage own packages, inquiries, commissions

---

## Naming Conventions

- Commands (mutations): `CreateTenantCommand`, `SyncPackagesCommand`
- Queries (reads): `GetTenantPackagesQuery`, `GetPublicPackagesQuery`
- Handlers: same name + `Handler` suffix
- Events: past tense — `PackagePublishedEvent`, `InquiryCreatedEvent`
- Controllers: versioned at `api/v1/` — `Admin/`, `Tenants/`, `Public/`

---

## Spec Code Reference

All implemented endpoints carry a SPEC code in their XML summary comment:
- `SPEC-ADMIN-*` — Admin/SuperAdmin operations
- `SPEC-TENANT-*` — Tenant agent operations
- `SPEC-PUBLIC-*` — Public website (unauthenticated)

Full behavioral specs live in `openspec/specs/` organized by domain.
Design/architecture docs live in `OpenSpecs/` (capital O — legacy reference docs).
