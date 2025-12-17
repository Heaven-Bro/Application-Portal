namespace Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Equipment;
using Domain.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly ApplicationDbContext _context;

    public EquipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Equipment?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Equipments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Equipment>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipments
            .Where(e => e.IsAvailable)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipments
            .OrderBy(e => e.Name)
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

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
