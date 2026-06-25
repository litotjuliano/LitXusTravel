using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using MediatR;

namespace LitXusTravel.Application.UseCases.Tenants.GetSubscriptionStatus;

public record GetSubscriptionStatusQuery(Guid TenantId)
    : IRequest<Result<TenantSubscriptionStatusResponse>>;
