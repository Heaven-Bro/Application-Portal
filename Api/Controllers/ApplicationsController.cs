namespace Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Applications.Commands;
using Application.Features.Applications.Queries;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Applicant")]
public class ApplicationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("my-applications")]
    public async Task<IActionResult> GetMyApplications()
    {
        var query = new GetMyApplicationsQuery();
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(new { message = result.Error });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationDetails(long id)
    {
        var query = new GetApplicationDetailsQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(new { message = result.Error });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        var command = new CreateApplicationCommand(request.ServiceId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { applicationId = result.Value });
    }

    [HttpPost("{id}/submit-step")]
    public async Task<IActionResult> SubmitStep(long id, [FromBody] SubmitStepRequest request)
    {
        var command = new SubmitStepCommand(
            id,
            request.StepId,
            request.FormData,
            request.DocumentIds ?? new List<long>()
        );
        
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
            return Ok(new { submissionId = result.Value });
        
        return BadRequest(new { message = result.Error });
    }
}

public record CreateApplicationRequest(long ServiceId);

public record SubmitStepRequest(
    long StepId,
    string? FormData,
    List<long>? DocumentIds
);
