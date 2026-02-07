namespace Application.Features.Notifications.Commands;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record MarkNotificationReadCommand(long NotificationId) : IRequest<Result>;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkNotificationReadCommandHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == 0)
            return Result.Failure("Unauthorized");

        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification == null || notification.UserId != userId)
            return Result.Failure("Notification not found");

        if (!notification.IsRead)
        {
            notification.MarkAsRead(userId);
            _notificationRepository.Update(notification);
            await _notificationRepository.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
