using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.ValueObjects;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Identity;

namespace LitXusTravel.Infrastructure.Seeding;

public class DatabaseSeeder(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    LitXusTravelDbContext dbContext,
    ILogger<DatabaseSeeder> logger)
{
    private static readonly string[] Roles = ["SuperAdmin", "Admin", "Agent"];

    private static readonly Dictionary<string, string> _defaultPackageImages = new()
    {
        ["Japan Sakura"]      = "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=800&q=80",
        ["Europe Grand Tour"] = "https://images.unsplash.com/photo-1499856871958-5b9627545d1a?w=800&q=80",
        ["Maldives Luxury"]   = "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=800&q=80",
        ["Bali Family"]       = "https://images.unsplash.com/photo-1537996194471-e657df975ab4?w=800&q=80",
        ["South Korea"]       = "https://images.unsplash.com/photo-1534274988757-a28bf1a57c17?w=800&q=80",
    };

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedSuperAdminUserAsync();
        await SeedAdminUserAsync();
        await SeedSubscriptionPlansAsync();
        await SeedTenantsAsync();
        await PatchTenantSubdomainsAsync();
        await SeedSubscriptionScenariosAsync();
        await SeedTenantAdminUsersAsync();
        await SeedDomainAdminUsersAsync();
        await SeedPackagesAsync();
        await PatchPackageImagesAsync();
        await SeedTenantPackagesAsync();
        await SeedTenantOwnedPackagesAsync();
        await BackfillPackageCreatorsAsync();
        await CleanupCrossTenantSyncAsync();
        await BackfillTenantDefaultCurrencyAsync();
        await SeedCommissionTestDataAsync();
        await SeedInvoicesAsync();
        await PurgeCrossTenantNotificationsAsync();
    }

    private async Task SeedSubscriptionPlansAsync()
    {
        if (await dbContext.SubscriptionPlans.AnyAsync()) return;

        var plans = new[]
        {
            SubscriptionPlan.Create("Starter",    99,  10,  2),
            SubscriptionPlan.Create("Pro",        299, 50,  10),
            SubscriptionPlan.Create("Enterprise", 999, 999, 50),
        };

        await dbContext.SubscriptionPlans.AddRangeAsync(plans);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("✅ Seeded {Count} subscription plans", plans.Length);
    }

    private async Task PatchPackageImagesAsync()
    {
        var packages = await dbContext.Packages
            .Where(p => p.FeaturedImageUrl == null)
            .ToListAsync();

        bool changed = false;
        foreach (var pkg in packages)
        {
            if (_defaultPackageImages.TryGetValue(pkg.Title, out var url))
            {
                pkg.SetFeaturedImage(url);
                changed = true;
            }
        }

        if (changed)
        {
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Patched default images for {Count} packages", packages.Count(p => p.FeaturedImageUrl != null));
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("✅ Created role: {Role}", role);
            }
        }
    }

    private async Task SeedSuperAdminUserAsync()
    {
        const string email = "superadmin@litxustravel.com";
        const string password = "SuperAdmin@123";

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "Super",
            LastName = "Admin",
            IsActive = true,
            EmailConfirmed = true,
            TenantId = null
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create SuperAdmin user");
            return;
        }

        await userManager.AddToRoleAsync(user, "SuperAdmin");
        logger.LogInformation("✅ Seeded SuperAdmin user");
        logger.LogInformation("   Email: {Email}", email);
        logger.LogInformation("   Password: {Password}", password);
        logger.LogInformation("   Permissions: Full platform control, all tenants, all users");
    }

    private async Task SeedAdminUserAsync()
    {
        const string email = "admin@litxustravel.com";
        const string password = "Admin@123";

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "System",
            LastName = "Admin",
            IsActive = true,
            EmailConfirmed = true,
            TenantId = null
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError("Failed to create Admin user");
            return;
        }

        await userManager.AddToRoleAsync(user, "Admin");
        logger.LogInformation("✅ Seeded Admin user");
        logger.LogInformation("   Email: {Email}", email);
        logger.LogInformation("   Password: {Password}", password);
        logger.LogInformation("   Permissions: Package & tenant management");
    }

    private async Task SeedTenantsAsync()
    {
        if (await dbContext.Tenants.AnyAsync()) return;

        var travelpro = Tenant.Create("Travel Pro", new Email("contact@travelpro.com"));
        travelpro.AssignSubdomain("travelpro");

        var wanderlust = Tenant.Create("Wanderlust Tours", new Email("info@wanderlust.com"));
        wanderlust.AssignSubdomain("wanderlust");

        var adventure = Tenant.Create("Adventure Seekers", new Email("bookings@adventureseek.com"));
        adventure.AssignSubdomain("adventure");

        var tenants = new[] { travelpro, wanderlust, adventure };

        foreach (var tenant in tenants)
        {
            await dbContext.Tenants.AddAsync(tenant);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("✅ Seeded 3 sample tenants");
    }

    private static readonly Dictionary<string, string> _tenantSubdomains = new()
    {
        ["Travel Pro"]        = "travelpro",
        ["Wanderlust Tours"]  = "wanderlust",
        ["Adventure Seekers"] = "adventure",
    };

    private async Task BackfillTenantDefaultCurrencyAsync()
    {
        var tenants = await dbContext.Tenants
            .Where(t => t.DefaultCurrency == "")
            .ToListAsync();

        if (tenants.Count == 0) return;

        foreach (var tenant in tenants)
            tenant.UpdateSettings("MYR");

        await dbContext.SaveChangesAsync();
        logger.LogInformation("✅ Backfilled DefaultCurrency=MYR for {Count} tenants", tenants.Count);
    }

    private async Task PatchTenantSubdomainsAsync()
    {
        var tenants = await dbContext.Tenants
            .Where(t => t.Subdomain == null)
            .ToListAsync();

        bool changed = false;
        foreach (var tenant in tenants)
        {
            if (_tenantSubdomains.TryGetValue(tenant.Name, out var subdomain))
            {
                tenant.AssignSubdomain(subdomain);
                changed = true;
            }
        }

        if (changed)
        {
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Patched subdomains for {Count} tenants", tenants.Count);
        }
    }

    private async Task SeedTenantAdminUsersAsync()
    {
        var tenantAdmins = new[]
        {
            (email: "admin@travelpro.com",      password: "TravelPro@123",   firstName: "Travel",    lastName: "Pro",      tenantName: "Travel Pro"),
            (email: "admin@wanderlust.com",     password: "Wanderlust@123",  firstName: "Wanderlust", lastName: "Tours",   tenantName: "Wanderlust Tours"),
            (email: "admin@adventureseek.com",  password: "Adventure@123",   firstName: "Adventure", lastName: "Seekers", tenantName: "Adventure Seekers"),
            (email: "admin@democorp.test",      password: "DemoCorp@123",    firstName: "Demo",      lastName: "Admin",   tenantName: "Demo Corp"),
            (email: "admin@gracecorp.test",     password: "GraceCorp@123",   firstName: "Grace",     lastName: "Admin",   tenantName: "Grace Corp"),
        };

        foreach (var (email, password, firstName, lastName, tenantName) in tenantAdmins)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null) continue;

            var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == tenantName);
            if (tenant is null)
            {
                logger.LogWarning("⚠️ Tenant '{TenantName}' not found — skipping admin user", tenantName);
                continue;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                EmailConfirmed = true,
                TenantId = tenant.Id
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create tenant admin for {TenantName}", tenantName);
                continue;
            }

            await userManager.AddToRoleAsync(user, "Admin");
            logger.LogInformation("✅ Seeded tenant admin for {TenantName}", tenantName);
            logger.LogInformation("   Email: {Email}", email);
            logger.LogInformation("   Password: {Password}", password);
        }
    }

    private async Task SeedDomainAdminUsersAsync()
    {
        var allIdentityUsers = userManager.Users.ToList();

        foreach (var identityUser in allIdentityUsers)
        {
            var email = identityUser.Email!;

            var alreadyExists = await dbContext.AdminUsers.AnyAsync(a => a.Email.Value == email);
            if (alreadyExists) continue;

            var roles = await userManager.GetRolesAsync(identityUser);
            var fullName = $"{identityUser.FirstName} {identityUser.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = email;

            var emailVo = new Email(email);
            AdminUser? domainUser = null;

            if (roles.Contains("SuperAdmin"))
            {
                domainUser = AdminUser.CreateSuperAdmin(fullName, emailVo);
            }
            else if (roles.Contains("Admin"))
            {
                if (identityUser.TenantId.HasValue)
                {
                    domainUser = AdminUser.CreateTenantAdmin(fullName, emailVo, identityUser.TenantId.Value);
                }
                else
                {
                    domainUser = AdminUser.CreatePlatformAdmin(fullName, emailVo);
                }
            }

            if (domainUser is not null)
            {
                await dbContext.AdminUsers.AddAsync(domainUser);
                logger.LogInformation("✅ Seeded domain AdminUser for {Email}", email);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedPackagesAsync()
    {
        if (await dbContext.Packages.AnyAsync()) return;

        var packages = new[]
        {
            CreateAndPublishPackage("Japan Sakura", "Japan", 1500, 10, "Asia", 
                "Experience cherry blossoms in Tokyo, Kyoto, and Osaka.", true, true),
            CreateAndPublishPackage("Europe Grand Tour", "Europe", 2200, 14, "Europe",
                "Visit 5 European countries in 14 days.", true, true),
            CreateAndPublishPackage("Maldives Luxury", "Maldives", 2500, 7, "Beach",
                "Relax in paradise with overwater bungalows.", true, false),
            CreateAndPublishPackage("Bali Family", "Bali", 1000, 7, "Asia",
                "Perfect family getaway with cultural tours and temples.", false, true),
            CreateAndPublishPackage("South Korea", "South Korea", 1200, 8, "Asia",
                "Explore Seoul, mountains, and coastal beauty.", false, false),
        };

        foreach (var pkg in packages)
        {
            await dbContext.Packages.AddAsync(pkg);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("✅ Seeded 5 sample packages");
    }

    private Package CreateAndPublishPackage(string title, string destination, decimal price, int days,
        string category, string description, bool featured, bool popular, string? imageUrl = null)
    {
        var pkg = Package.Create(title, destination, price, days, category, description, "MYR");
        pkg.Publish();
        if (featured) pkg.SetFeatured(true);
        if (popular) pkg.SetPopular(true);
        if (imageUrl != null) pkg.SetFeaturedImage(imageUrl);
        return pkg;
    }

    private async Task SeedTenantPackagesAsync()
    {
        var tenants = await dbContext.Tenants.ToListAsync();
        // System packages (no tenant owner) auto-sync to all tenants per platform rules
        var systemPackages = await dbContext.Packages
            .Where(p => p.CreatedByTenantId == null)
            .ToListAsync();

        if (tenants.Count == 0 || systemPackages.Count == 0) return;

        bool changed = false;
        foreach (var tenant in tenants)
        {
            foreach (var package in systemPackages)
            {
                var exists = await dbContext.TenantPackages
                    .AnyAsync(tp => tp.TenantId == tenant.Id && tp.MasterPackageId == package.Id);

                if (!exists)
                {
                    await dbContext.TenantPackages.AddAsync(TenantPackage.Create(tenant.Id, package.Id));
                    changed = true;
                }
            }

            logger.LogInformation("✅ Ensured {Count} system packages → {Tenant}", systemPackages.Count, tenant.Name);
        }

        if (changed) await dbContext.SaveChangesAsync();
    }

    private async Task SeedTenantOwnedPackagesAsync()
    {
        // Guard: skip if any tenant-owned (IsOwnedPackage) records already exist
        if (await dbContext.TenantPackages.AnyAsync(tp => tp.IsOwnedPackage)) return;

        var travelpro      = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Travel Pro");
        var wanderlust     = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Wanderlust Tours");
        var adventureSeek  = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Adventure Seekers");

        if (travelpro is null || wanderlust is null || adventureSeek is null)
        {
            logger.LogWarning("⚠️ Tenants not found — skipping tenant-owned packages seed");
            return;
        }

        // ────────────────────────────────────────────────────────────────
        // SCENARIO 1: "Extend to Master" = true
        //   → Creates a master Package + a TenantPackage linked to it.
        //   → Visible in Admin Dashboard → Packages with this tenant listed.
        // ────────────────────────────────────────────────────────────────

        // Travel Pro — extended packages
        await SeedExtendedPackageAsync(travelpro.Id,
            "Kyoto Tea Ceremony & Cultural Walk", "Japan", 980, 5, "Cultural",
            "Immerse in authentic Japanese culture — matcha ceremonies, geisha district, and temple trails.");

        await SeedExtendedPackageAsync(travelpro.Id,
            "Mount Fuji Sunrise Trek", "Japan", 1200, 4, "Adventure",
            "Guided overnight ascent of Mount Fuji, watch sunrise from the summit crater.");

        // Wanderlust Tours — extended packages
        await SeedExtendedPackageAsync(wanderlust.Id,
            "Santorini Sunset Cruise & Wine Tour", "Greece", 1850, 6, "Luxury",
            "Sail the Aegean Sea past volcanic cliffs, swim in hot springs, and taste local wines.");

        await SeedExtendedPackageAsync(wanderlust.Id,
            "Iceland Northern Lights Quest", "Iceland", 2400, 7, "Adventure",
            "Chase the Aurora Borealis across Reykjavik, the Golden Circle, and Jökulsárlón glacier lagoon.");

        // Adventure Seekers — extended package
        await SeedExtendedPackageAsync(adventureSeek.Id,
            "Nepal Everest Base Camp Trek", "Nepal", 2800, 14, "Adventure",
            "Follow in the footsteps of legends — teahouse trekking to 5,364 m with Sherpa guides.");

        // ────────────────────────────────────────────────────────────────
        // SCENARIO 2: "Extend to Master" = false
        //   → Creates TenantPackage (IsOwnedPackage = true) + PackageOverride.
        //   → Visible ONLY on that tenant's portal. Not in Admin master catalog.
        // ────────────────────────────────────────────────────────────────

        // Travel Pro — portal-only packages
        await SeedOwnedPackageAsync(travelpro.Id,
            "Penang Street Food Walking Tour", "Malaysia", 280, 2, "Cultural", "Asia",
            "Discover the UNESCO-listed George Town food scene — hawker stalls, roti canai, and laksa.",
            "+60 12-345 6789", "+60 12-345 6789");

        await SeedOwnedPackageAsync(travelpro.Id,
            "Cameron Highlands Tea Retreat", "Malaysia", 450, 3, "Nature", "Asia",
            "Escape to the cool highlands — tea plantation walks, strawberry farms, and mossy forest trails.",
            "+60 12-345 6789", null);

        // Wanderlust Tours — portal-only packages
        await SeedOwnedPackageAsync(wanderlust.Id,
            "Boracay Beach Holiday", "Philippines", 950, 5, "Beach", "Asia-Pacific",
            "White sand beaches, island hopping, helmet diving, and spectacular kite surfing.",
            "+63 9-123 4567", "+63 9-123 4567");

        await SeedOwnedPackageAsync(wanderlust.Id,
            "Phuket Island Hopping & Snorkeling", "Thailand", 780, 4, "Beach", "Asia-Pacific",
            "Phi Phi Islands, Maya Bay, snorkeling in crystal-clear waters, and sunset beach BBQ.",
            null, "+66 8-765 4321");

        // Adventure Seekers — portal-only packages
        await SeedOwnedPackageAsync(adventureSeek.Id,
            "Patagonia Glacier Hiking", "Argentina", 3200, 10, "Adventure", "Americas",
            "Trek across the Perito Moreno Glacier with certified mountain guides. Cold. Spectacular. Unforgettable.",
            "+54 11-1234 5678", null);

        await SeedOwnedPackageAsync(adventureSeek.Id,
            "Kenya Safari & Masai Mara", "Kenya", 3500, 8, "Adventure", "Africa",
            "Big Five game drives in the Masai Mara, sundowners on the savanna, and luxury tented camps.",
            "+254 700-123456", "+254 700-123456");

        await dbContext.SaveChangesAsync();
        logger.LogInformation("✅ Seeded tenant-owned packages (5 extended to master, 6 portal-only)");
    }


    private async Task BackfillPackageCreatorsAsync()
    {
        // Packages that already existed when CreatedByTenantId was added have null for that column.
        // Tenant-extended packages have exactly 1 non-owned TenantPackage (the creator).
        // Admin-synced packages have ≥2 (one per tenant the admin pushed to).
        var unfixed = await dbContext.Packages
            .Where(p => p.CreatedByTenantId == null)
            .Include(p => p.TenantPackages)
            .ToListAsync();

        var changed = 0;
        foreach (var pkg in unfixed)
        {
            var links = pkg.TenantPackages
                .Where(tp => !tp.IsOwnedPackage && tp.IsActive)
                .ToList();

            if (links.Count == 1)
            {
                pkg.MarkCreatedByTenant(links[0].TenantId);
                changed++;
            }
        }

        if (changed > 0)
        {
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Backfilled CreatedByTenantId for {Count} tenant-extended package(s)", changed);
        }
    }

    private async Task SeedExtendedPackageAsync(
        Guid tenantId, string title, string destination, decimal price, int days,
        string category, string description)
    {
        // Create master package stamped with the originating tenant
        var master = Package.Create(title, destination, price, days, category, description, "MYR");
        master.MarkCreatedByTenant(tenantId);
        master.Publish();
        await dbContext.Packages.AddAsync(master);
        await dbContext.SaveChangesAsync();

        // Link tenant to it
        var tp = TenantPackage.Create(tenantId, master.Id);
        await dbContext.TenantPackages.AddAsync(tp);
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedOwnedPackageAsync(
        Guid tenantId, string title, string destination, decimal price, int days,
        string category, string region, string description,
        string? phone, string? whatsapp)
    {
        var tp = TenantPackage.CreateOwned(tenantId);
        await dbContext.TenantPackages.AddAsync(tp);
        await dbContext.SaveChangesAsync();

        var ov = PackageOverride.CreateForOwned(
            tenantId, tp.Id,
            title, destination, days, price, "MYR",
            category, region, description,
            shortDescription: null,
            featuredImageUrl: null,
            contactPhone: phone,
            contactWhatsapp: whatsapp);

        await dbContext.PackageOverrides.AddAsync(ov);
        await dbContext.SaveChangesAsync();
    }

    private async Task CleanupCrossTenantSyncAsync()
    {
        var stale = await dbContext.TenantPackages
            .Include(tp => tp.MasterPackage)
            .Where(tp => !tp.IsOwnedPackage
                   && tp.MasterPackage != null
                   && tp.MasterPackage.CreatedByTenantId != null
                   && tp.MasterPackage.CreatedByTenantId != tp.TenantId)
            .ToListAsync();

        if (stale.Count > 0)
        {
            dbContext.TenantPackages.RemoveRange(stale);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("🧹 Removed {Count} stale cross-tenant package link(s)", stale.Count);
        }
    }

    private async Task SeedCommissionTestDataAsync()
    {
        if (await dbContext.Tours.AnyAsync()) return;

        var travelPro = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Travel Pro");
        if (travelPro is null)
        {
            logger.LogWarning("⚠️ Travel Pro tenant not found — skipping commission test data");
            return;
        }

        var tenantId = travelPro.Id;

        // Staff agents
        var john = StaffAgent.Create(tenantId, "John Smith", new Email("john.smith@travelpro.com"));
        var jane = StaffAgent.Create(tenantId, "Jane Doe", new Email("jane.doe@travelpro.com"));
        await dbContext.StaffAgents.AddRangeAsync(john, jane);

        // Commission rules
        var defaultRule = CommissionRule.CreateDefault(tenantId, CommissionTrigger.TourCompleted, 10, isPercentage: true);
        var johnRule = CommissionRule.CreateForAgent(tenantId, john.Id, CommissionTrigger.TourCompleted, 15, isPercentage: true);
        await dbContext.CommissionRules.AddRangeAsync(defaultRule, johnRule);

        // Tours
        var baliTour = Tour.Create(tenantId, "Bali Cultural Experience", "Bali, Indonesia",
            new DateTime(2026, 7, 15), new DateTime(2026, 7, 22), 20, 1000, "MYR");
        var tokyoTour = Tour.Create(tenantId, "Tokyo Cherry Blossom", "Tokyo, Japan",
            new DateTime(2026, 9, 1), new DateTime(2026, 9, 8), 15, 1500, "MYR");
        var langkawiTour = Tour.Create(tenantId, "Langkawi Beach Retreat", "Langkawi, Malaysia",
            new DateTime(2025, 12, 1), new DateTime(2025, 12, 7), 10, 800, "MYR");
        await dbContext.Tours.AddRangeAsync(baliTour, tokyoTour, langkawiTour);

        // Booking 1: Alice on Bali — Confirmed, commission accrued (15%, John's code)
        var aliceBooking = Booking.Create(tenantId, baliTour.Id, "Alice Johnson", "alice@customer.com",
            new DateTime(2026, 7, 15), 1000, john.UniqueCode, john.Id);
        baliTour.AddBooking();
        aliceBooking.Confirm();
        var aliceAccrual = CommissionAccrual.CreateFromBooking(
            john.Id, tenantId, johnRule.Id, aliceBooking.Id,
            CommissionTriggerType.TourCompleted, 150, 15, 1000);

        // Booking 2: Bob on Tokyo — Confirmed, commission accrued (10%, Jane's code)
        var bobBooking = Booking.Create(tenantId, tokyoTour.Id, "Bob Chen", "bob@customer.com",
            new DateTime(2026, 9, 1), 1500, jane.UniqueCode, jane.Id);
        tokyoTour.AddBooking();
        bobBooking.Confirm();
        var bobAccrual = CommissionAccrual.CreateFromBooking(
            jane.Id, tenantId, defaultRule.Id, bobBooking.Id,
            CommissionTriggerType.TourCompleted, 150, 10, 1500);

        // Booking 3: Carol on Langkawi — Completed, commission finalized (15%, John's code)
        var carolBooking = Booking.Create(tenantId, langkawiTour.Id, "Carol Williams", "carol@customer.com",
            new DateTime(2025, 12, 1), 800, john.UniqueCode, john.Id);
        langkawiTour.AddBooking();
        carolBooking.Confirm();
        carolBooking.Complete();
        langkawiTour.Complete();
        var carolAccrual = CommissionAccrual.CreateFromBooking(
            john.Id, tenantId, johnRule.Id, carolBooking.Id,
            CommissionTriggerType.TourCompleted, 120, 15, 800);
        carolAccrual.MarkAsFinalized();

        await dbContext.Bookings.AddRangeAsync(aliceBooking, bobBooking, carolBooking);
        await dbContext.CommissionAccruals.AddRangeAsync(aliceAccrual, bobAccrual, carolAccrual);

        await dbContext.SaveChangesAsync();
        logger.LogInformation("✅ Seeded commission test data");
        logger.LogInformation("   Staff: John Smith ({JohnCode}), Jane Doe ({JaneCode})", john.UniqueCode, jane.UniqueCode);
        logger.LogInformation("   Tours: Bali (Scheduled), Tokyo (Scheduled), Langkawi (Completed)");
        logger.LogInformation("   Bookings: Alice/Bali (Accrued MYR150), Bob/Tokyo (Accrued MYR150), Carol/Langkawi (Finalized MYR120)");
    }

    private async Task SeedSubscriptionScenariosAsync()
    {
        var plans = await dbContext.SubscriptionPlans.ToListAsync();
        if (plans.Count == 0) return;

        var starter = plans.FirstOrDefault(p => p.Name == "Starter");
        var pro     = plans.FirstOrDefault(p => p.Name == "Pro");

        if (starter is null || pro is null)
        {
            logger.LogWarning("⚠️ Standard plans not found — skipping subscription scenarios");
            return;
        }

        bool changed = false;

        // ── Core 3 tenants: always reset to the intended demo scenario ────────
        // Deleting + recreating ensures all 5 health states are visible on every
        // fresh API start, regardless of any mock-payment renewals in prior sessions.
        //
        //  Travel Pro     → Active       (Pro,     90 days remaining)
        //  Wanderlust     → ExpiringSoon (Starter,  8 days remaining — urgent warning)
        //  Adventure      → Trial        (free 30-day trial)

        var coreScenarios = new (string tenantName, Func<Guid, TenantSubscription> factory, string health)[]
        {
            ("Travel Pro",
                id => TenantSubscription.CreateWithEndDate(
                    id, pro.Name, pro.Price, pro.MaxPackages, pro.MaxTeamMembers,
                    DateTime.UtcNow.AddDays(90)),
                "Active"),

            ("Wanderlust Tours",
                id => TenantSubscription.CreateWithEndDate(
                    id, starter.Name, starter.Price, starter.MaxPackages, starter.MaxTeamMembers,
                    DateTime.UtcNow.AddDays(8)),
                "ExpiringSoon"),

            ("Adventure Seekers",
                id => TenantSubscription.CreateTrial(id),
                "Trial"),
        };

        foreach (var (tenantName, factory, health) in coreScenarios)
        {
            var tenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == tenantName);
            if (tenant is null) continue;

            var existingSubs = await dbContext.TenantSubscriptions
                .Where(x => x.TenantId == tenant.Id)
                .ToListAsync();

            if (existingSubs.Count > 0)
            {
                dbContext.TenantSubscriptions.RemoveRange(existingSubs);
                await dbContext.SaveChangesAsync();
            }

            await dbContext.TenantSubscriptions.AddAsync(factory(tenant.Id));
            await dbContext.SaveChangesAsync();
            changed = true;
            logger.LogInformation("✅ Set {Tenant} subscription → {Health}", tenantName, health);
        }

        // ── Demo Corp: Expired (past 7-day grace window, read-only mode) ─────
        const string expiredTenantName = "Demo Corp";
        var expiredTenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == expiredTenantName);
        if (expiredTenant is null)
        {
            expiredTenant = Tenant.Create(expiredTenantName, new Email("admin@democorp.test"));
            expiredTenant.AssignSubdomain("democorp");
            await dbContext.Tenants.AddAsync(expiredTenant);
            dbContext.Entry(expiredTenant).Property("CreatedAt").CurrentValue =
                DateTimeOffset.UtcNow.AddDays(-45);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Created Demo Corp tenant (backdated 45d)");
        }

        var existingExpiredSubs = await dbContext.TenantSubscriptions
            .Where(x => x.TenantId == expiredTenant.Id)
            .ToListAsync();
        var demoHasActive = existingExpiredSubs.Any(s => s.Status == SubscriptionStatus.Active);
        if (!demoHasActive)
        {
            if (existingExpiredSubs.Count > 0)
            {
                dbContext.TenantSubscriptions.RemoveRange(existingExpiredSubs);
                await dbContext.SaveChangesAsync();
            }
            await dbContext.TenantSubscriptions.AddAsync(TenantSubscription.CreateExpired(
                expiredTenant.Id, starter.Name, starter.Price,
                starter.MaxPackages, starter.MaxTeamMembers,
                DateTime.UtcNow.AddDays(-10)));
            await dbContext.SaveChangesAsync();
            changed = true;
            logger.LogInformation("✅ Set Demo Corp subscription → Expired (10d past grace)");
        }
        else
        {
            logger.LogInformation("ℹ️ Demo Corp subscription preserved (Active — tenant has renewed)");
        }

        // ── Grace Corp: Expired (past 7-day grace window, read-only mode) ────
        const string graceTenantName = "Grace Corp";
        var graceTenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == graceTenantName);
        if (graceTenant is null)
        {
            graceTenant = Tenant.Create(graceTenantName, new Email("admin@gracecorp.test"));
            graceTenant.AssignSubdomain("gracecorp");
            await dbContext.Tenants.AddAsync(graceTenant);
            dbContext.Entry(graceTenant).Property("CreatedAt").CurrentValue =
                DateTimeOffset.UtcNow.AddDays(-35);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Created Grace Corp tenant");
        }

        var existingGraceSubs = await dbContext.TenantSubscriptions
            .Where(x => x.TenantId == graceTenant.Id)
            .ToListAsync();
        var graceHasActive = existingGraceSubs.Any(s => s.Status == SubscriptionStatus.Active);
        if (!graceHasActive)
        {
            if (existingGraceSubs.Count > 0)
            {
                dbContext.TenantSubscriptions.RemoveRange(existingGraceSubs);
                await dbContext.SaveChangesAsync();
            }
            await dbContext.TenantSubscriptions.AddAsync(TenantSubscription.CreateExpired(
                graceTenant.Id, starter.Name, starter.Price,
                starter.MaxPackages, starter.MaxTeamMembers,
                DateTime.UtcNow.AddDays(-10)));
            await dbContext.SaveChangesAsync();
            changed = true;
            logger.LogInformation("✅ Set Grace Corp subscription → Expired (10d past grace)");
        }
        else
        {
            logger.LogInformation("ℹ️ Grace Corp subscription preserved (Active — tenant has renewed)");
        }

        if (changed) await dbContext.SaveChangesAsync();
    }

    private async Task SeedInvoicesAsync()
    {
        var travelPro     = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Travel Pro");
        var wanderlust    = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Wanderlust Tours");
        var adventureSeek = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Adventure Seekers");

        if (travelPro is not null && !await dbContext.Invoices.AnyAsync(i => i.TenantId == travelPro.Id))
        {
            await dbContext.Invoices.AddRangeAsync(
                Invoice.Create(travelPro.Id, "Travel Pro Agency", "INV-2026-035", "Pro", 299, "Jun 2026", "Paid",
                    new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)),
                Invoice.Create(travelPro.Id, "Travel Pro Agency", "INV-2026-028", "Pro", 299, "May 2026", "Paid",
                    new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)),
                Invoice.Create(travelPro.Id, "Travel Pro Agency", "INV-2026-021", "Pro", 299, "Apr 2026", "Paid",
                    new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)));
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Seeded invoices for Travel Pro Agency");
        }

        if (wanderlust is not null && !await dbContext.Invoices.AnyAsync(i => i.TenantId == wanderlust.Id))
        {
            await dbContext.Invoices.AddRangeAsync(
                Invoice.Create(wanderlust.Id, "Wanderlust Tours", "INV-2026-036", "Starter", 99, "Jun 2026", "Paid",
                    new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)),
                Invoice.Create(wanderlust.Id, "Wanderlust Tours", "INV-2026-029", "Starter", 99, "May 2026", "Paid",
                    new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)),
                Invoice.Create(wanderlust.Id, "Wanderlust Tours", "INV-2026-022", "Starter", 99, "Apr 2026", "Failed",
                    new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)));
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Seeded invoices for Wanderlust Tours");
        }

        if (adventureSeek is not null && !await dbContext.Invoices.AnyAsync(i => i.TenantId == adventureSeek.Id))
        {
            await dbContext.Invoices.AddRangeAsync(
                Invoice.Create(adventureSeek.Id, "Adventure Seekers", "INV-2026-037", "Enterprise", 999, "Jun 2026", "Pending",
                    new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)),
                Invoice.Create(adventureSeek.Id, "Adventure Seekers", "INV-2026-030", "Enterprise", 999, "May 2026", "Paid",
                    new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)),
                Invoice.Create(adventureSeek.Id, "Adventure Seekers", "INV-2026-023", "Enterprise", 999, "Apr 2026", "Paid",
                    new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc)));
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Seeded invoices for Adventure Seekers");
        }

        var demoCorp = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Demo Corp");
        if (demoCorp is not null)
        {
            if (!await dbContext.Invoices.AnyAsync(i => i.InvoiceNumber == "INV-2026-015"))
            {
                await dbContext.Invoices.AddAsync(Invoice.Create(
                    demoCorp.Id, "Demo Corp", "INV-2026-015", "Starter", 99, "May 2026", "Paid",
                    new DateTime(2026, 5, 16, 0, 0, 0, DateTimeKind.Utc)));
                await dbContext.SaveChangesAsync();
            }
            if (!await dbContext.Invoices.AnyAsync(i => i.InvoiceNumber == "INV-2026-038"))
            {
                await dbContext.Invoices.AddAsync(Invoice.Create(
                    demoCorp.Id, "Demo Corp", "INV-2026-038", "Starter", 99, "Jun 2026", "Failed",
                    new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc)));
                await dbContext.SaveChangesAsync();
                logger.LogInformation("✅ Seeded Failed renewal invoice for Demo Corp");
            }
        }

        var graceCorp = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Grace Corp");
        if (graceCorp is not null)
        {
            if (!await dbContext.Invoices.AnyAsync(i => i.InvoiceNumber == "INV-2026-016"))
            {
                await dbContext.Invoices.AddAsync(Invoice.Create(
                    graceCorp.Id, "Grace Corp", "INV-2026-016", "Starter", 99, "May 2026", "Paid",
                    new DateTime(2026, 5, 16, 0, 0, 0, DateTimeKind.Utc)));
                await dbContext.SaveChangesAsync();
            }
            if (!await dbContext.Invoices.AnyAsync(i => i.InvoiceNumber == "INV-2026-039"))
            {
                await dbContext.Invoices.AddAsync(Invoice.Create(
                    graceCorp.Id, "Grace Corp", "INV-2026-039", "Starter", 99, "Jun 2026", "Failed",
                    new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc)));
                await dbContext.SaveChangesAsync();
                logger.LogInformation("✅ Seeded Failed renewal invoice for Grace Corp");
            }
        }
    }

    private async Task PurgeCrossTenantNotificationsAsync()
    {
        // Remove subscription lifecycle notifications that were incorrectly delivered to
        // tenant-scoped admins about tenants other than their own (created before the
        // tenant-scoping filter was added to NotificationService).
        var subscriptionTypes = new[]
        {
            "subscription_expiring_soon",
            "subscription_grace_period",
            "subscription_fully_expired",
            "subscription_renewed",
        };

        var tenantAdmins = await dbContext.Users
            .Where(u => u.TenantId != null)
            .Select(u => new { u.Id, u.TenantId })
            .ToListAsync();

        if (tenantAdmins.Count == 0) return;

        var deleted = 0;
        foreach (var admin in tenantAdmins)
        {
            var wrong = await dbContext.Notifications
                .Where(n => n.UserId == admin.Id
                         && subscriptionTypes.Contains(n.Type)
                         && n.RelatedEntityType == "Tenant"
                         && n.RelatedEntityId != admin.TenantId)
                .ToListAsync();

            if (wrong.Count == 0) continue;

            dbContext.Notifications.RemoveRange(wrong);
            deleted += wrong.Count;
        }

        if (deleted > 0)
        {
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Purged {Count} cross-tenant notification(s) from tenant-scoped admins", deleted);
        }
    }

    private async Task PatchDemoCorpCreatedAtAsync(Tenant tenant)
    {
        var sub = await dbContext.TenantSubscriptions
            .Where(s => s.TenantId == tenant.Id && s.Status == SubscriptionStatus.Expired)
            .FirstOrDefaultAsync();

        if (sub is null) return;

        // CreatedAt must precede the subscription's StartDate. If it doesn't, backdate it.
        // SpecifyKind ensures the DateTime is treated as UTC regardless of what Npgsql returned.
        var startUtc = DateTime.SpecifyKind(sub.StartDate, DateTimeKind.Utc);
        var expectedCreatedAt = new DateTimeOffset(startUtc.AddDays(-5), TimeSpan.Zero);
        if (tenant.CreatedAt > expectedCreatedAt)
        {
            dbContext.Entry(tenant).Property("CreatedAt").CurrentValue = expectedCreatedAt;
            await dbContext.SaveChangesAsync();
            logger.LogInformation("✅ Patched Demo Corp CreatedAt → {Date}",
                expectedCreatedAt.ToString("yyyy-MM-dd"));
        }
    }
}
