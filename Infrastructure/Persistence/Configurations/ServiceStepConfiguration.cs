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

        builder.Property(ss => ss.ServiceId).IsRequired();
        builder.Property(ss => ss.Name).IsRequired().HasMaxLength(255);
        builder.Property(ss => ss.Order).IsRequired();
        builder.Property(ss => ss.RequiresFileUpload).IsRequired();
        builder.Property(ss => ss.RequiresTextInput).IsRequired();
        builder.Property(ss => ss.Instructions).HasMaxLength(2000);
        builder.Property(ss => ss.DownloadableFormUrl).HasMaxLength(500);
        builder.Property(ss => ss.UploadConfig).HasColumnType("JSON");
        
        // ⚠️ THESE WERE MISSING - THIS IS THE BUG FIX
        builder.Property(ss => ss.IsOptional).IsRequired().HasDefaultValue(false);
        builder.Property(ss => ss.RequiresApproval).IsRequired().HasDefaultValue(true);

        builder.HasIndex(ss => new { ss.ServiceId, ss.Order });

        builder.Property(ss => ss.Version).IsConcurrencyToken();
        builder.Property(ss => ss.CreatedBy).IsRequired();
        builder.Property(ss => ss.CreatedAt).IsRequired();
    }
}
