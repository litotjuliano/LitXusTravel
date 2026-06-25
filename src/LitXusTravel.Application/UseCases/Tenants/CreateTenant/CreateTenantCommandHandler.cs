using MediatR;
using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.DTOs.Response;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Application.UseCases.Tenants.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public CreateTenantCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<TenantResponse>> Handle(CreateTenantCommand request, CancellationToken ct)
    {
        try
        {
            var tenant = Tenant.Create(request.Name, request.Email, request.Phone, request.Country);

            await _unitOfWork.Tenants.AddAsync(tenant, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                action: AuditAction.Created,
                entityType: nameof(Tenant),
                entityId: tenant.Id,
                ct: ct);

            var response = new TenantResponse(
                tenant.Id,
                tenant.Name,
                tenant.Slug,
                tenant.Subdomain,
                tenant.ContactEmail.Value,
                tenant.ContactPhone,
                tenant.LogoUrl,
                tenant.IsActive,
                tenant.ProvisioningStatus.ToString(),
                tenant.WebsiteUrl,
                tenant.CreatedAt);

            return Result<TenantResponse>.Success(response);
        }
        catch (DomainException ex)
        {
            return Result<TenantResponse>.Failure(ex.Message);
        }
    }
}
