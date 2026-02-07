namespace Domain.Repositories;

using Domain.Settings;

public interface IChairmanAvailabilityRepository
{
    Task<List<ChairmanAvailabilitySlot>> GetBetweenAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<ChairmanAvailabilitySlot> slots, CancellationToken cancellationToken = default);
    void RemoveRange(IEnumerable<ChairmanAvailabilitySlot> slots);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
