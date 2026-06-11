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

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedSuperAdminUserAsync();
        await SeedAdminUserAsync();
        await SeedTenantsAsync();
        await SeedTenantAdminUsersAsync();
        await SeedPackagesAsync();
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

        var tenants = new[]
        {
            Tenant.Create("Travel Pro", new Email("contact@travelpro.com")),
            Tenant.Create("Wanderlust Tours", new Email("info@wanderlust.com")),
            Tenant.Create("Adventure Seekers", new Email("bookings@adventureseek.com")),
        };

        foreach (var tenant in tenants)
        {
            await dbContext.Tenants.AddAsync(tenant);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("✅ Seeded 3 sample tenants");
    }

    private async Task SeedTenantAdminUsersAsync()
    {
        var tenantAdmins = new[]
        {
            (email: "admin@travelpro.com",      password: "TravelPro@123",   firstName: "Travel",    lastName: "Pro",      tenantName: "Travel Pro"),
            (email: "admin@wanderlust.com",     password: "Wanderlust@123",  firstName: "Wanderlust", lastName: "Tours",   tenantName: "Wanderlust Tours"),
            (email: "admin@adventureseek.com",  password: "Adventure@123",   firstName: "Adventure", lastName: "Seekers", tenantName: "Adventure Seekers"),
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
        string category, string description, bool featured, bool popular)
    {
        var pkg = Package.Create(title, destination, price, days, category, description, "MYR");
        pkg.Publish();
        if (featured) pkg.SetFeatured(true);
        if (popular) pkg.SetPopular(true);
        return pkg;
    }
}
