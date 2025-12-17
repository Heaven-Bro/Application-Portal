namespace Domain.Repositories;

using Domain.Services;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Service?> GetByIdWithStepsAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Service>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> HasPaidApplicationsAsync(long serviceId, CancellationToken cancellationToken = default);
    Task AddAsync(Service service, CancellationToken cancellationToken = default);
    void Update(Service service);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
