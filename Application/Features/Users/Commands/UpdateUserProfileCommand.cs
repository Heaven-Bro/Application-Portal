namespace Application.Features.Users.Commands;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record UpdateUserProfileCommand(
    string? PhotoPath,
    string? Phone,
    string? PermanentAddress,
    string? CurrentAddress,
    string? Year,
    string? Session,
    string? Semester,
    string? Faculty,
    string? Department
) : IRequest<Result>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == 0)
            return Result.Failure("Unauthorized");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        user.UpdateProfile(
            request.PhotoPath,
            request.Phone,
            request.PermanentAddress,
            request.CurrentAddress,
            request.Year,
            request.Session,
            request.Semester,
            request.Faculty,
            request.Department,
            userId);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
