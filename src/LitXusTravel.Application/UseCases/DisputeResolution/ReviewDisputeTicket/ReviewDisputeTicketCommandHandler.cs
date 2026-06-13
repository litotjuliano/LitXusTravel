using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using LitXusTravel.Application.Interfaces.Services;
using LitXusTravel.Domain.Entities;
using LitXusTravel.Domain.Exceptions;
using MediatR;

namespace LitXusTravel.Application.UseCases.DisputeResolution.ReviewDisputeTicket;

public class ReviewDisputeTicketCommandHandler : IRequestHandler<ReviewDisputeTicketCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public ReviewDisputeTicketCommandHandler(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<bool>> Handle(ReviewDisputeTicketCommand request, CancellationToken ct)
    {
        var ticket = await _unitOfWork.DisputeResolutionTickets.GetByIdAsync(request.TicketId, ct);
        if (ticket is null)
            return Result<bool>.Failure("Dispute ticket not found.");

        try
        {
            if (request.IsApproved)
            {
                if (!request.AdjustedAmount.HasValue || request.AdjustedAmount <= 0)
                    return Result<bool>.Failure("Adjusted amount is required when approving a dispute.");

                ticket.Approve(request.TenantAdminId, request.AdjustedAmount.Value);
            }
            else
            {
                ticket.Reject(request.TenantAdminId);
            }

            _unitOfWork.DisputeResolutionTickets.Update(ticket);
            await _unitOfWork.SaveChangesAsync(ct);

            await _auditService.LogAsync(
                action: AuditAction.StatusChanged,
                entityType: nameof(DisputeResolutionTicket),
                entityId: ticket.Id,
                ct: ct);

            return Result<bool>.Success(true);
        }
        catch (DomainException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
