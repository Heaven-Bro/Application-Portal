namespace Domain.Repositories;

using Domain.Services;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Service?> GetByIdWithStepsAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Service>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<List<Service>> GetAllAsync(CancellationToken cancellationToken = default); // ✅ ADD THIS
    Task<bool> HasPaidApplicationsAsync(long serviceId, CancellationToken cancellationToken = default);
    Task AddAsync(Service service, CancellationToken cancellationToken = default);
    Task DeleteAsync(Service service, CancellationToken cancellationToken = default);
    Task UpdateAsync(Service service, CancellationToken cancellationToken = default);
    void Update(Service service);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
