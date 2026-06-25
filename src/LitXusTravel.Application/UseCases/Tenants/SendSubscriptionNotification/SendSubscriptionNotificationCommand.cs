using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tenants.SendSubscriptionNotification;

public record SendSubscriptionNotificationCommand(
    Guid TenantId,
    string Type) : IRequest<Result>;
