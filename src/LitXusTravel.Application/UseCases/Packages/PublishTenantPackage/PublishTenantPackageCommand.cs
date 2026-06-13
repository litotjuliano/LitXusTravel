using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.PublishTenantPackage;

public record PublishTenantPackageCommand(Guid TenantId, Guid TenantPackageId) : IRequest<Result>;
