# LitXusTravel Deployment Guide

## Overview

LitXusTravel is a multi-tenant travel package distribution SaaS platform built with:
- **Backend**: .NET 8 API (Clean Architecture)
- **Frontend**: Next.js with TypeScript and shadcn/ui
- **Database**: PostgreSQL (native local install for development, production host TODO)
- **Deployment**: Docker + Kubernetes / Azure App Service

---

## Table of Contents

1. [Local Development Setup](#local-development-setup)
2. [Architecture Overview](#architecture-overview)
3. [Environment Configuration](#environment-configuration)
4. [Database Management](#database-management)
5. [Building & Running](#building--running)
6. [Staging Deployment](#staging-deployment)
7. [Production Deployment](#production-deployment)
8. [Monitoring & Troubleshooting](#monitoring--troubleshooting)
9. [CI/CD Pipeline](#cicd-pipeline)
10. [Rollback Procedures](#rollback-procedures)

---

## Local Development Setup

### Prerequisites

- **.NET 8 SDK** (https://dotnet.microsoft.com/download)
- **Node.js 18+** with npm (https://nodejs.org/)
- **PostgreSQL** (native Windows install, https://www.postgresql.org/download/windows/ — default port 5432)
- **Git** and GitHub CLI (gh)
- **Docker** (optional, for container testing)

### Initial Setup

```bash
# Clone the repository
git clone https://github.com/litxus-systems/litxus-travel.git
cd litxus-travel

# Restore .NET dependencies
dotnet restore

# Install Node dependencies
cd web/admin-portal
npm install
cd ../../web/public-website
npm install
cd ../..

# Initialize database (PostgreSQL)
dotnet ef database update --project src/LitXusTravel.Infrastructure
```

### Running Locally

**Terminal 1 - API Backend:**
```bash
cd src/LitXusTravel.API
dotnet run --configuration Debug
# API runs on http://localhost:5000
# Swagger docs: http://localhost:5000/swagger
```

**Terminal 2 - Admin Portal:**
```bash
cd web/admin-portal
npm run dev
# Portal runs on http://localhost:3000
```

**Terminal 3 - Public Website:**
```bash
cd web/public-website
npm run dev
# Website runs on http://localhost:3001
```

### Default Credentials (Development)

| Role | Email | Password |
|------|-------|----------|
| SuperAdmin | superadmin@litxustravel.com | SuperAdmin@123 |
| Admin | admin@litxustravel.com | Admin@123 |
| Tenant | contact@travelpro.com | TenantPassword@123 |

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Client Layer                         │
│  ┌─────────────────────┬──────────────────┐            │
│  │  Admin Portal       │  Public Website  │            │
│  │  (Next.js)          │  (Next.js)       │            │
│  └─────────────────────┴──────────────────┘            │
└────────────────────┬────────────────────────────────────┘
                     │ HTTPS
┌────────────────────▼────────────────────────────────────┐
│                    API Layer                            │
│  LitXusTravel.API (ASP.NET 8)                           │
│  ├─ Controllers (REST endpoints)                        │
│  ├─ Middleware (Auth, TenantContext, CORS)             │
│  └─ Dependency Injection                                │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                 Application Layer                       │
│  ├─ Handlers (CQRS pattern with MediatR)              │
│  ├─ Validators (FluentValidation)                      │
│  ├─ Mappers (AutoMapper)                               │
│  └─ Business Logic                                      │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                 Infrastructure Layer                    │
│  ├─ EF Core (Database Context)                         │
│  ├─ Repositories                                        │
│  ├─ External Services (Email, Storage, etc.)           │
│  └─ JWT Token Generation                                │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                    Data Layer                           │
│  PostgreSQL (native local / production host TODO)       │
│  ├─ Tenants                                             │
│  ├─ Packages (Master)                                   │
│  ├─ TenantPackages (Synced copies)                      │
│  ├─ PackageOverrides (Tenant customizations)           │
│  └─ Inquiries & Activities                              │
└─────────────────────────────────────────────────────────┘
```

---

## Environment Configuration

### Development (.env.local)

```env
# Database
ConnectionStrings__LitXusTravel=Host=localhost;Port=5432;Database=litxustravel_dev;Username=postgres;Password=postgres

# JWT
Jwt__Key=your-256-bit-secret-key-here
Jwt__Issuer=litxustravel.com
Jwt__Audience=litxustravel-api
Jwt__ExpirationMinutes=1440

# API
API_URL=http://localhost:5000
PUBLIC_API_URL=http://localhost:5000

# Frontend
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_APP_URL=http://localhost:3000
```

### Staging (.env.staging)

```env
# Database
# TODO: staging Postgres host undecided.
ConnectionStrings__LitXusTravel=Host=<TODO>;Port=5432;Database=litxustravel_staging;Username=<TODO>;Password=<TODO>

# JWT
Jwt__Key=staging-secret-key-min-256-characters-required
Jwt__Issuer=litxustravel.com
Jwt__Audience=litxustravel-api
Jwt__ExpirationMinutes=1440

# API
API_URL=https://api-staging.litxustravel.com
PUBLIC_API_URL=https://api-staging.litxustravel.com

# Frontend
NEXT_PUBLIC_API_URL=https://api-staging.litxustravel.com
NEXT_PUBLIC_APP_URL=https://staging.litxustravel.com
```

### Production (.env.production)

```env
# Database (PostgreSQL — production host TODO)
# TODO: production Postgres host undecided.
ConnectionStrings__LitXusTravel=Host=<TODO>;Port=5432;Database=litxustravel_prod;Username=<TODO>;Password=<TODO>;SSL Mode=Require

# JWT
Jwt__Key=production-secret-key-min-256-characters-required
Jwt__Issuer=litxustravel.com
Jwt__Audience=litxustravel-api
Jwt__ExpirationMinutes=1440

# API
API_URL=https://api.litxustravel.com
PUBLIC_API_URL=https://api.litxustravel.com

# Frontend
NEXT_PUBLIC_API_URL=https://api.litxustravel.com
NEXT_PUBLIC_APP_URL=https://litxustravel.com

# Logging
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_LOGLEVEL=Information
```

**Security Notes:**
- Store secrets in **Azure Key Vault**, not in files
- Use **Managed Identities** for Azure resource access
- Enable **TLS 1.3** for all HTTPS connections
- Rotate JWT keys quarterly

---

## Database Management

### Creating Migrations

```bash
# Add a new migration
dotnet ef migrations add DescribeYourChanges \
  --project src/LitXusTravel.Infrastructure \
  --startup-project src/LitXusTravel.API

# Review the generated migration file, then update:
dotnet ef database update --project src/LitXusTravel.Infrastructure
```

### Database Seeding

**Development only** - Seeds default data:
```csharp
// src/LitXusTravel.Infrastructure/Persistence/Seeding/DatabaseSeeder.cs
public class DatabaseSeeder
{
    public static async Task SeedAsync(LitXusTravelDbContext context)
    {
        // Seeds SuperAdmin, Admin, and sample Tenant
        // Runs automatically on startup if database is empty
    }
}
```

### Backup Strategy

**Development**: No backups required
**Staging**: Daily automated backups (7-day retention)
**Production**: 
- Hourly automated backups (30-day retention)
- Weekly full backups (90-day retention)
- Point-in-time recovery enabled

# TODO: production PostgreSQL host is undecided — backup strategy depends on the chosen
# provider (e.g. pg_dump + cron, or the managed backup tooling of whichever hosted
# Postgres service is selected). Document the concrete commands once the host is picked.

---

## Building & Running

### Backend Build

```bash
# Clean build
dotnet clean
dotnet build --configuration Release

# Run tests
dotnet test

# Publish for deployment
dotnet publish src/LitXusTravel.API \
  --configuration Release \
  --output ./publish/api
```

### Frontend Build

```bash
# Admin Portal
cd web/admin-portal
npm run lint
npm run build
# Output: .next directory ready for deployment

# Public Website
cd web/public-website
npm run lint
npm run build
```

### Docker Build

```bash
# Build Docker image for API
docker build -f src/LitXusTravel.API/Dockerfile -t litxustravel-api:latest .

# Build Docker image for Admin Portal
docker build -f web/admin-portal/Dockerfile -t litxustravel-admin:latest .

# Run locally
docker run -p 5000:80 litxustravel-api:latest
docker run -p 3000:3000 litxustravel-admin:latest
```

---

## Staging Deployment

### Prerequisites

- Azure subscription with App Service plan
- PostgreSQL database (hosting provider TODO)
- Azure Container Registry
- GitHub Actions secrets configured

### Deployment Steps

**1. Trigger Build:**
```bash
git push origin feature/your-feature
# Create Pull Request
# Merge to main branch
```

**2. GitHub Actions runs automatically:**
- Builds .NET API (Release configuration)
- Runs test suite
- Builds and pushes Docker images to ACR
- Deploys to App Service

**3. Verify Deployment:**
```bash
# Check API health
curl https://api-staging.litxustravel.com/health

# Check admin portal
https://staging.litxustravel.com/admin

# Verify database migrations
# Connect via psql and run: SELECT * FROM "__EFMigrationsHistory";
```

**4. Smoke Tests:**
```bash
# Test SuperAdmin login
curl -X POST https://api-staging.litxustravel.com/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@litxustravel.com","password":"Admin@123"}'

# Test package listing
curl https://api-staging.litxustravel.com/api/v1/admin/packages \
  -H "Authorization: Bearer <token>"
```

---

## Production Deployment

### Blue-Green Deployment Strategy

```
Production
├─ Blue Slot (Current)
│  └─ v1.2.0 (running)
└─ Green Slot (New)
   └─ v1.3.0 (deployed, not live)

After testing: Swap slots
└─ Blue Slot now runs v1.3.0
```

### Deployment Checklist

- [ ] All PR reviews completed
- [ ] All tests passing (unit, integration, e2e)
- [ ] Database migrations tested on staging
- [ ] Feature flags ready for rollout
- [ ] Monitoring dashboards prepared
- [ ] Rollback plan documented
- [ ] Stakeholders notified

### Production Deployment Steps

**1. Create Release:**
```bash
# Tag release
git tag -a v1.3.0 -m "Release version 1.3.0"
git push origin v1.3.0

# GitHub releases action triggers:
# - Creates release notes from commits
# - Builds and publishes Docker images
# - Deploys to Green slot
```

**2. Test Green Slot (2-4 hours):**
```bash
# Route traffic to green slot (1% of users)
# Monitor metrics in Application Insights
# Run synthetic tests against green
```

**3. Swap Slots (Blue ↔ Green):**
```powershell
# Using Azure CLI
az webapp deployment slot swap \
  --resource-group litxustravel-prod \
  --name litxustravel-api \
  --slot green
```

**4. Monitor Post-Deployment:**
- Error rates (target: < 0.1%)
- Response times (p99 < 1 second)
- Database query performance
- User-reported issues

---

## Monitoring & Troubleshooting

### Application Insights

**Metrics to Monitor:**
- Request rate (requests/sec)
- Response time (ms)
- Failed requests (%)
- Server response time
- Database query performance
- Exception rates

**Configure alerts:**
```json
{
  "alerts": [
    {
      "name": "High Error Rate",
      "condition": "Failed requests > 1%",
      "action": "Email ops team"
    },
    {
      "name": "Slow API Response",
      "condition": "p99 response time > 2000ms",
      "action": "PagerDuty alert"
    },
    {
      "name": "Database Connection Issues",
      "condition": "Connection pool exhaustion",
      "action": "Notify DBA"
    }
  ]
}
```

### Logging

**Log Levels:**
- **Error**: Database failures, unhandled exceptions
- **Warning**: Invalid input, retries, performance degradation
- **Info**: API calls, data modifications
- **Debug**: Query execution, parameter values (staging only)

**Query Logs:**
```bash
# View logs in Azure
az webapp log tail \
  --resource-group litxustravel-prod \
  --name litxustravel-api

# Or via Application Insights
# Navigate to: Logs → customEvents
```

### Common Issues

**Issue: High Database Query Times**
```sql
-- Check query performance
SELECT TOP 10 
  avg_elapsed_time,
  execution_count,
  total_elapsed_time,
  query_hash
FROM sys.dm_exec_query_stats
ORDER BY avg_elapsed_time DESC
```

**Issue: Memory Leaks**
- Check for unclosed database connections
- Monitor GC pressure in Application Insights
- Review: Diagnostics → Memory Dumps

**Issue: Authentication Failures**
```bash
# Verify JWT key matches across services
# Check token expiration in logs
# Verify CORS headers configured correctly
```

---

## CI/CD Pipeline

### GitHub Actions Workflow

**File:** `.github/workflows/deploy.yml`

```yaml
name: Deploy

on:
  push:
    branches: [main]
    tags: [v*]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0'
      
      - name: Build
        run: dotnet build --configuration Release
      
      - name: Test
        run: dotnet test --configuration Release
      
      - name: Publish
        run: dotnet publish src/LitXusTravel.API -c Release -o publish
      
      - name: Build Docker Image
        run: docker build -f src/LitXusTravel.API/Dockerfile -t ${{ secrets.REGISTRY }}/litxustravel-api:${{ github.sha }} .
      
      - name: Push to ACR
        run: docker push ${{ secrets.REGISTRY }}/litxustravel-api:${{ github.sha }}
      
      - name: Deploy to Staging
        if: github.ref == 'refs/heads/main'
        run: |
          az webapp deployment slot create \
            --resource-group litxustravel-staging \
            --name litxustravel-api \
            --slot green \
            --image ${{ secrets.REGISTRY }}/litxustravel-api:${{ github.sha }}
      
      - name: Deploy to Production
        if: startsWith(github.ref, 'refs/tags/v')
        run: |
          # Similar to staging but for prod
```

---

## Rollback Procedures

### Quick Rollback (5 minutes)

**Slot Swap (Blue-Green):**
```powershell
# Immediately swap back to previous version
az webapp deployment slot swap \
  --resource-group litxustravel-prod \
  --name litxustravel-api \
  --slot green

# Current running: v1.2.0
# Green slot: v1.3.0 (failed, will be investigated)
```

### Database Rollback

**Scenario: Migration caused issues**

```sql
-- Last resort - restore from backup (with data loss)
-- TODO: production PostgreSQL host undecided — restore steps depend on the chosen
-- provider's backup tooling (e.g. pg_restore from a pg_dump snapshot, or the managed
-- point-in-time-restore feature of whichever hosted Postgres service is selected).
```

**Safer: Keep old schema version available**

```csharp
// Add down migration that reverses breaking changes
public override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn("NewName", "OldName");
    // ... restore old schema
}
```

### Communication Plan

1. **Detect issue** (monitoring alerts)
2. **Declare incident** (PagerDuty, Slack #incident-response)
3. **Assess impact** (affected users, data integrity)
4. **Execute rollback** (5 min for slot swap, 30 min for DB)
5. **Root cause analysis** (24-48 hours)
6. **Post-mortem** (next team meeting)

---

## Disaster Recovery

### Recovery Time Objectives (RTO)
- API: 15 minutes
- Database: 30 minutes
- Website: 10 minutes

### Recovery Point Objectives (RPO)
- Database: 1 hour (hourly backups)
- Code: 5 minutes (GitHub as source of truth)

### Disaster Recovery Procedure

1. **Data Center Failure:**
   - API: Redeploy to secondary region (container registry has images)
   - DB: Restore from geo-replicated backup
   - DNS: Failover to secondary region IPs

2. **Data Corruption:**
   - Stop all writes immediately
   - Restore database from last known good backup
   - Replay clean transactions

3. **Account Compromise:**
   - Rotate all credentials (DB passwords, API keys)
   - Audit access logs (who accessed what, when)
   - Reset all user sessions

---

## Support & Escalation

**L1 Support**: GitHub Issues, Documentation
**L2 Support**: Engineering team on-call (PagerDuty)
**L3 Support**: DevOps/SRE for infrastructure issues

**Contact:**
- **Slack**: #litxustravel-ops
- **PagerDuty**: Escalation chain
- **Email**: ops@litxustravel.com

---

## Appendix: Useful Commands

```bash
# Check deployment status
az webapp show --resource-group litxustravel-prod --name litxustravel-api

# View recent deployments
az webapp deployment list-publishing-profiles \
  --resource-group litxustravel-prod \
  --name litxustravel-api

# Scale up for traffic surge
az appservice plan update \
  --resource-group litxustravel-prod \
  --name litxustravel-plan \
  --sku P2V2

# Database connection string
az sql db show-connection-string \
  --server-name litxustravel-prod \
  --name LitXusTravel_Prod

# Tail logs
az webapp log tail --resource-group litxustravel-prod --name litxustravel-api
```

---

**Last Updated:** 2026-05-30  
**Maintained By:** DevOps Team  
**Version:** 1.0
