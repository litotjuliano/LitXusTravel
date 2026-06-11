using LitXusTravel.Domain.Common;
using LitXusTravel.Domain.Exceptions;

namespace LitXusTravel.Domain.Entities;

public enum ActivityType { Created, Contacted, Quoted, StatusChanged, Note }

public class CrmActivity : BaseEntity
{
    public Guid InquiryId { get; private set; }
    public Guid TenantId { get; private set; }
    public ActivityType Type { get; private set; }
    public string? Notes { get; private set; }
    public Guid? PerformedByUserId { get; private set; }

    public Inquiry Inquiry { get; private set; } = null!;

    private CrmActivity() { }

    public static CrmActivity Create(Guid inquiryId, Guid tenantId,
        ActivityType type, string? notes = null, Guid? performedByUserId = null)
    {
        if (type == ActivityType.Note && string.IsNullOrWhiteSpace(notes))
            throw new DomainException("Notes are required for a note activity.");

        return new CrmActivity
        {
            InquiryId = inquiryId,
            TenantId = tenantId,
            Type = type,
            Notes = notes,
            PerformedByUserId = performedByUserId
        };
    }
}
