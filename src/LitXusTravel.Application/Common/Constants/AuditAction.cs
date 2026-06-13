namespace LitXusTravel.Application.Common.Constants;

public enum AuditAction
{
    Create = 0,
    Update = 1,
    Delete = 2,
    CreateAdmin = 3,
    DeactivateAdmin = 4,
    ActivateAdmin = 5,
    CreateStaffAgent = 6,
    CreateCommissionRule = 7,
    UpdateCommissionRule = 8,
    ProcessCommissionPayout = 9,
    CreateDispute = 10,
    ResolveDispute = 11,
    CreateInquiry = 12,
    CreatePackage = 13,
    PublishPackage = 14,
    SyncPackages = 15,
    CreateTenant = 16
}
