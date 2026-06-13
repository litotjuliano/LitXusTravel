namespace LitXusTravel.Application.Interfaces.Services;

public interface IPhotoService
{
    Task<string?> GetPhotoUrlAsync(string destination, string? category, CancellationToken ct = default);
}
