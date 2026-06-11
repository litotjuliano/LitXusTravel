namespace LitXusTravel.Application.UseCases.CommissionAccruals.GetCommissionStatement;

public record GetCommissionStatementQuery(
    Guid AgentId,
    DateTime? PeriodStart = null,
    DateTime? PeriodEnd = null) : IRequest<Result<CommissionStatementDto>>;
