namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Settings;

public class ChairmanAvailabilityConfiguration : IEntityTypeConfiguration<ChairmanAvailabilitySlot>
{
    public void Configure(EntityTypeBuilder<ChairmanAvailabilitySlot> builder)
    {
        builder.ToTable("chairman_availability");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.EndTime).IsRequired();

        builder.Property(s => s.Version).IsConcurrencyToken();
        builder.Property(s => s.CreatedBy).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
    }
}
