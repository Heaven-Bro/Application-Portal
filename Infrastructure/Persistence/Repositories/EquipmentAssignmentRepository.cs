namespace Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Equipment;
using Domain.Repositories;
using Domain.Common.Enums;

public class EquipmentAssignmentRepository : IEquipmentAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentAssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EquipmentAssignment?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentAssignments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<EquipmentAssignment>> GetByApplicationIdAsync(long applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentAssignments
            .Where(ea => ea.ApplicationId == applicationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EquipmentAssignment>> GetByEquipmentIdAsync(long equipmentId, CancellationToken cancellationToken = default)
    {
        return await _context.EquipmentAssignments
            .Where(ea => ea.EquipmentId == equipmentId)
            .OrderByDescending(ea => ea.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EquipmentAssignment>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.EquipmentAssignments
            .Where(ea => ea.ExpectedReturnDate < today && 
                         ea.Status == EquipmentAssignmentStatus.CheckedOut)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AreAllReturnedAsync(long applicationId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.EquipmentAssignments
            .Where(ea => ea.ApplicationId == applicationId)
            .ToListAsync(cancellationToken);

        return assignments.All(a => a.Status == EquipmentAssignmentStatus.ReturnedGood || 
                                     a.Status == EquipmentAssignmentStatus.ReturnedDamaged);
    }

    public async Task AddAsync(EquipmentAssignment assignment, CancellationToken cancellationToken = default)
    {
        await _context.EquipmentAssignments.AddAsync(assignment, cancellationToken);
    }

    public void Update(EquipmentAssignment assignment)
    {
        _context.EquipmentAssignments.Update(assignment);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
