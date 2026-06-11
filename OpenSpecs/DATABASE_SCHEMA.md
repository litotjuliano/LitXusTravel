# LitXusTravel Database Schema
## Multi-Tenant SQL Server Design

**Database:** `LitXusTravel_Production`  
**DBMS:** SQL Server 2019+  
**Architecture:** Shared database with tenant isolation via TenantId

---

## Core Tables

### 1. Tenants (Tenant Master Data)

```sql
CREATE TABLE [dbo].[Tenants] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Name] NVARCHAR(255) NOT NULL,
    [Slug] NVARCHAR(100) NOT NULL UNIQUE,                 -- For URL/subdomain
    [Subdomain] NVARCHAR(100) UNIQUE,                    -- tenant.nexustravel.com
    [ContactEmail] NVARCHAR(255) NOT NULL,
    [ContactPhone] NVARCHAR(20),
    [LogoUrl] NVARCHAR(500),
    [BrandColors] NVARCHAR(MAX),                         -- JSON: {primary, secondary}
    [IsActive] BIT NOT NULL DEFAULT 1,
    [ProvisioningStatus] NVARCHAR(50) DEFAULT 'pending', -- pending, completed, failed
    [WebsiteUrl] NVARCHAR(500),
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIMEOFFSET,
    INDEX [IX_Tenants_Slug] ([Slug]),
    INDEX [IX_Tenants_Subdomain] ([Subdomain]),
    INDEX [IX_Tenants_IsActive] ([IsActive])
);
```

### 2. TenantSubscriptions (SaaS Subscriptions)

```sql
CREATE TABLE [dbo].[TenantSubscriptions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Tenants]([Id]),
    [PlanName] NVARCHAR(100) NOT NULL,                   -- Starter, Pro, Enterprise
    [MonthlyPrice] DECIMAL(10, 2) NOT NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2,                                 -- NULL = active
    [MaxPackages] INT DEFAULT 100,
    [MaxTeamMembers] INT DEFAULT 5,
    [Features] NVARCHAR(MAX),                            -- JSON array of feature flags
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'active',     -- active, expired, suspended
    [AutoRenew] BIT DEFAULT 1,
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_TenantSubscriptions_TenantId] ([TenantId]),
    INDEX [IX_TenantSubscriptions_Status] ([Status]),
    INDEX [IX_TenantSubscriptions_EndDate] ([EndDate])
);
```

### 3. Packages (Master Packages)

```sql
CREATE TABLE [dbo].[Packages] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [Title] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX),
    [ShortDescription] NVARCHAR(500),
    [Category] NVARCHAR(100),                            -- Beach, Mountain, City, etc.
    [BasePrice] DECIMAL(10, 2) NOT NULL,
    [Currency] NVARCHAR(10) DEFAULT 'RM',
    [Duration] INT NOT NULL,                             -- Days
    [Destination] NVARCHAR(255) NOT NULL,
    [Region] NVARCHAR(100),                              -- Country/state
    [Highlights] NVARCHAR(MAX),                          -- JSON array
    [Itinerary] NVARCHAR(MAX),                           -- JSON structured itinerary
    [Inclusions] NVARCHAR(MAX),                          -- JSON array
    [Exclusions] NVARCHAR(MAX),                          -- JSON array
    [Images] NVARCHAR(MAX),                              -- JSON array of URLs
    [FeaturedImage] NVARCHAR(500),
    [VideoUrl] NVARCHAR(500),
    [Rating] DECIMAL(3, 2),                              -- 1-5 stars
    [ReviewCount] INT DEFAULT 0,
    [Visibility] NVARCHAR(50) DEFAULT 'draft',           -- draft, published, archived
    [IsPopular] BIT DEFAULT 0,
    [IsFeatured] BIT DEFAULT 0,
    [MaxGroupSize] INT,
    [MinGroupSize] INT DEFAULT 2,
    [CreatedById] UNIQUEIDENTIFIER,                      -- Admin user
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_Packages_Category] ([Category]),
    INDEX [IX_Packages_Destination] ([Destination]),
    INDEX [IX_Packages_Visibility] ([Visibility]),
    INDEX [IX_Packages_IsFeatured] ([IsFeatured])
);
```

