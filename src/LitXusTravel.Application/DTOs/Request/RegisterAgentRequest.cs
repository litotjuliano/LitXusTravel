namespace LitXusTravel.Application.DTOs.Request;

public record RegisterAgentRequest(
    Guid TenantId,
    string Email,
    string Password,
    string FirstName,
    string LastName
);
