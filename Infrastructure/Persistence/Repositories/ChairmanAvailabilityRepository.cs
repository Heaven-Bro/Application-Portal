namespace Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Repositories;
using Domain.Settings;

public sealed class ChairmanAvailabilityRepository : IChairmanAvailabilityRepository
{
    private readonly ApplicationDbContext _context;

    public ChairmanAvailabilityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChairmanAvailabilitySlot>> GetBetweenAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await _context.ChairmanAvailabilitySlots
            .AsNoTracking()
            .Where(s => s.StartTime >= start && s.EndTime <= end)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<ChairmanAvailabilitySlot> slots, CancellationToken cancellationToken = default)
    {
        await _context.ChairmanAvailabilitySlots.AddRangeAsync(slots, cancellationToken);
    }

    public void RemoveRange(IEnumerable<ChairmanAvailabilitySlot> slots)
    {
        _context.ChairmanAvailabilitySlots.RemoveRange(slots);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
