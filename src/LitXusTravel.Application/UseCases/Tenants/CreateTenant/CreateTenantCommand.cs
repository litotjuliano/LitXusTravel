using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Tenants.CreateTenant;

public record CreateTenantCommand(
    string Name,
    string Email,
    string? Phone,
    string? Country
) : IRequest<Result<TenantResponse>>;
