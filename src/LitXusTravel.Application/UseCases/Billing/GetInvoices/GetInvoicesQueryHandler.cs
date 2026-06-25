using LitXusTravel.Application.Common.Models;
using LitXusTravel.Application.Interfaces.Persistence;
using MediatR;

namespace LitXusTravel.Application.UseCases.Billing.GetInvoices;

public class GetInvoicesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetInvoicesQuery, Result<GetInvoicesResponse>>
{
    public async Task<Result<GetInvoicesResponse>> Handle(GetInvoicesQuery request, CancellationToken ct)
    {
        var all = request.TenantId.HasValue
            ? await uow.Invoices.FindAsync(i => i.TenantId == request.TenantId.Value, ct)
            : await uow.Invoices.GetAllAsync(ct);

        var ordered = all
            .OrderByDescending(i => i.Date)
            .ToList();

        var dtos = ordered.Select(i => new InvoiceDto(
            i.Id, i.InvoiceNumber, i.TenantName, i.TenantId,
            i.PlanName, i.Amount, i.Period, i.Status, i.Date))
            .ToList();

        var now = DateTime.UtcNow;
        var paidThisMonth = ordered
            .Where(i => i.Status == "Paid" && i.Date.Year == now.Year && i.Date.Month == now.Month)
            .ToList();

        var mrr = paidThisMonth.Sum(i => i.Amount);
        var totalPaid = ordered.Count(i => i.Status == "Paid");
        var totalPending = ordered.Count(i => i.Status == "Pending");
        var totalFailed = ordered.Count(i => i.Status == "Failed");

        return Result<GetInvoicesResponse>.Success(
            new GetInvoicesResponse(dtos, mrr, totalPaid, totalPending, totalFailed));
    }
}
