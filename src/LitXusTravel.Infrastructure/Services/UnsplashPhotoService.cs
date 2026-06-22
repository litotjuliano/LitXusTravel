using System.Net.Http;
using System.Net.Http.Json;
using LitXusTravel.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LitXusTravel.Infrastructure.Services;

public class UnsplashPhotoService(IHttpClientFactory httpFactory, IConfiguration config, ILogger<UnsplashPhotoService> logger)
    : IPhotoService
{
    public async Task<string?> GetPhotoUrlAsync(string destination, string? category, CancellationToken ct = default)
    {
        var key = config["Unsplash:AccessKey"];
        if (string.IsNullOrWhiteSpace(key)) return null;

        var query = Uri.EscapeDataString($"{destination} {category ?? "travel"} travel");
        var url = $"https://api.unsplash.com/photos/random?query={query}&orientation=landscape&content_filter=high&client_id={key}";

        try
        {
            var client = httpFactory.CreateClient("Unsplash");
            var result = await client.GetFromJsonAsync<UnsplashPhotoResult>(url, ct);
            return result?.Urls?.Regular;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Caller (HTTP request) was aborted client-side — not an Unsplash failure, nothing to report.
            logger.LogInformation("Unsplash fetch cancelled by caller for query '{Query}'", query);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unsplash fetch failed for query '{Query}'", query);
            return null;
        }
    }
}

file record UnsplashPhotoResult(UnsplashUrls? Urls);
file record UnsplashUrls(string? Regular);
