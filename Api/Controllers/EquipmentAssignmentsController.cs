namespace Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.EquipmentAssignments.Commands;
using Application.Features.EquipmentAssignments.Queries;
using Domain.Common.Enums;

[ApiController]
[Route("api/[controller]")]
public class EquipmentAssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentAssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Admin endpoints
    [HttpGet("available")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAvailable()
    {
        var query = new GetAvailableEquipmentsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign([FromBody] AssignEquipmentRequest request)
    {
        var command = new AssignEquipmentCommand(
            request.ApplicationId,
            request.EquipmentId,
            request.ExpectedReturnDate
        );

        var assignmentId = await _mediator.Send(command);
        return Ok(new { assignmentId, message = "Equipment assigned successfully" });
    }

    [HttpPost("{assignmentId}/approve-return")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveReturnAsGood(long assignmentId)
    {
        var command = new ApproveReturnAsGoodCommand(assignmentId);
        await _mediator.Send(command);
        return Ok(new { message = "Return approved as good" });
    }

    [HttpPost("{assignmentId}/mark-damaged")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAsDamaged(long assignmentId, [FromBody] MarkDamagedRequest request)
    {
        var command = new MarkAsDamagedCommand(assignmentId, request.AdminNotes);
        await _mediator.Send(command);
        return Ok(new { message = "Equipment marked as damaged" });
    }

    [HttpPost("{assignmentId}/reject-return")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectReturn(long assignmentId, [FromBody] RejectReturnRequest request)
    {
        var command = new RejectReturnCommand(assignmentId, request.AdminNotes);
        await _mediator.Send(command);
        return Ok(new { message = "Return rejected" });
    }

    [HttpPost("{assignmentId}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResolveManually(long assignmentId, [FromBody] ResolveManuallyRequest request)
    {
        var command = new ResolveManuallyCommand(assignmentId, request.FinalStatus, request.Notes);
        await _mediator.Send(command);
        return Ok(new { message = "Assignment resolved manually" });
    }

    [HttpGet("overdue")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOverdue()
    {
        var query = new GetOverdueAssignmentsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Applicant endpoints
    [HttpPost("{assignmentId}/request-return")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> RequestReturn(long assignmentId)
    {
        var command = new RequestReturnCommand(assignmentId);
        await _mediator.Send(command);
        return Ok(new { message = "Return request submitted" });
    }

    [HttpPost("{assignmentId}/acknowledge-damage")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> AcknowledgeDamage(long assignmentId)
    {
        var command = new AcknowledgeDamageCommand(assignmentId);
        await _mediator.Send(command);
        return Ok(new { message = "Damage acknowledged" });
    }

    [HttpPost("{assignmentId}/dispute-damage")]
    [Authorize(Roles = "Applicant")]
    public async Task<IActionResult> DisputeDamage(long assignmentId, [FromBody] DisputeDamageRequest request)
    {
        var command = new DisputeDamageCommand(assignmentId, request.ApplicantResponse);
        await _mediator.Send(command);
        return Ok(new { message = "Damage disputed" });
    }

    // Shared endpoints
    [HttpGet("application/{applicationId}")]
    [Authorize]
    public async Task<IActionResult> GetByApplication(long applicationId)
    {
        var query = new GetAssignmentsByApplicationQuery(applicationId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

// Request models
public record AssignEquipmentRequest(long ApplicationId, long EquipmentId, DateTime? ExpectedReturnDate);
public record MarkDamagedRequest(string AdminNotes);
public record RejectReturnRequest(string AdminNotes);
public record ResolveManuallyRequest(EquipmentAssignmentStatus FinalStatus, string Notes);
public record DisputeDamageRequest(string ApplicantResponse);
