using LitXusTravel.Application.Interfaces.Services;

namespace LitXusTravel.Infrastructure.Services;

public class WhatsAppService : IWhatsAppService
{
    public string BuildClickToChatUrl(string phoneNumber, string? message = null)
    {
        var phone = phoneNumber.Replace("+", "").Replace("-", "").Replace(" ", "");
        var url = $"https://wa.me/{phone}";

        if (!string.IsNullOrWhiteSpace(message))
            url += $"?text={Uri.EscapeDataString(message)}";

        return url;
    }

    public Task SendMessageAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        // Integration with WhatsApp Business API would go here
        return Task.CompletedTask;
    }
}
