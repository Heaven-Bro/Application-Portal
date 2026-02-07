namespace Application.Features.Notifications.Queries;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record GetMyNotificationsQuery(int Take = 50) : IRequest<Result<List<NotificationDto>>>;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<List<NotificationDto>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<NotificationDto>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == 0)
            return Result<List<NotificationDto>>.Failure("Unauthorized");

        var notifications = await _notificationRepository.GetByUserIdAsync(userId, request.Take, cancellationToken);

        var dtos = notifications.Select(n => new NotificationDto(
            n.Id,
            n.Type.ToString(),
            n.ReferenceId,
            n.Message,
            n.IsRead,
            n.CreatedAt)).ToList();

        return Result<List<NotificationDto>>.Success(dtos);
    }
}

public record NotificationDto(
    long Id,
    string Type,
    long? ReferenceId,
    string Message,
    bool IsRead,
    DateTime CreatedAt
);
