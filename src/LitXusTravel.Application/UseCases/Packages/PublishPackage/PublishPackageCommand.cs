using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Packages.PublishPackage;

public record PublishPackageCommand(Guid PackageId) : IRequest<Result<PackageResponse>>;
