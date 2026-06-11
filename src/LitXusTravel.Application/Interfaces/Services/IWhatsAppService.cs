namespace LitXusTravel.Application.Interfaces.Services;

public interface IWhatsAppService
{
    string BuildClickToChatUrl(string phoneNumber, string? message = null);
    Task SendMessageAsync(string phoneNumber, string message, CancellationToken ct = default);
}
