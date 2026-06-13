using MediatR;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.Application.UseCases.Public;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Tests.Integration.Builders;
using LitXusTravel.Tests.Integration.Fixtures;

namespace LitXusTravel.Tests.Integration.Tests;

public class PublicApiIntegrationTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task GetWebsiteMetadata_WithValidSubdomain_ReturnsMetadata()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithName("Public Test Tenant")
                .WithEmail("public@example.com")
                .WithSubdomain("publictest")
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetWebsiteMetadataQuery("publictest");

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var metadata = result.Value;
            Assert.NotNull(metadata);
            Assert.Equal("Public Test Tenant", metadata.TenantName);
        });
    }

    [Fact]
    public async Task GetPublicPackages_WithPublishedPackages_ReturnsPackages()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithName("Public Packages Tenant")
                .WithEmail("public-packages@example.com")
                .WithSubdomain("publicpkg")
                .Build();

            var package1 = new PackageBuilder()
                .WithTitle("Published Package 1")
                .WithDestination("Bali")
                .WithVisibility(PackageVisibility.Published)
                .Build();

            var package2 = new PackageBuilder()
                .WithTitle("Published Package 2")
                .WithDestination("Lombok")
                .WithVisibility(PackageVisibility.Published)
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddRangeAsync(package1, package2);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetPublicPackagesQuery("publicpkg");

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var packages = result.Value?.Items;
            Assert.NotNull(packages);
            Assert.Equal(2, packages.Count());
        });
    }

    [Fact]
    public async Task GetPublicPackages_WithDraftPackages_ExcludesDraft()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithSubdomain("excludedraft")
                .Build();

            var publishedPackage = new PackageBuilder()
                .WithTitle("Published")
                .WithVisibility(PackageVisibility.Published)
                .Build();

            var draftPackage = new PackageBuilder()
                .WithTitle("Draft")
                .WithVisibility(PackageVisibility.Draft)
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddRangeAsync(publishedPackage, draftPackage);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetPublicPackagesQuery("excludedraft");

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var packages = result.Value?.Items;
            Assert.NotNull(packages);
            Assert.Single(packages);
            Assert.Equal("Published", packages.First().Title);
        });
    }

    [Fact]
    public async Task GetPublicPackageDetail_WithValidId_ReturnsDetail()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithSubdomain("detail")
                .Build();

            var package = new PackageBuilder()
                .WithTitle("Detail Test Package")
                .WithDestination("Jakarta")
                .WithCategory("Urban & City")
                .WithVisibility(PackageVisibility.Published)
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddAsync(package);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetPublicPackageDetailQuery(package.Id);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var detail = result.Value;
            Assert.NotNull(detail);
            Assert.Equal("Detail Test Package", detail.Title);
            Assert.Equal("Jakarta", detail.Destination);
            Assert.Equal("Urban & City", detail.Category);
        });
    }

    [Fact]
    public async Task GetPublicPackageDetail_WithDraftPackage_ReturnsFailed()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithSubdomain("draftdetail")
                .Build();

            var draftPackage = new PackageBuilder()
                .WithTitle("Draft Package")
                .WithVisibility(PackageVisibility.Draft)
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddAsync(draftPackage);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetPublicPackageDetailQuery(draftPackage.Id);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.False(result.IsSuccess);
        });
    }
}
