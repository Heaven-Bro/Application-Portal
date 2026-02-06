namespace Infrastructure.Persistence.Repositories;

using Domain.Equipment;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Equipment?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Equipments
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipments
            .OrderBy(e => e.EquipmentCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Equipment>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipments
            .Where(e => e.IsAvailable)
            .OrderBy(e => e.EquipmentCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Equipment>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipments
            .Where(e => !e.IsAvailable)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default)
    {
        await _context.Equipments.AddAsync(equipment, cancellationToken);
    }

    public void Update(Equipment equipment)
    {
        _context.Equipments.Update(equipment);
    }

    public void Delete(Equipment equipment)
    {
        _context.Equipments.Remove(equipment);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
