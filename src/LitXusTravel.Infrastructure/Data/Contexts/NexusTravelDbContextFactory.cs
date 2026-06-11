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
        var options = new DbContextOptionsBuilder<LitXusTravelDbContext>()
            .UseSqlServer(
                "Server=.;Database=LitXusTravel_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
                sql => sql.MigrationsAssembly(typeof(LitXusTravelDbContext).Assembly.FullName))
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
