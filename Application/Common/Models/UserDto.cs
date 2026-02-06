namespace Application.Common.Models;

public record UserDto(
    long Id,
    string Username,
    string Email,
    bool EmailConfirmed,
    string Role,
    string Status,
    string? PhotoPath,
    string? Phone,
    string? Department
);







