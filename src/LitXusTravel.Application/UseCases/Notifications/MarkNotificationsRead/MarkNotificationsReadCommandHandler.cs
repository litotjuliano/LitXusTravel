using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.Notifications.MarkNotificationsRead;

public class MarkNotificationsReadCommandHandler(IUnitOfWork uow)
    : IRequestHandler<MarkNotificationsReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationsReadCommand request, CancellationToken ct)
    {
        var notifications = (await uow.Notifications.FindAsync(
            n => n.UserId == request.UserId && !n.IsRead, ct)).ToList();

        if (request.NotificationId.HasValue)
            notifications = notifications.Where(n => n.Id == request.NotificationId.Value).ToList();

        foreach (var n in notifications)
        {
            n.MarkRead();
            uow.Notifications.Update(n);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
