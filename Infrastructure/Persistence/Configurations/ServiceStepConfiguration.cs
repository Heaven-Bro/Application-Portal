namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Services;

public class ServiceStepConfiguration : IEntityTypeConfiguration<ServiceStep>
{
    public void Configure(EntityTypeBuilder<ServiceStep> builder)
    {
        builder.ToTable("service_steps");

        builder.HasKey(ss => ss.Id);
        builder.Property(ss => ss.Id).ValueGeneratedOnAdd();

        builder.Property(ss => ss.Name).IsRequired().HasMaxLength(255);
        builder.Property(ss => ss.Description).HasMaxLength(1000);

        builder.HasIndex(ss => new { ss.ServiceId, ss.Order });
    }
}