### 4. TenantPackages (Tenant Synchronized Packages)

```sql
CREATE TABLE [dbo].[TenantPackages] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Tenants]([Id]),
    [MasterPackageId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Packages]([Id]),
    [TenantPackageId] UNIQUEIDENTIFIER,                  -- Can be same as Master if no override
    [SyncStatus] NVARCHAR(50) NOT NULL DEFAULT 'synced', -- synced, draft, conflict
    [LastSyncedAt] DATETIMEOFFSET,
    [IsCustomized] BIT DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    UNIQUE ([TenantId], [MasterPackageId]),
    INDEX [IX_TenantPackages_TenantId] ([TenantId]),
    INDEX [IX_TenantPackages_MasterPackageId] ([MasterPackageId]),
    INDEX [IX_TenantPackages_SyncStatus] ([SyncStatus]),
    INDEX [IX_TenantPackages_IsActive] ([IsActive])
);
```

### 5. PackageOverrides (Tenant-Specific Customizations)

```sql
CREATE TABLE [dbo].[PackageOverrides] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Tenants]([Id]),
    [TenantPackageId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [TenantPackages]([Id]),
    [FieldName] NVARCHAR(100) NOT NULL,                  -- title, price, images, etc.
    [OriginalValue] NVARCHAR(MAX),                       -- JSON serialized
    [OverriddenValue] NVARCHAR(MAX),                     -- JSON serialized
    [OverrideType] NVARCHAR(50),                         -- markup, full_replace, partial
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_PackageOverrides_TenantId] ([TenantId]),
    INDEX [IX_PackageOverrides_TenantPackageId] ([TenantPackageId]),
    INDEX [IX_PackageOverrides_FieldName] ([FieldName])
);
```

### 6. Inquiries (Customer Inquiries)

```sql
CREATE TABLE [dbo].[Inquiries] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Tenants]([Id]),
    [PackageId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [TenantPackages]([Id]),
    [CustomerName] NVARCHAR(255) NOT NULL,
    [CustomerEmail] NVARCHAR(255) NOT NULL,
    [CustomerPhone] NVARCHAR(20) NOT NULL,
    [Message] NVARCHAR(MAX),
    [NumberOfTravelers] INT,
    [PreferredDates] NVARCHAR(MAX),                      -- JSON or free text
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'new',        -- new, contacted, quotation_sent, confirmed, rejected
    [AssignedToUserId] UNIQUEIDENTIFIER,                 -- Tenant user
    [WhatsAppGroupUrl] NVARCHAR(500),                    -- Optional WhatsApp group link
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_Inquiries_TenantId] ([TenantId]),
    INDEX [IX_Inquiries_PackageId] ([PackageId]),
    INDEX [IX_Inquiries_Status] ([Status]),
    INDEX [IX_Inquiries_CreatedAt] ([CreatedAt])
);
```

### 7. Quotations (Generated Quotations)

```sql
CREATE TABLE [dbo].[Quotations] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [InquiryId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Inquiries]([Id]),
    [TenantId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Tenants]([Id]),
    [PackageTitle] NVARCHAR(255) NOT NULL,
    [BasePrice] DECIMAL(10, 2) NOT NULL,
    [Markup] DECIMAL(10, 2) DEFAULT 0,                   -- Tenant markup
    [FinalPrice] DECIMAL(10, 2) NOT NULL,
    [Currency] NVARCHAR(10) DEFAULT 'RM',
    [NumberOfTravelers] INT NOT NULL,
    [TotalPrice] DECIMAL(10, 2) NOT NULL,                -- Final price * travelers
    [CustomNotes] NVARCHAR(MAX),
    [ValidUntil] DATETIME2,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'draft',      -- draft, sent, accepted, rejected, expired
    [SentAt] DATETIMEOFFSET,
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_Quotations_InquiryId] ([InquiryId]),
    INDEX [IX_Quotations_TenantId] ([TenantId]),
    INDEX [IX_Quotations_Status] ([Status])
);
```

