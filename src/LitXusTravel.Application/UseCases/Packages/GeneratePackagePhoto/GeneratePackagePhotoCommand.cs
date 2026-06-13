using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.GeneratePackagePhoto;

public record GeneratePackagePhotoCommand(Guid TenantId, Guid TenantPackageId) : IRequest<Result<string>>;
