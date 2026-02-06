namespace Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Applications;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.ToTable("applications");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.ServiceId).IsRequired();
        builder.Property(a => a.ApplicantId).IsRequired();
        
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.CurrentStep).IsRequired();
        builder.Property(a => a.ScheduledDateTime);
        builder.Property(a => a.CompletedAt);
        builder.Property(a => a.AdminNotes).HasMaxLength(2000);

        builder.HasMany(a => a.Submissions)
            .WithOne()
            .HasForeignKey(ss => ss.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.ApplicantId, a.Status });
        builder.HasIndex(a => a.CreatedAt);

        builder.Property(a => a.Version).IsConcurrencyToken();
        builder.Property(a => a.CreatedBy).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
    }
}
