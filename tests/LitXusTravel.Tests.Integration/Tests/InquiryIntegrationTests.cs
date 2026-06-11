using MediatR;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.Application.UseCases.Inquiries.GetInquiries;
using LitXusTravel.Application.UseCases.Inquiries.SubmitInquiry;
using LitXusTravel.Application.UseCases.Inquiries.UpdateInquiryStatus;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Tests.Integration.Builders;
using LitXusTravel.Tests.Integration.Fixtures;

namespace LitXusTravel.Tests.Integration.Tests;

public class InquiryIntegrationTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task SubmitInquiry_WithValidData_CreatesInquiry()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder()
                .WithName("Inquiry Test Tenant")
                .WithEmail("inquiry-test@example.com")
                .Build();

            var package = new PackageBuilder()
                .WithTitle("Test Package")
                .WithDestination("Bangkok")
                .Build();

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Packages.AddAsync(package);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var command = new SubmitInquiryCommand(
                tenant.Id,
                package.Id,
                "John Doe",
                "john@example.com",
                "+60123456789",
                "I'm interested in your packages",
                4,
                null);

            // Act
            var result = await mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.NotEqual(Guid.Empty, result.Value.InquiryId);
        });
    }

    [Fact]
    public async Task UpdateInquiryStatus_ChangesStatus()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder().Build();
            var inquiry = Inquiry.Create(
                tenant.Id,
                "Jane Doe",
                "jane@example.com",
                "+60987654321",
                "Inquiry message",
                null,
                2,
                null);

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Inquiries.AddAsync(inquiry);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var command = new UpdateInquiryStatusCommand(
                tenant.Id,
                inquiry.Id,
                "Contacted");

            // Act
            var result = await mediator.Send(command);

            // Assert
            Assert.True(result.IsSuccess);
            var updatedInquiry = dbContext.Inquiries.First(i => i.Id == inquiry.Id);
            Assert.Equal("Contacted", updatedInquiry.Status.ToString());
        });
    }

    [Fact]
    public async Task GetInquiries_WithTenantFilter_ReturnsTenantInquiries()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant1 = new TenantBuilder()
                .WithName("Tenant 1")
                .WithEmail("tenant1@example.com")
                .Build();

            var tenant2 = new TenantBuilder()
                .WithName("Tenant 2")
                .WithEmail("tenant2@example.com")
                .Build();

            var inquiry1 = Inquiry.Create(
                tenant1.Id, "Customer 1", "cust1@example.com", "+11111111", "Message 1", null, 2, null);
            var inquiry2 = Inquiry.Create(
                tenant1.Id, "Customer 2", "cust2@example.com", "+22222222", "Message 2", null, 3, null);
            var inquiry3 = Inquiry.Create(
                tenant2.Id, "Customer 3", "cust3@example.com", "+33333333", "Message 3", null, 4, null);

            await dbContext.Tenants.AddRangeAsync(tenant1, tenant2);
            await dbContext.Inquiries.AddRangeAsync(inquiry1, inquiry2, inquiry3);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetInquiriesQuery(tenant1.Id, null, 1, 20);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var inquiries = result.Value?.Items;
            Assert.NotNull(inquiries);
            Assert.Equal(2, inquiries.Count());
            Assert.All(inquiries, i => Assert.Equal(tenant1.Id.ToString(), i.TenantId.ToString()));
        });
    }

    [Fact]
    public async Task GetInquiries_FilterByStatus_ReturnFiltered()
    {
        await fixture.ExecuteInScopeAsync(async (dbContext, sp) =>
        {
            // Arrange
            var tenant = new TenantBuilder().Build();

            var inquiry1 = Inquiry.Create(
                tenant.Id, "Customer 1", "cust1@example.com", "+11111111", "Message 1", null, 2, null);
            inquiry1.UpdateStatus(InquiryStatus.Contacted);

            var inquiry2 = Inquiry.Create(
                tenant.Id, "Customer 2", "cust2@example.com", "+22222222", "Message 2", null, 3, null);

            await dbContext.Tenants.AddAsync(tenant);
            await dbContext.Inquiries.AddRangeAsync(inquiry1, inquiry2);
            await dbContext.SaveChangesAsync();

            var mediator = sp.GetRequiredService<IMediator>();
            var query = new GetInquiriesQuery(tenant.Id, "Contacted", 1, 20);

            // Act
            var result = await mediator.Send(query);

            // Assert
            Assert.True(result.IsSuccess);
            var inquiries = result.Value?.Items;
            Assert.NotNull(inquiries);
            Assert.Single(inquiries);
            Assert.Equal("Contacted", inquiries.First().Status);
        });
    }
}
