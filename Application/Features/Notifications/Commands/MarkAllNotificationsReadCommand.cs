namespace Application.Features.Notifications.Commands;

using MediatR;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Repositories;

public record MarkAllNotificationsReadCommand() : IRequest<Result>;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkAllNotificationsReadCommandHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == 0)
            return Result.Failure("Unauthorized");

        var notifications = await _notificationRepository.GetByUserIdAsync(userId, 200, cancellationToken);
        var unread = notifications.Where(n => !n.IsRead).ToList();

        if (unread.Count == 0)
            return Result.Success();

        foreach (var notification in unread)
        {
            notification.MarkAsRead(userId);
            _notificationRepository.Update(notification);
        }

        await _notificationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
