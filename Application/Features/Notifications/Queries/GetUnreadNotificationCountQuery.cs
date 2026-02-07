namespace Application.Features.Notifications.Queries;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record GetUnreadNotificationCountQuery() : IRequest<Result<int>>;

public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, Result<int>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUnreadNotificationCountQueryHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == 0)
            return Result<int>.Failure("Unauthorized");

        var count = await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
        return Result<int>.Success(count);
    }
}
