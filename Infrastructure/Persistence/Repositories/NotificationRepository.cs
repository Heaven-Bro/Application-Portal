namespace Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Notifications;
using Domain.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<List<Notification>> GetByUserIdAsync(long userId, int take = 50, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId && (n.ExpiresAt == null || n.ExpiresAt > now))
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(long userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead && (n.ExpiresAt == null || n.ExpiresAt > now), cancellationToken);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddRangeAsync(notifications, cancellationToken);
    }

    public void Update(Notification notification)
    {
        _context.Notifications.Update(notification);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
