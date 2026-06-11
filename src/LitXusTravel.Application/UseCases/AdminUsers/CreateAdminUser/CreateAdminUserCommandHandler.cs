using MediatR;
using LitXusTravel.Application.Common.Constants;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using LitXusTravel.Domain.ValueObjects;

namespace LitXusTravel.Application.UseCases.AdminUsers.CreateAdminUser;

public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public CreateAdminUserCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CreateAdminUserCommand request, CancellationToken ct)
    {
        // Check if email already exists
        var existingAdmin = await _unitOfWork.AdminUsers.ExistsByEmailAsync(new Email(request.Email), ct);
        if (existingAdmin)
            return Result<Guid>.Failure($"Admin with email {request.Email} already exists");

        try
        {
            // Create the admin user based on scope
            AdminUser admin = request.Scope switch
            {
                AdminScope.Platform => AdminUser.CreatePlatformAdmin(request.Name, new Email(request.Email)),
                AdminScope.Tenant => AdminUser.CreateTenantAdmin(
                    request.Name,
                    new Email(request.Email),
                    request.AssignedTenantId!.Value),
                _ => throw new InvalidOperationException("Invalid admin scope")
            };

            await _unitOfWork.AdminUsers.AddAsync(admin, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Log audit trail
            await _auditService.LogAsync(
                action: AuditAction.CreateAdmin,
                affectedEntityType: nameof(AdminUser),
                affectedEntityId: admin.Id,
                affectedTenantId: request.AssignedTenantId,
                reason: $"Created {request.Scope} admin",
                ct: ct);

            return Result<Guid>.Success(admin.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
