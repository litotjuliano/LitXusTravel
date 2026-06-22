# OpenSpec — AI Agent Instructions for LitXusTravel

## How to Use These Specs

1. **Before implementing any feature**, read the relevant spec file in `openspec/specs/<domain>/spec.md`
2. **Before proposing a new feature**, run `/opsx:propose "<feature name>"` to generate structured artifacts
3. **When implementing**, run `/opsx:apply` to follow the tasks checklist
4. **After completing**, run `/opsx:archive` to merge delta specs into main specs

---

## Spec Location Map

| Domain | Spec File | SPEC Codes / Coverage |
|--------|-----------|------------|
| Package management | `openspec/specs/packages/spec.md` | SPEC-ADMIN-001/002/003, SPEC-TENANT-001/002/004/005/006/007 |
| Tenant management | `openspec/specs/tenants/spec.md` | SPEC-ADMIN-004/005, settings, multi-tenancy, subdomain |
| Inquiries & CRM | `openspec/specs/inquiries/spec.md` | SPEC-ADMIN-010, SPEC-TENANT-008/010, SPEC-PUBLIC-004 |
| Public website | `openspec/specs/public-website/spec.md` | SPEC-PUBLIC-001/002/003, server-side data fetching |
| Authentication | `openspec/specs/auth/spec.md` | Login, JWT, RBAC, role hierarchy |
| Commission system | `openspec/specs/commission/spec.md` | Role hierarchy, commission rules, 10 safeguards, dispute resolution |
| Admin portal UI | `openspec/specs/admin-portal/spec.md` | Dashboard, packages, tenants pages; sidebar layout; components |
| Tenant portal UI | `openspec/specs/tenant-portal/spec.md` | Tenant dashboard layout, Tours/Bookings/Staff/Commission pages |
| Design system | `openspec/specs/design-system/spec.md` | Brand colors (#0066CC/#00A89A/#FF6B35), typography, spacing, Framer Motion |

---

## Critical Rules (Always Follow)

### Multi-Tenancy (Security-Critical)
- Every query MUST filter by TenantId — never return cross-tenant data
- Validate `tenantId` in URL path matches the `tenantId` claim in JWT
- Cross-tenant access MUST return 403 Forbidden, not 404

### Clean Architecture (Layer Rules)
- Domain layer: ZERO external package dependencies — only System + domain types
- Application layer depends on Domain only — never reference Infrastructure directly
- Infrastructure implements Application interfaces — never the reverse
- API layer: only HTTP routing, middleware, DI — no business logic

### Package Override NULL Semantics
- `override.field IS NULL` → use master value (not customized)
- `override.field IS NOT NULL` → use override value (agent customized this)
- Never auto-populate override fields — NULL is a valid and meaningful state

### CQRS Pattern
- All mutations → MediatR Command with `IRequest<Result<T>>`
- All reads → MediatR Query with `IRequest<Result<T>>`
- Validation → FluentValidation in separate `*Validator.cs` file
- No business logic in controllers — controllers only dispatch to MediatR

### Error Handling
- Use `Result<T>` pattern for business errors, not exceptions
- Domain invariant violations → `DomainException` (caught by middleware)
- Return `Result.Failure(message)` for validation/not-found/unauthorized errors

---

## Adding a New Endpoint

1. Check `openspec/specs/<domain>/spec.md` for the requirement
2. Create `UseCases/<Domain>/<ActionName>/` folder with:
   - `<ActionName>Command.cs` or `<ActionName>Query.cs`
   - `<ActionName>CommandHandler.cs` or `<ActionName>QueryHandler.cs`
   - `<ActionName>Validator.cs` (if command with input)
3. Register nothing — MediatR auto-discovers handlers by assembly scan
4. Add controller action with `/// <summary>Description (SPEC-CODE)</summary>`
5. Add to `openspec/specs/<domain>/spec.md` if not already there

---

## Full Architectural Reference

See `CLAUDE.md` in the project root for complete architectural decisions, patterns, and examples.
See `openspec/project.md` for tech stack and domain context.
