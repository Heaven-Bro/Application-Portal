namespace Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.Features.Services.Commands;
using Application.Features.Services.Queries;

[ApiController]
[Route("api/[controller]")]
public class ServicesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        var query = new GetAllServicesQuery();
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        var command = new CreateServiceCommand(
            request.Name,
            request.Description,
            request.Steps.Select(s => new CreateServiceStepDto(
                s.Name,
                s.Description,
                s.Order,
                s.RequiresFileUpload,
                s.RequiresTextInput,
                s.IsEquipmentAssignment
            )).ToList()
        );

        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { serviceId = result.Value });
    }

    [HttpPut("{id}/description")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDescription(long id, [FromBody] UpdateDescriptionRequest request)
    {
        var command = new UpdateServiceDescriptionCommand(id, request.Description);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "Description updated successfully" });
    }
}

public record CreateServiceRequest(
    string Name,
    string Description,
    List<CreateServiceStepRequest> Steps
);

public record CreateServiceStepRequest(
    string Name,
    string Description,
    int Order,
    bool RequiresFileUpload,
    bool RequiresTextInput,
    bool IsEquipmentAssignment
);

public record UpdateDescriptionRequest(string Description);
