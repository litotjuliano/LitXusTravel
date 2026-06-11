using MediatR;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.Application.UseCases.Tenants.CreateTenant;
using LitXusTravel.Application.UseCases.Tenants.GetTenantById;
using LitXusTravel.Application.UseCases.Tenants.GetTenants;
using LitXusTravel.Domain.ValueObjects;
using LitXusTravel.Tests.Integration.Builders;
using LitXusTravel.Tests.Integration.Fixtures;

namespace LitXusTravel.Tests.Integration.Tests;

public class TenantIntegrationTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CreateTenant_WithValidData_CreatesSuccessfully()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var mediator = sp.GetRequiredService<IMediator>();
            var command = new CreateTenantCommand(
                "New Tenant",
                "newtenant@example.com",
                "+60123456789",
                "https://example.com");

            // Act
            var result = await mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("New Tenant", result.Value.Name);
            Assert.Equal("newtenant@example.com", result.Value.ContactEmail);

            var createdTenant = dbContext.Tenants.FirstOrDefault(t => t.Id == result.Value.Id);
            Assert.NotNull(createdTenant);
        });
    }

    [Fact]
    public async Task GetTenantById_WithValidId_ReturnsTenant()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithName("Specific Tenant")
                .WithEmail("specific@example.com")
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetTenantByIdQuery(tenant.Id);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(tenant.Id, result.Value.Id);
            Assert.Equal("Specific Tenant", result.Value.Name);
        });
    }

    [Fact]
    public async Task GetTenantById_WithInvalidId_ReturnsFailed()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetTenantByIdQuery(invalidId);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        });
    }

    [Fact]
    public async Task GetTenants_WithPagination_ReturnsPaginatedResults()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenants = Enumerable.Range(1, 25)
                .Select(i => new TenantBuilder()
                    .WithName($"Tenant {i}")
                    .WithEmail($"tenant{i}@example.com")
                    .Build())
                .ToList();

            await dbContext.Tenants.AddRangeAsync(tenants);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetTenantsQuery(null, 1, 10);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var paginatedResult = result.Value;
            Assert.NotNull(paginatedResult);
            Assert.Equal(10, paginatedResult.Items.Count());
            Assert.Equal(1, paginatedResult.Page);
            Assert.Equal(10, paginatedResult.PageSize);
            Assert.True(paginatedResult.HasNextPage);
        });
    }

    [Fact]
    public async Task GetTenants_SecondPage_ReturnsCorrectItems()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenants = Enumerable.Range(1, 25)
                .Select(i => new TenantBuilder()
                    .WithName($"Tenant {i:D2}")
                    .WithEmail($"tenant{i}@example.com")
                    .Build())
                .ToList();

            await dbContext.Tenants.AddRangeAsync(tenants);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetTenantsQuery(null, 2, 10);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var paginatedResult = result.Value;
            Assert.NotNull(paginatedResult);
            Assert.Equal(10, paginatedResult.Items.Count());
            Assert.Equal(2, paginatedResult.Page);
            Assert.True(paginatedResult.HasNextPage);
            Assert.True(paginatedResult.HasPreviousPage);
        });
    }
}
