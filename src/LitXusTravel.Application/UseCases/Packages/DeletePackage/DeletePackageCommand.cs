using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Packages.DeletePackage;

public record DeletePackageCommand(Guid Id) : IRequest<Result<string>>;
