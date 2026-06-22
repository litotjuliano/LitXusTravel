using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LitXusTravel.Application.Interfaces.Services;

namespace LitXusTravel.Infrastructure.Data.Contexts;

/// <summary>
/// Used only at design-time (migrations). Supplies a null tenant so query filters
/// short-circuit and the schema is generated without tenant isolation applied.
/// </summary>
public class LitXusTravelDbContextFactory : IDesignTimeDbContextFactory<LitXusTravelDbContext>
{
    public LitXusTravelDbContext CreateDbContext(string[] args)
    {
        // Override via `ConnectionStrings__DefaultConnection` env var so real credentials
        // never need to live in this tracked file. Falls back to a generic local placeholder.
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=litxustravel_dev;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<LitXusTravelDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(LitXusTravelDbContext).Assembly.FullName))
            .Options;

        return new LitXusTravelDbContext(options, new NullCurrentTenant());
    }

    private sealed class NullCurrentTenant : ICurrentTenant
    {
        public Guid? Id => null;
        public string? Subdomain => null;
        public bool IsResolved => false;
        public void SetTenant(Guid tenantId, string subdomain) { }
    }
}
