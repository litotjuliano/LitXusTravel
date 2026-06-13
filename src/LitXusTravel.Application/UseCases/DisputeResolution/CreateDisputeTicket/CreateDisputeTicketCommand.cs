using LitXusTravel.Application.Common.Models;
using LitXusTravel.Domain.Entities;
using MediatR;

namespace LitXusTravel.Application.UseCases.DisputeResolution.CreateDisputeTicket;

public record CreateDisputeTicketCommand(
    Guid SuperAdminId,
    Guid CommissionAccrualId,
    string Description,
    string ProposedFix,
    DisputeReasonCode ReasonCode,
    decimal OriginalAmount)
    : IRequest<Result<Guid>>;
