namespace Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Applications.Queries;
using Application.Features.Applications.Commands;

[ApiController]
[Route("api/admin/applications")]
[Authorize(Roles = "Admin")]
public class AdminApplicationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminApplicationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? serviceType = null,
        [FromQuery] long? applicantId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] bool? returnRequestsOnly = null)
    {
        var query = new GetAdminApplicationsQuery(page, pageSize, search, status, serviceType, applicantId, fromDate, toDate, returnRequestsOnly);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetails(long id)
    {
        var query = new GetAdminApplicationDetailsQuery(id);
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
            return Ok(result.Value);
        
        return BadRequest(new { message = result.Error });
    }

    [HttpPost("submissions/{submissionId}/approve")]
    public async Task<IActionResult> ApproveSubmission(long submissionId)
    {
        var command = new ApproveStepCommand(submissionId);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
            return Ok(new { message = "Submission approved successfully" });
        
        return BadRequest(new { message = result.Error });
    }

    [HttpPost("submissions/{submissionId}/reject")]
    public async Task<IActionResult> RejectSubmission(long submissionId, [FromBody] RejectSubmissionRequest request)
    {
        var command = new RejectStepCommand(submissionId, request.RejectionReason);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
            return Ok(new { message = "Submission rejected successfully" });
        
        return BadRequest(new { message = result.Error });
    }

    [HttpPost("{applicationId}/schedule")]
    public async Task<IActionResult> ScheduleAppointment(long applicationId, [FromBody] ScheduleAppointmentRequest request)
    {
        var command = new ScheduleAppointmentCommand(applicationId, request.ScheduledDateTime, request.Notes);
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
            return Ok(new { message = "Appointment scheduled successfully" });
        
        return BadRequest(new { message = result.Error });
    }
}

public record RejectSubmissionRequest(string RejectionReason);
public record ScheduleAppointmentRequest(DateTime ScheduledDateTime, string? Notes);
