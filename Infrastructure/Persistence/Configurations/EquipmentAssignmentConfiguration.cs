namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Equipment;
using Domain.Common.Enums;

public class EquipmentAssignmentConfiguration : IEntityTypeConfiguration<EquipmentAssignment>
{
    public void Configure(EntityTypeBuilder<EquipmentAssignment> builder)
    {
        builder.ToTable("equipmentassignments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.ApplicationId)
            .IsRequired();

        builder.Property(e => e.EquipmentId)
            .IsRequired();

        builder.Property(e => e.AssignedAt)
            .IsRequired();

        builder.Property(e => e.ExpectedReturnDate);

        builder.Property(e => e.ReturnRequestedAt);

        builder.Property(e => e.ReturnVerifiedAt);

        builder.Property(e => e.ReturnVerifiedBy);

        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.AdminNotes);

        builder.Property(e => e.ApplicantResponse);

        builder.Property(e => e.DamageAcknowledgedAt);

        // Indexes
        builder.HasIndex(e => e.ApplicationId);
        builder.HasIndex(e => e.EquipmentId);
        builder.HasIndex(e => e.Status);
    }
}
