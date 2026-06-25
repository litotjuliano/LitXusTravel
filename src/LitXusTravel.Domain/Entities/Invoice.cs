using LitXusTravel.Domain.Common;

namespace LitXusTravel.Domain.Entities;

public class Invoice : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string TenantName { get; private set; } = string.Empty;
    public string InvoiceNumber { get; private set; } = string.Empty;
    public string PlanName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Period { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }

    private Invoice() { }

    public static Invoice Create(
        Guid tenantId, string tenantName, string invoiceNumber,
        string planName, decimal amount, string period, string status, DateTime date)
        => new()
        {
            TenantId = tenantId,
            TenantName = tenantName,
            InvoiceNumber = invoiceNumber,
            PlanName = planName,
            Amount = amount,
            Period = period,
            Status = status,
            Date = date
        };
}
