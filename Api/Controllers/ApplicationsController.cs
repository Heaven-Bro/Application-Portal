namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.Features.Applications.Commands;
using Application.Features.Applications.Queries;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("my")]
    public async Task<IActionResult> GetMyApplications()
    {
        var query = new GetMyApplicationsQuery();
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        var command = new CreateApplicationCommand(request.ServiceId);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { applicationId = result.Value });
    }

    [HttpPost("{applicationId}/steps/{stepId}/submit")]
    public async Task<IActionResult> SubmitStep(long applicationId, long stepId, [FromBody] SubmitStepRequest request)
    {
        var command = new SubmitStepCommand(
            applicationId,
            stepId,
            request.FormData,
            request.FilePaths ?? new List<string>()
        );

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { submissionId = result.Value });
    }

    [HttpPut("submissions/{submissionId}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveStep(long submissionId)
    {
        var command = new ApproveStepCommand(submissionId);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "Step approved successfully" });
    }

    [HttpPut("submissions/{submissionId}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectStep(long submissionId, [FromBody] RejectStepRequest request)
    {
        var command = new RejectStepCommand(submissionId, request.Reason);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "Step rejected successfully" });
    }
}

public record CreateApplicationRequest(long ServiceId);
public record SubmitStepRequest(string? FormData, List<string>? FilePaths);
public record RejectStepRequest(string Reason);
