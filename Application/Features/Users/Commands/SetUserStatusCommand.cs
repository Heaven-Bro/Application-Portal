namespace Application.Features.Users.Commands;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;
using Domain.Common.Enums;

public record SetUserStatusCommand(long UserId, UserStatus Status) : IRequest<Result>;

public class SetUserStatusCommandHandler : IRequestHandler<SetUserStatusCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public SetUserStatusCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(SetUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        var currentUserId = _currentUserService.UserId;
        if (currentUserId == user.Id)
            return Result.Failure("You cannot change your own status");

        if (request.Status == UserStatus.Active)
            user.Enable(currentUserId);
        else
            user.Disable(currentUserId);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
