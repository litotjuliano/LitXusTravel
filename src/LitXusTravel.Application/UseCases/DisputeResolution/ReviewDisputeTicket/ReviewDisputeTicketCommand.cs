using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.DisputeResolution.ReviewDisputeTicket;

public record ReviewDisputeTicketCommand(
    Guid TicketId,
    Guid TenantAdminId,
    bool IsApproved,
    decimal? AdjustedAmount = null)
    : IRequest<Result<bool>>;
