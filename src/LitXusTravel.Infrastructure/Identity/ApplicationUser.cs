using Microsoft.AspNetCore.Identity;

namespace LitXusTravel.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public Guid? TenantId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
