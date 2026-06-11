namespace LitXusTravel.Application.DTOs.Response;

public record CommissionStatementDto(
    Guid AgentId,
    string AgentName,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    decimal AccruedAmount,
    decimal FinalizedAmount,
    decimal PaidAmount,
    decimal ReversedAmount,
    int BookingCount,
    int CompletedBookingCount,
    List<CommissionAccrualLineItemDto> Accruals);

public record CommissionAccrualLineItemDto(
    Guid Id,
    Guid SourceId,
    decimal Amount,
    decimal? Percentage,
    string Status,
    DateTime AccruedAt,
    string Description);
