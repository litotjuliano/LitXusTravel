using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
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
        services.AddDbContext<LitXusTravelDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(LitXusTravelDbContext).Assembly.FullName)));

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
        services.AddScoped<Seeding.DatabaseSeeder>();

        return services;
    }
}
