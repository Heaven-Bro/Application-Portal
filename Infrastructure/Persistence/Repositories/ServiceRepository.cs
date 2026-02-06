namespace Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Services;
using Domain.Repositories;
using Domain.Common.Enums;

public class ServiceRepository : IServiceRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Services.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Service?> GetByIdWithStepsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Include(s => s.Steps)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Service>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Include(s => s.Steps)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPaidApplicationsAsync(long serviceId, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .Include(a => a.Submissions)
            .Where(a => a.ServiceId == serviceId)
            .AnyAsync(a => a.Submissions.Any(s => s.Status == StepSubmissionStatus.Approved), cancellationToken);
    }

    public async Task AddAsync(Service service, CancellationToken cancellationToken = default)
    {
        await _context.Services.AddAsync(service, cancellationToken);
    }

    public async Task DeleteAsync(Service service, CancellationToken cancellationToken = default)
    {
        _context.Services.Remove(service);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Service service, CancellationToken cancellationToken = default)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Service service)
    {
        _context.Services.Update(service);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task<List<Service>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Include(s => s.Steps)
            .ToListAsync(cancellationToken);
    }

}
