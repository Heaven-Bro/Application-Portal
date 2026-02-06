namespace Domain.Notifications;

using Domain.Common.Base;
using Domain.Common.Enums;

public sealed class Notification : Entity
{
    public long UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public long? ReferenceId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private Notification() { }

    public static Notification Create(long userId, NotificationType type, long? referenceId, 
        string message, DateTime? expiresAt, long createdBy)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            ReferenceId = referenceId,
            Message = message,
            IsRead = false,
            ExpiresAt = expiresAt
        };
        notification.MarkAsCreated(createdBy);
        return notification;
    }

    public void MarkAsRead(long userId)
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        MarkAsModified(userId);
    }
}
