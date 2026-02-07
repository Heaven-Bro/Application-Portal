namespace Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Equipment.Commands;
using Application.Features.Equipment.Queries;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EquipmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? available = null,
        [FromQuery] bool? overdue = null)
    {
        var query = new GetAllEquipmentsQuery(page, pageSize, search, available, overdue);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var query = new GetEquipmentByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound(new { message = "Equipment not found" });
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentRequest request)
    {
        var command = new CreateEquipmentCommand(
            request.Name,
            request.EquipmentCode,
            request.Category,
            request.Condition,
            request.Description
        );
        
        var equipmentId = await _mediator.Send(command);
        return Ok(new { equipmentId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateEquipmentRequest request)
    {
        var command = new UpdateEquipmentCommand(
            id, 
            request.Name, 
            request.EquipmentCode, 
            request.Category,
            request.Condition,
            request.Description
        );
        
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id}/condition")]
    public async Task<IActionResult> UpdateCondition(long id, [FromBody] UpdateEquipmentConditionRequest request)
    {
        var command = new UpdateEquipmentConditionCommand(
            id,
            (Domain.Common.Enums.EquipmentCondition)request.Condition
        );

        await _mediator.Send(command);
        return Ok(new { message = "Equipment condition updated" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var command = new DeleteEquipmentCommand(id);
        await _mediator.Send(command);
        return Ok(new { message = "Equipment deleted successfully" });
    }
    [HttpGet("search")]
    public async Task<IActionResult> SearchAvailable([FromQuery] string? searchTerm = null)
    {
        var query = new SearchAvailableEquipmentQuery(searchTerm ?? string.Empty);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

}

public record CreateEquipmentRequest(
    string Name, 
    string EquipmentCode, 
    string Category,
    int Condition,
    string? Description
);

public record UpdateEquipmentRequest(
    string Name, 
    string EquipmentCode, 
    string Category,
    int Condition,
    string? Description
);

public record UpdateEquipmentConditionRequest(int Condition);
