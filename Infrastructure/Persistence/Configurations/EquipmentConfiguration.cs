namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Equipment;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("equipment");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.EquipmentCode).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Category).HasMaxLength(100);

        builder.HasIndex(e => e.EquipmentCode).IsUnique();
        builder.HasIndex(e => e.IsAvailable);
    }
}
