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
            return Result.Failure($"Admin with email {request.Email} already exists");

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
            await _auditService.LogActionAsync(
                action: AuditActions.CreateAdmin,
                affectedEntityType: nameof(AdminUser),
                affectedEntityId: admin.Id,
                affectedTenantId: request.AssignedTenantId,
                reason: $"Created {request.Scope} admin",
                ct: ct);

            return Result.Success(admin.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
