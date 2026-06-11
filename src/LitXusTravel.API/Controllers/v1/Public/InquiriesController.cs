using MediatR;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Inquiries.CreateInquiry;

namespace LitXusTravel.API.Controllers.v1.Public;

[ApiController]
[Route("api/v1/public/websites/{subdomain}")]
public class InquiriesController(IMediator mediator) : ControllerBase
{
    /// <summary>Get website metadata (SPEC-PUBLIC-001)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWebsiteMetadata(string subdomain, CancellationToken ct)
    {
        // TODO: Implement GetWebsiteMetadataQuery
        return Ok(new { subdomain, companyName = "LitXusTravel", syncedPackagesCount = 0 });
    }

    /// <summary>Submit customer inquiry (SPEC-PUBLIC-004)</summary>
    [HttpPost("inquiries")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Submit(
        string subdomain,
        [FromBody] PublicInquiryRequest request,
        [FromServices] Application.Interfaces.Services.ITenantResolver resolver,
        CancellationToken ct)
    {
        var tenantId = await resolver.ResolveFromSubdomainAsync(subdomain, ct);
        if (tenantId is null)
            return NotFound(new { message = "Website not found." });

        var command = new CreateInquiryCommand(
            tenantId.Value, request.CustomerName, request.CustomerEmail,
            request.CustomerPhone, request.Message,
            request.MasterPackageId, request.NumberOfPax, request.PreferredTravelDates);

        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });

        return Created("", new
        {
            inquiryId = result.Value!.Id,
            status = "new",
            message = "Thank you. Our agent will contact you shortly."
        });
    }
}

public record PublicInquiryRequest(
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string Message,
    Guid? MasterPackageId,
    int? NumberOfPax,
    string? PreferredTravelDates);
