using System.Net;
using System.Net.Http.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.Application.UseCases.Packages.CreatePackage;
using LitXusTravel.Application.UseCases.Packages.GetPackageById;
using LitXusTravel.Application.UseCases.Packages.GetPackages;
using LitXusTravel.Tests.Integration.Builders;
using LitXusTravel.Tests.Integration.Fixtures;

namespace LitXusTravel.Tests.Integration.Tests;

public class PackageApiIntegrationTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CreatePackage_WithValidData_ReturnsSuccess()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var mediator = sp.GetRequiredService<IMediator>();
            var command = new CreatePackageCommand(
                "Integration Test Package",
                "Bangkok",
                3000m,
                7,
                "Adventure",
                "Test description",
                "Short test description",
                null, null, null, null, null, null, null, null, null);

            // Act
            var result = await mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Integration Test Package", result.Value.Title);
        });
    }

    [Fact]
    public async Task GetPackageById_WithValidId_ReturnsPackage()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var package = new PackageBuilder()
                .WithTitle("Get Test Package")
                .WithDestination("Singapore")
                .Build();

            await dbContext.Packages.AddAsync(package);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetPackageByIdQuery(package.Id);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Get Test Package", result.Value.Title);
            Assert.Equal("Singapore", result.Value.Destination);
        });
    }

    [Fact]
    public async Task GetPackageById_WithInvalidId_ReturnsFailed()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetPackageByIdQuery(invalidId);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        });
    }

    [Fact]
    public async Task ListPackages_WithPagination_ReturnsPaginatedResults()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var packages = Enumerable.Range(1, 25)
                .Select(i => new PackageBuilder()
                    .WithTitle($"Package {i}")
                    .WithDestination($"Destination {i}")
                    .Build())
                .ToList();

            await dbContext.Packages.AddRangeAsync(packages);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetPackagesQuery(null, null, 1, 10);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var paginatedResult = result.Value;
            Assert.NotNull(paginatedResult);
            Assert.Equal(10, paginatedResult.Items.Count());
            Assert.True(paginatedResult.HasNextPage);
        });
    }
}
