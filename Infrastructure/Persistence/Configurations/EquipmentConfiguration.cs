namespace Infrastructure.Persistence.Configurations;

using Domain.Equipment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("equipment");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id");

        builder.Property(e => e.Name)
            .HasColumnName("Name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.EquipmentCode)
            .HasColumnName("EquipmentCode")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Category)
            .HasColumnName("Category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("Description")
            .HasMaxLength(1000);

        builder.Property(e => e.IsAvailable)
            .HasColumnName("IsAvailable")
            .IsRequired();

        builder.Property(e => e.Condition)
            .HasColumnName("Condition")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Version)
            .HasColumnName("Version")
            .IsConcurrencyToken();

        builder.Property(e => e.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(e => e.ModifiedBy)
            .HasColumnName("ModifiedBy");

        builder.Property(e => e.ModifiedAt)
            .HasColumnName("ModifiedAt");
    }
}
