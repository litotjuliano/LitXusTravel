using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.GenerateAdminPackagePhoto;

public record GenerateAdminPackagePhotoCommand(Guid PackageId) : IRequest<Result<string>>;
