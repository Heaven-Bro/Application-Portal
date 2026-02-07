namespace Domain.Repositories;

using Domain.Equipment;
using Domain.Common.Enums;

public interface IEquipmentAssignmentRepository
{
    Task<EquipmentAssignment?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<EquipmentAssignment>> GetByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default);
    Task<List<EquipmentAssignment>> GetByApplicationIdsAsync(List<long> applicationIds, CancellationToken cancellationToken = default);
    Task<List<EquipmentAssignment>> GetByEquipmentIdAsync(long equipmentId, CancellationToken cancellationToken = default);
    Task<List<EquipmentAssignment>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task<bool> AreAllReturnedAsync(long applicationId, CancellationToken cancellationToken = default);
    Task AddAsync(EquipmentAssignment assignment, CancellationToken cancellationToken = default);
    void Update(EquipmentAssignment assignment);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
