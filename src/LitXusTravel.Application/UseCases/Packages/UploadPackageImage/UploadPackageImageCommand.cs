using MediatR;
using LitXusTravel.Application.Common.Models;

namespace LitXusTravel.Application.UseCases.Packages.UploadPackageImage;

public record UploadPackageImageCommand(Guid PackageId, string ImageUrl) : IRequest<Result>;
