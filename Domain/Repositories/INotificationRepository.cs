namespace Domain.Repositories;

using Domain.Notifications;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetByUserIdAsync(long userId, int take = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(long userId, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    void Update(Notification notification);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
