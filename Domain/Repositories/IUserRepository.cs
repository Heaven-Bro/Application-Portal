namespace Domain.Repositories;

using Domain.Identity;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<List<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default); // ✅ ADD THIS
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
