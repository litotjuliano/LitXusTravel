using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.AdminUsers.GetAdminById;

public record GetAdminByIdQuery(Guid AdminId) : IRequest<Result<AdminUserDto>>;
