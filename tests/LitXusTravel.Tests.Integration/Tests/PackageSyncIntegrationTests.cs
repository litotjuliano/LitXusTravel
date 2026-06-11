using MediatR;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.Application.UseCases.Packages.GetTenantPackages;
using LitXusTravel.Application.UseCases.Packages.SyncPackagesToTenant;
using LitXusTravel.Application.UseCases.Packages.UpdatePackageOverride;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Tests.Integration.Builders;
using LitXusTravel.Tests.Integration.Fixtures;

namespace LitXusTravel.Tests.Integration.Tests;

public class PackageSyncIntegrationTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task SyncPackageToTenant_WithNewPackage_CreatesSuccessfully()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithName("Sync Test Tenant")
                .WithEmail("sync-test@example.com")
                .Build();

            var package = new PackageBuilder()
                .WithTitle("Sync Test Package")
                .WithDestination("Penang")
                .WithPrice(2000m)
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddAsync(package);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();

            // Act
            var command = new SyncPackagesToTenantCommand(tenant.Id, [package.Id]);
            var result = await mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);

            var tenantPackages = dbContext.TenantPackages
                .Where(tp => tp.TenantId == tenant.Id)
                .ToList();

            Assert.Single(tenantPackages);
            Assert.Equal(package.Id, tenantPackages.First().MasterPackageId);
        });
    }

    [Fact]
    public async Task SyncPackageToTenant_WithExistingSync_PreservesOverrides()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder().Build();
            var package = new PackageBuilder()
                .WithTitle("Override Test Package")
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddAsync(package);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();

            // First sync
            var command1 = new SyncPackagesToTenantCommand(tenant.Id, [package.Id]);
            await mediator.Send(command1);

            var tenantPackage = dbContext.TenantPackages
                .First(tp => tp.TenantId == tenant.Id && tp.MasterPackageId == package.Id);

            // Create an override
            var overrideCommand = new UpdatePackageOverrideCommand(
                tenant.Id, tenantPackage.Id, "Overridden Title", null, null, null, null);

            await mediator.Send(overrideCommand);

            // Act - sync again
            var command2 = new SyncPackagesToTenantCommand(tenant.Id, [package.Id]);
            var result = await mediator.Send(command2);

            // Assert
            Assert.True(result.IsSuccess);

            var overridedPackage = dbContext.TenantPackages
                .First(tp => tp.TenantId == tenant.Id && tp.MasterPackageId == package.Id);

            // Override should still exist
            var packageOverrides = dbContext.PackageOverrides
                .Where(po => po.TenantPackageId == overridedPackage.Id)
                .ToList();

            Assert.NotEmpty(packageOverrides);
        });
    }

    [Fact]
    public async Task GetTenantPackages_WithOverrides_ReturnsResolvedPackages()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder().Build();
            var package = new PackageBuilder()
                .WithTitle("Master Title")
                .WithPrice(1000m)
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddAsync(package);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();

            // Sync package
            var syncCommand = new SyncPackagesToTenantCommand(tenant.Id, [package.Id]);
            await mediator.Send(syncCommand);

            // Create override
            var tenantPackage = dbContext.TenantPackages
                .First(tp => tp.TenantId == tenant.Id && tp.MasterPackageId == package.Id);

            var overrideCommand = new UpdatePackageOverrideCommand(
                tenant.Id, tenantPackage.Id, "Overridden Title", 2000m, null, null, null);

            await mediator.Send(overrideCommand);

            // Act
            var getQuery = new GetTenantPackagesQuery(tenant.Id, 1, 20);
            var result = await mediator.Send(getQuery);

            // Assert
            Assert.True(result.IsSuccess);
            var packages = result.Value?.Items;
            Assert.NotNull(packages);
            Assert.Single(packages);

            var resolved = packages.First();
            Assert.Equal("Overridden Title", resolved.Title);
            Assert.Equal(2000m, resolved.Price);
        });
    }
}
