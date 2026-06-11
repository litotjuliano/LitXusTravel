using MediatR;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.CommissionRules.ConfigureCommissionRule;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/commission-rules")]
[Tags("Commission Rules")]
public class CommissionRulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommissionRulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Configure a commission rule (default or agent-specific).
    /// </summary>
    /// <remarks>
    /// Rules can be:
    /// - Default: Apply to all agents in the tenant
    /// - Agent-specific: Apply only to a specific agent
    ///
    /// Safeguards:
    /// - Percentage capped at 30% (Safeguard 9)
    /// - Fixed amounts must be positive
    /// - Only Tenant Admins can configure rules
    ///
    /// Triggers:
    /// - TourBooked: Commission when tour is booked
    /// - TourCompleted: Commission when tour is completed
    /// - RevenueGenerated: Commission based on revenue
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfigureRule(
        Guid tenantId,
        ConfigureCommissionRuleRequest request)
    {
        var command = new ConfigureCommissionRuleCommand(
            tenantId,
            request.AgentId,
            Enum.Parse<CommissionTrigger>(request.Trigger),
            request.Amount,
            request.IsPercentage,
            request.MinimumThreshold ?? 100,
            request.PayoutFrequency ?? "Monthly");

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(null, new { id = result.Data });
    }
}

public record ConfigureCommissionRuleRequest(
    Guid? AgentId,
    string Trigger,
    decimal Amount,
    bool IsPercentage,
    decimal? MinimumThreshold = null,
    string? PayoutFrequency = null);
