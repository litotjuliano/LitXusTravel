using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Notifications.GetAdminNotifications;

public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    bool IsRead,
    DateTimeOffset CreatedAt);

public record GetAdminNotificationsQuery(string UserId) : IRequest<Result<List<NotificationDto>>>;
