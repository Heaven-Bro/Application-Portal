namespace Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Domain.Identity;
using Domain.Services;
using Domain.Applications;
using Domain.Equipment;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceStep> ServiceSteps => Set<ServiceStep>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<StepSubmission> StepSubmissions => Set<StepSubmission>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<EquipmentAssignment> EquipmentAssignments => Set<EquipmentAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
