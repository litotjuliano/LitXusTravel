using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;

namespace LitXusTravel.Application.UseCases.Packages.GetPackageById;

public record GetPackageByIdQuery(Guid PackageId) : IRequest<Result<PackageResponse>>;
