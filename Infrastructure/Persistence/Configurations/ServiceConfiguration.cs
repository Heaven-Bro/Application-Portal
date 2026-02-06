namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Services;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.Name).IsRequired().HasMaxLength(255);
        builder.Property(s => s.Description).HasMaxLength(1000);

        builder.Property(s => s.ServiceType)
            .IsRequired()
            .HasConversion<int>();

        builder.HasMany(s => s.Steps)
            .WithOne()
            .HasForeignKey(ss => ss.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.IsActive, s.ServiceType });

        builder.Property(s => s.Version).IsConcurrencyToken();
        builder.Property(s => s.CreatedBy).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
    }
}
