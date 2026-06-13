using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Packages.CreatePackage;
using LitXusTravel.Application.UseCases.Packages.GenerateAdminPackagePhoto;
using LitXusTravel.Application.UseCases.Packages.GetPackages;
using LitXusTravel.Application.UseCases.Packages.GetPackageById;
using LitXusTravel.Application.UseCases.Packages.PublishPackage;
using LitXusTravel.Application.UseCases.Packages.UploadPackageImage;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/packages")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class PackagesController(IMediator mediator, IWebHostEnvironment env) : ControllerBase
{
    /// <summary>Create a new master package (SPEC-ADMIN-001)</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePackageCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>List packages with pagination (SPEC-ADMIN-002)</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] string? status,
        [FromQuery] string? destination,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetPackagesQuery(status, destination, page, pageSize);
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });

        var list = result.Value!;
        return Ok(new {
            data = list.Items,
            pagination = new {
                page = list.Page,
                pageSize = list.PageSize,
                totalCount = list.TotalCount,
                totalPages = list.TotalPages,
                hasNextPage = list.HasNextPage,
                hasPreviousPage = list.HasPreviousPage
            }
        });
    }

    /// <summary>Get package by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPackageByIdQuery(id), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });
        return Ok(result.Value);
    }

    /// <summary>Upload a featured image for a package</summary>
    [HttpPost("{id:guid}/image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "File must be under 5 MB." });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
            return BadRequest(new { message = "Only JPG, PNG, or WebP files are allowed." });

        var uploadDir = Path.Combine(env.WebRootPath, "uploads", "packages");
        Directory.CreateDirectory(uploadDir);

        var fileName = $"{id}{ext}";
        var filePath = Path.Combine(uploadDir, fileName);
        await using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream, ct);

        var imageUrl = $"/uploads/packages/{fileName}";
        var result = await mediator.Send(new UploadPackageImageCommand(id, imageUrl), ct);
        if (!result.IsSuccess) return NotFound(new { result.Errors });

        return Ok(new { imageUrl });
    }

    /// <summary>Generate a photo for a master package from Unsplash</summary>
    [HttpPost("{id:guid}/generate-photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GeneratePhoto(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GenerateAdminPackagePhotoCommand(id), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(new { featuredImageUrl = result.Value });
    }

    /// <summary>Publish a master package (SPEC-ADMIN-003)</summary>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new PublishPackageCommand(id), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok(result.Value);
    }
}
