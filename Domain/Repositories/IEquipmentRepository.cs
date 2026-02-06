namespace Domain.Repositories;

using Domain.Equipment;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Equipment>> GetAllAsync(CancellationToken cancellationToken = default); 
    Task<List<Equipment>> GetAvailableAsync(CancellationToken cancellationToken = default);
    Task<List<Equipment>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default);
    void Update(Equipment equipment);
    void Delete(Equipment equipment);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
