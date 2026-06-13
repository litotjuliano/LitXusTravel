using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.DisputeResolution.CreateDisputeTicket;

public class CreateDisputeTicketCommandHandler : IRequestHandler<CreateDisputeTicketCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public CreateDisputeTicketCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<Guid>> Handle(CreateDisputeTicketCommand request, CancellationToken ct)
    {
        try
        {
            var ticket = DisputeResolutionTicket.Create(
                request.SuperAdminId,
                request.CommissionAccrualId,
                request.Description,
                request.ProposedFix,
                request.ReasonCode,
                request.OriginalAmount);

            await _unitOfWork.DisputeResolutionTickets.AddAsync(ticket, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                action: AuditAction.Created,
                entityType: nameof(DisputeResolutionTicket),
                entityId: ticket.Id,
                ct: ct);

            return Result<Guid>.Success(ticket.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
