using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Infrastructure.BackgroundServices;
using LitXusTravel.Infrastructure.Data.Contexts;
using LitXusTravel.Infrastructure.Identity;
using LitXusTravel.Infrastructure.Repositories;
using LitXusTravel.Infrastructure.Services;

namespace LitXusTravel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Domain entities use DateTime (Kind=Unspecified) for several timestamp fields, which
        // SQL Server accepted but Npgsql rejects by default ("Cannot write DateTime with
        // Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is
        // supported"). Restore Npgsql's pre-6.0 behavior, which treats Unspecified-kind
        // DateTimes as UTC, rather than auditing every DateTime construction in the domain.
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<LitXusTravelDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql
                    .MigrationsAssembly(typeof(LitXusTravelDbContext).Assembly.FullName)
                    .EnableRetryOnFailure()));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
        .AddEntityFrameworkStores<LitXusTravelDbContext>();

        // Tenant context — scoped so it's per-request
        services.AddScoped<ICurrentTenant, CurrentTenant>();
        services.AddScoped<ITenantResolver, TenantResolver>();

        // Persistence
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application services
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();
        services.AddScoped<IWebsiteProvisioner, WebsiteProvisioner>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddHttpClient("Unsplash");
        services.AddScoped<IPhotoService, UnsplashPhotoService>();
        services.AddScoped<Seeding.DatabaseSeeder>();
        services.AddHostedService<SubscriptionExpiryCheckerService>();

        return services;
    }
}
