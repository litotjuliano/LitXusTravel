using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LitXusTravel.API;
using LitXusTravel.Infrastructure.Data.Contexts;

namespace LitXusTravel.Tests.Integration.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public DatabaseFixture()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.FirstOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<LitXusTravelDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<LitXusTravelDbContext>(options =>
                        options.UseInMemoryDatabase("LitXusTravel_Integration_Test"));
                });
            });
    }

    public async Task InitializeAsync()
    {
        Client = _factory.CreateClient();
        ServiceProvider = _factory.Services;

        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LitXusTravelDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LitXusTravelDbContext>();
        await dbContext.Database.EnsureDeletedAsync();

        _factory?.Dispose();
    }

    public async Task<T> ExecuteInScopeAsync<T>(
        Func<LitXusTravelDbContext, IServiceProvider, Task<T>> action)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LitXusTravelDbContext>();
        return await action(dbContext, scope.ServiceProvider);
    }

    public async Task ExecuteInScopeAsync(
        Func<LitXusTravelDbContext, IServiceProvider, Task> action)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LitXusTravelDbContext>();
        await action(dbContext, scope.ServiceProvider);
    }
}
