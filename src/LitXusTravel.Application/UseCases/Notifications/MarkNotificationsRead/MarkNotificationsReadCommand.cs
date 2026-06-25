using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Notifications.MarkNotificationsRead;

// NotificationId = null → mark ALL unread notifications for the user
public record MarkNotificationsReadCommand(string UserId, Guid? NotificationId) : IRequest<Result>;
