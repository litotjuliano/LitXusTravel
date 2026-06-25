using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.Notifications.GetAdminNotifications;

public class GetAdminNotificationsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAdminNotificationsQuery, Result<List<NotificationDto>>>
{
    public async Task<Result<List<NotificationDto>>> Handle(
        GetAdminNotificationsQuery request, CancellationToken ct)
    {
        var all = await uow.Notifications.FindAsync(
            n => n.UserId == request.UserId, ct);

        var dtos = all
            .OrderByDescending(n => n.CreatedAt)
            .Take(100)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.IsRead, n.CreatedAt))
            .ToList();

        return Result<List<NotificationDto>>.Success(dtos);
    }
}
