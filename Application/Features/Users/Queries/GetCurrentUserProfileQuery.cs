namespace Application.Features.Users.Queries;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record GetCurrentUserProfileQuery() : IRequest<Result<UserProfileDto>>;

public class GetCurrentUserProfileQueryHandler : IRequestHandler<GetCurrentUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserProfileQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserProfileDto>> Handle(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == 0)
            return Result<UserProfileDto>.Failure("Unauthorized");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result<UserProfileDto>.Failure("User not found");

        var dto = new UserProfileDto(
            user.Id,
            user.Name,
            user.Username,
            user.Email,
            user.Role.ToString(),
            user.Status.ToString(),
            user.PhotoPath,
            user.Phone,
            user.PermanentAddress,
            user.CurrentAddress,
            user.Year,
            user.Session,
            user.Semester,
            user.Faculty,
            user.Department
        );

        return Result<UserProfileDto>.Success(dto);
    }
}

public record UserProfileDto(
    long Id,
    string Name,
    string Username,
    string Email,
    string Role,
    string Status,
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
