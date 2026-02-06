namespace Infrastructure.Persistence.Repositories;

using Domain.Documents;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class UserDocumentRepository : IUserDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public UserDocumentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDocument?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.UserDocuments
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);
    }

    public async Task<List<UserDocument>> GetByUserIdAsync(long userId, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = _context.UserDocuments.Where(d => d.UserId == userId);

        if (!includeDeleted)
        {
            query = query.Where(d => !d.IsDeleted);
        }

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserDocument>> GetByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.UserDocuments
            .Where(d => d.ApplicationId == applicationId && !d.IsDeleted)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserDocument>> GetByIdsAsync(List<long> ids, CancellationToken cancellationToken = default)
    {
        return await _context.UserDocuments
            .Where(d => ids.Contains(d.Id) && !d.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> BelongsToUserAsync(long documentId, long userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserDocuments
            .AnyAsync(d => d.Id == documentId && d.UserId == userId && !d.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(UserDocument document, CancellationToken cancellationToken = default)
    {
        await _context.UserDocuments.AddAsync(document, cancellationToken);
    }

    public void Update(UserDocument document)
    {
        _context.UserDocuments.Update(document);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
