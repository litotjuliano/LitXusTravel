# LitXusTravel — Seed Users Cheatsheet

> Generated from `src/LitXusTravel.Infrastructure/Seeding/DatabaseSeeder.cs`

---

## 👤 Platform Users

| Role | Email | Password |
|------|-------|----------|
| SuperAdmin | superadmin@litxustravel.com | SuperAdmin@123 |
| Admin | admin@litxustravel.com | Admin@123 |

**SuperAdmin** — Full platform control: all tenants, all users, all packages, no TenantId  
**Admin** — Package & tenant management, no TenantId

---

## 🏨 Tenant Admin Users

Each tenant has one seed Admin user scoped to that tenant.

| Tenant | Email | Password |
|--------|-------|----------|
| Travel Pro | admin@travelpro.com | TravelPro@123 |
| Wanderlust Tours | admin@wanderlust.com | Wanderlust@123 |
| Adventure Seekers | admin@adventureseek.com | Adventure@123 |

> Role: **Admin** + TenantId set — can manage their own tenant's packages, inquiries, and agents.

---

## 🔑 Quick Login (Dev/UAT)

```
SuperAdmin        →  superadmin@litxustravel.com   /  SuperAdmin@123
Platform Admin    →  admin@litxustravel.com        /  Admin@123

Travel Pro        →  admin@travelpro.com           /  TravelPro@123
Wanderlust Tours  →  admin@wanderlust.com          /  Wanderlust@123
Adventure Seekers →  admin@adventureseek.com       /  Adventure@123
```

---

## 🏢 Roles

| Role | Scope | Description |
|------|-------|-------------|
| SuperAdmin | Platform | Full control — all tenants, all data |
| Admin | Platform or Tenant | Package & tenant management. If TenantId is set, scoped to that tenant |
| Agent | Tenant | Tenant-level staff — no seed user, created manually per tenant |

---

## 📦 Seed Packages

| # | Title | Destination | Price (MYR) | Days | Category | Featured | Popular |
|---|-------|-------------|-------------|------|----------|----------|---------|
| 1 | Japan Sakura | Japan | 1,500 | 10 | Asia | ✅ | ✅ |
| 2 | Europe Grand Tour | Europe | 2,200 | 14 | Europe | ✅ | ✅ |
| 3 | Maldives Luxury | Maldives | 2,500 | 7 | Beach | ✅ | ❌ |
| 4 | Bali Family | Bali | 1,000 | 7 | Asia | ❌ | ✅ |
| 5 | South Korea | South Korea | 1,200 | 8 | Asia | ❌ | ❌ |

---

## ⚠️ Notes
- All seed users run once — if the user already exists, seeding is skipped
- Tenant admin users are created after tenants are seeded (requires tenant to exist first)
- Tenants and packages only seed if their tables are empty
- Agent users are not seeded — create via `POST /api/v1/auth/register-agent`
- Seeder source: `LitXusTravel.Infrastructure/Seeding/DatabaseSeeder.cs`