### 8. Notifications (System Notifications)

```sql
CREATE TABLE [dbo].[Notifications] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [Tenants]([Id]),  -- NULL = platform-wide
    [UserId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [AspNetUsers]([Id]),
    [Type] NVARCHAR(100) NOT NULL,                       -- subscription_expiry, inquiry_received, etc.
    [Title] NVARCHAR(255) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [RelatedEntityId] UNIQUEIDENTIFIER,                  -- Inquiry ID, Subscription ID, etc.
    [RelatedEntityType] NVARCHAR(100),
    [IsRead] BIT DEFAULT 0,
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [ReadAt] DATETIMEOFFSET,
    INDEX [IX_Notifications_UserId] ([UserId]),
    INDEX [IX_Notifications_TenantId] ([TenantId]),
    INDEX [IX_Notifications_IsRead] ([IsRead])
);
```

---

## Multi-Tenancy Implementation

### Tenant Resolution Strategy

**Option 1: Subdomain-Based (Phase 1)**
- URL: `agentname.nexustravel.com`
- Extract tenant from host header in middleware
- Store in `ICurrentTenant` scoped service

**Option 2: Custom Domain (Phase 2)**
- URL: `www.agenttravel.com`
- Hostname → Tenant mapping query

### Data Isolation Approach

```csharp
// In repository queries - ALWAYS filter by TenantId
var packages = dbContext.TenantPackages
    .Where(p => p.TenantId == currentTenant.Id)  // Mandatory filter
    .ToListAsync();
```

### DbContext Configuration

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Query filter to automatically include TenantId check
    modelBuilder.Entity<TenantPackage>()
        .HasQueryFilter(p => p.TenantId == _currentTenant.Id);
    
    modelBuilder.Entity<Inquiry>()
        .HasQueryFilter(i => i.TenantId == _currentTenant.Id);
    
    modelBuilder.Entity<Quotation>()
        .HasQueryFilter(q => q.TenantId == _currentTenant.Id);
    
    // Similar for all tenant-dependent entities
}
```

---

## Key Relationships

```
Tenants (1) ────→ (many) TenantSubscriptions
          ────→ (many) TenantPackages
          ────→ (many) Inquiries
          ────→ (many) Quotations

Packages (1) ────→ (many) TenantPackages
                        ↓
                 (many) PackageOverrides

TenantPackages (1) ────→ (many) PackageOverrides
                   ────→ (many) Inquiries
                   
Inquiries (1) ────→ (many) Quotations
```

---

## Indexing Strategy

- **TenantId**: Every query must filter by tenant (clustered or non-clustered)
- **Status fields**: Orders, subscriptions, notifications often filtered by status
- **Dates**: CreatedAt, UpdatedAt for range queries and sorting
- **Foreign keys**: Automatic for relationships

---

## Backup & Migration Strategy

```sql
-- Full backup before migrations
BACKUP DATABASE [LitXusTravel_Production] 
TO DISK = 'Z:\Backups\LitXusTravel_Production_2026-05-28.bak'
WITH COMPRESSION;

-- EF Core migration (handles schema changes)
dotnet ef database update --project LitXusTravel.Infrastructure
```

---

## Seed Data Strategy

```csharp
// DefaultDataSeeder.cs
public class DefaultDataSeeder
{
    public async Task SeedAsync(LitXusTravelDbContext context)
    {
        // Default package categories
        // Default themes
        // Admin user
        // Sample packages for demo
        
        await context.SaveChangesAsync();
    }
}
```

---

## Performance Considerations

- **Vertical Partitioning**: Separate "cold" data (old inquiries) to archive table
- **Pagination**: Always use Skip/Take for list queries
- **Lazy Loading**: Disabled; use `.Include()` for related data
- **Query Timeout**: 30 seconds default
- **Connection Pooling**: 100 connections per pool

---

**Database Schema Ready for EF Core Implementation** ✅
