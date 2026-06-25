using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.CommissionRules.ConfigureCommissionRule;
using LitXusTravel.Application.UseCases.CommissionRules.GetCommissionRules;
using LitXusTravel.Domain.Entities;

using LitXusTravel.API.Filters;

namespace LitXusTravel.API.Controllers.v1.Tenants;

[ApiController]
[Route("api/v1/tenants/{tenantId:guid}/commission-rules")]
[Authorize(Roles = "Agent,Admin")]
[TenantAuthorizationFilter]
[SubscriptionWriteGuard]
[Tags("Commission Rules")]
public class CommissionRulesController(IMediator mediator) : ControllerBase
{
    /// <summary>List commission rules for a tenant. Optionally filter by agent.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CommissionRuleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRules(Guid tenantId, [FromQuery] Guid? agentId = null, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetCommissionRulesQuery(tenantId, agentId), ct);
        return Ok(result.Value);
    }

    /// <summary>
    /// Configure a commission rule (default or agent-specific).
    /// Rules can be Default (all agents) or AgentSpecific. Percentage capped at 30% (Safeguard 9).
    /// Triggers: TourBooked, TourCompleted, RevenueGenerated.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfigureRule(Guid tenantId, ConfigureCommissionRuleRequest request, CancellationToken ct = default)
    {
        var command = new ConfigureCommissionRuleCommand(
            tenantId,
            request.AgentId,
            Enum.Parse<CommissionTrigger>(request.Trigger),
            request.Amount,
            request.IsPercentage,
            request.MinimumThreshold ?? 100,
            request.PayoutFrequency ?? "Monthly");

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetRules), new { tenantId }, new { id = result.Value });
    }
}

public record ConfigureCommissionRuleRequest(
    Guid? AgentId,
    string Trigger,
    decimal Amount,
    bool IsPercentage,
    decimal? MinimumThreshold = null,
    string? PayoutFrequency = null);
