namespace Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Applications;
using Domain.Repositories;
using Domain.Common.Enums;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Application?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Applications.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Application?> GetByIdWithSubmissionsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Application>> GetByApplicantIdAsync(long applicantId, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Application>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .Where(a => a.Status == status)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Application>> GetPendingForAdminAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .Include(a => a.Submissions)
            .Where(a => a.Status == ApplicationStatus.Submitted || a.Status == ApplicationStatus.InReview)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Application application, CancellationToken cancellationToken = default)
    {
        await _context.Applications.AddAsync(application, cancellationToken);
    }

    public void Update(Application application)
    {
        _context.Applications.Update(application);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
