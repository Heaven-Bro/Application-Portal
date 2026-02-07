namespace Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Users.Queries;
using Application.Features.Users.Commands;
using Domain.Common.Enums;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? role = null)
    {
        var query = new GetAdminUsersQuery(search, status, role);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDetails(long id)
    {
        var query = new GetAdminUserDetailsQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { message = result.Error });
    }

    [HttpPut("{id:long}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetStatus(long id, [FromBody] UpdateUserStatusRequest request)
    {
        var command = new SetUserStatusCommand(id, request.Status);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "User status updated" });

        return BadRequest(new { message = result.Error });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var query = new GetCurrentUserProfileQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { message = result.Error });
    }

    [HttpPut("me/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileRequest request)
    {
        var command = new UpdateUserProfileCommand(
            request.PhotoPath,
            request.Phone,
            request.PermanentAddress,
            request.CurrentAddress,
            request.Year,
            request.Session,
            request.Semester,
            request.Faculty,
            request.Department);

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Profile updated" });

        return BadRequest(new { message = result.Error });
    }

    [HttpPut("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand(request.CurrentPassword, request.NewPassword);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Password updated" });

        return BadRequest(new { message = result.Error });
    }
}

public record UpdateUserStatusRequest(UserStatus Status);

public record UpdateUserProfileRequest(
    string? PhotoPath,
    string? Phone,
    string? PermanentAddress,
    string? CurrentAddress,
    string? Year,
    string? Session,
    string? Semester,
    string? Faculty,
    string? Department
);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
