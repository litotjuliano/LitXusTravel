using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Packages.UnsyncPackage;

public record UnsyncPackageCommand(
    Guid TenantId,
    Guid TenantPackageId
) : IRequest<Result<string>>;
