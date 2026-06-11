namespace LitXusTravel.Domain.Entities;

/// <summary>
/// Code usage audit tracks when and where staff referral codes are used.
/// Safeguard 3: Code sharing prevention via IP/location anomaly detection.
/// </summary>
public class CodeUsageAudit : Entity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public DateTime UsedAt { get; private set; }
    public string? CustomerIp { get; private set; }
    public string? CustomerLocation { get; private set; }
    public Guid BookingId { get; private set; }
    public Guid? StaffAgentId { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private CodeUsageAudit() { }

    /// <summary>
    /// Create a code usage audit record.
    /// Safeguard 3: Track IP and location for anomaly detection.
    /// </summary>
    public static CodeUsageAudit Create(
        string code,
        Guid bookingId,
        Guid tenantId,
        Guid? staffAgentId,
        string? customerIp,
        string? customerLocation)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Code is required");
        if (bookingId == Guid.Empty)
            throw new DomainException("Booking ID is required");
        if (tenantId == Guid.Empty)
            throw new DomainException("Tenant ID is required");

        return new CodeUsageAudit
        {
            Id = Guid.NewGuid(),
            Code = code,
            UsedAt = DateTime.UtcNow,
            CustomerIp = customerIp,
            CustomerLocation = customerLocation,
            BookingId = bookingId,
            StaffAgentId = staffAgentId,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
