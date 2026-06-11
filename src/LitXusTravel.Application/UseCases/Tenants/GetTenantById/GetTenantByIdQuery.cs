using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Tenants.GetTenantById;

public record GetTenantByIdQuery(Guid TenantId) : IRequest<Result<TenantResponse>>;
