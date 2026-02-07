namespace Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.ChairmanAvailability.Queries;
using Application.Features.ChairmanAvailability.Commands;

[ApiController]
[Route("api/chairman-availability")]
public class ChairmanAvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChairmanAvailabilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("current-week")]
    public async Task<IActionResult> GetCurrentWeek()
    {
        var query = new GetCurrentWeekChairmanAvailabilityQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("current-week")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCurrentWeek([FromBody] UpdateChairmanAvailabilityRequest request)
    {
        var command = new UpdateCurrentWeekChairmanAvailabilityCommand(request.Slots);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Chairman availability updated" });

        return BadRequest(new { message = result.Error });
    }
}

public record UpdateChairmanAvailabilityRequest(List<ChairmanAvailabilityInput> Slots);
