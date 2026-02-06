namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.Features.Services.Commands;
using Application.Features.Services.Queries;
using Domain.Common.Enums;

[ApiController]
[Route("api/[controller]")]
public class ServicesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetServices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? type = null)
    {
        var query = new GetServicesPagedQuery(page, pageSize, search, type);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            return Unauthorized(new { error = "Invalid user id claim." });

        var command = new CreateServiceCommand(
            request.Name,
            request.Description,
            request.ServiceType,
            request.Steps.Select(s => new CreateServiceStepDto(
                s.Name,
                s.Order,
                s.RequiresFileUpload,
                s.RequiresTextInput,
                s.Instructions,
                s.DownloadableFormUrl,
                s.IsOptional,
                s.UploadConfig,
                s.RequiresApproval
            )).ToList(),
            userId
        );

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { serviceId = result.Value });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateService(long id, [FromBody] UpdateServiceRequest request)
    {
        var command = new UpdateServiceCommand(
            id,
            request.Name,
            request.Description,
            (int)request.ServiceType,
            request.IsActive,
            request.Steps.Select(s => new UpdateServiceStepDto(
                s.Id,
                s.Name,
                s.Order,
                s.RequiresFileUpload,
                s.RequiresTextInput,
                s.Instructions,
                s.DownloadableFormUrl,
                s.IsOptional,
                s.UploadConfig,
                s.RequiresApproval
            )).ToList()
        );

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "Service updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteService(long id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            return Unauthorized(new { error = "Invalid user id claim." });

        var command = new DeleteServiceCommand(id, userId);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPut("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public IActionResult ToggleStatus(long id)
    {
        return Ok(new { message = "Toggle not implemented yet" });
    }
}

public record CreateServiceRequest(
    string Name,
    string Description,
    ServiceType ServiceType,
    List<CreateServiceStepRequest> Steps
);

public record CreateServiceStepRequest(
    string Name,
    int Order,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    string? Instructions,
    string? DownloadableFormUrl,
    bool IsOptional,
    string? UploadConfig,
    bool RequiresApproval
);

public record UpdateServiceRequest(
    string Name,
    string Description,
    ServiceType ServiceType,
    bool IsActive,
    List<UpdateServiceStepRequest> Steps
);

public record UpdateServiceStepRequest(
    long? Id,
    string Name,
    int Order,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    string? Instructions,
    string? DownloadableFormUrl,
    bool IsOptional,
    string? UploadConfig,
    bool RequiresApproval
);

public record UpdateDescriptionRequest(string Description);
