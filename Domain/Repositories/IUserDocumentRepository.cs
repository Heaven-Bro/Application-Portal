namespace Domain.Repositories;

using Domain.Documents;

public interface IUserDocumentRepository
{
    Task<UserDocument?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<UserDocument>> GetByUserIdAsync(long userId, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<List<UserDocument>> GetByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default);
    Task<List<UserDocument>> GetByIdsAsync(List<long> ids, CancellationToken cancellationToken = default);
    Task<bool> BelongsToUserAsync(long documentId, long userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserDocument document, CancellationToken cancellationToken = default);
    void Update(UserDocument document);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
