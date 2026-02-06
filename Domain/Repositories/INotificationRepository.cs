namespace Domain.Repositories;

using Domain.Notifications;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetUnreadByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetByUserIdAsync(long userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(long userId, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    void Update(Notification notification);
    Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
