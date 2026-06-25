using LitXusTravel.Application.Common.Models;
using MediatR;

namespace LitXusTravel.Application.UseCases.Billing.GetInvoices;

public record GetInvoicesQuery(Guid? TenantId = null) : IRequest<Result<GetInvoicesResponse>>;

public record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    string TenantName,
    Guid TenantId,
    string PlanName,
    decimal Amount,
    string Period,
    string Status,
    DateTime Date);

public record GetInvoicesResponse(
    IReadOnlyList<InvoiceDto> Data,
    decimal MrrCurrentMonth,
    int TotalPaid,
    int TotalPending,
    int TotalFailed);
